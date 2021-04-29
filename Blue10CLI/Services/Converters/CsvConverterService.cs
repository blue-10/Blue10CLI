using Blue10CLI.Services.Interfaces.Converters;
using Blue10SDK.Models;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Blue10CLI.Services.Converters
{
    public class CsvConverterService : ICsvConverterService
    {
        private readonly ILogger<CsvConverterService> _logger;

        public CsvConverterService(ILogger<CsvConverterService> logger)
        {
            _logger = logger;
        }

        #region Read CSV file
        public IList<T>? GetRecords<T>(string origin, string separator) where T : BaseObject, new()
        {
            var recordIsBad = false;
            var successfullRecords = new List<T>();
            var recordErrors = string.Empty;
            var errorCount = 0;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = separator,
                NewLine = Environment.NewLine,
                ReadingExceptionOccurred = context =>
                {
                    recordIsBad = true;
                    var recordError = ConvertCSVErrorMessage(context.Exception.Message, context.Exception.InnerException?.Message ?? string.Empty);
                    recordErrors += Environment.NewLine + recordError;
                    errorCount++;
                    return false;
                }
            };

            using var reader = new StringReader(origin);
            using (var csvReader = new CsvReader(reader, config))
            {
                while (csvReader.Read())
                {
                    var recordDynamic = csvReader.GetRecord<dynamic>();
                    if (!recordIsBad && recordDynamic != null)
                    {
                        var record = ConvertTo<T>(recordDynamic);
                        successfullRecords.Add(record);
                    }

                    recordIsBad = false;
                }
            }

            if (errorCount > 0)
            {
                _logger.LogError($"{errorCount} errors occurd while reading file, make sure to select the right format. {recordErrors}");
                return null;
            }

            return successfullRecords;
        }

        private T ConvertTo<T>(dynamic entity) where T : BaseObject, new()
        {
            // Get default object
            var record = new T();

            // Get all properties of the dynamic object
            var dictionary = new RouteValueDictionary(entity);
            // Get all properties of T
            PropertyInfo[] properties = typeof(T).GetProperties();

            // Iterate over properties of T and check if dynamic object got that property
            foreach (var property in properties)
            {
                var gotProperty = dictionary.TryGetValue(property.Name, out var value);

                if (!gotProperty)
                    continue;

                // Remove to check later if there are non-T properties
                dictionary.Remove(property.Name);

                if (value is null)
                    continue;

                var valueString = value as string;
                if (!string.IsNullOrEmpty(valueString))
                {
                    // Check what the type of the property should be. All values from dynamic object are string.
                    var type = property.PropertyType;

                    // Setup convert functions
                    var convertToType = new Dictionary<Type, Action> {
                        { typeof(string), () => property.SetValue(record, value) },
                        { typeof(bool), () =>
                            {
                                var newValue = bool.Parse(valueString);
                                property.SetValue(record, newValue);
                            }
                        },
                        { typeof(Guid), () =>
                            {
                                var newValue = Guid.Parse(valueString);
                                property.SetValue(record, newValue);
                            }
                        },
                        { typeof(decimal), () =>
                            {
                                var newValue = decimal.Parse(valueString);
                                property.SetValue(record, newValue);
                            }
                        },
                        { typeof(List<string>), () =>
                            {
                                var valueString = value as string;
                                if (!string.IsNullOrEmpty(valueString))
                                {
                                    var list = valueString.Replace("[", "").Replace("]", "").Split("|").ToList();

                                    if (list.Count > 0)
                                        property.SetValue(record, list);
                                }
                            }
                        },
                        { typeof(DateTime), () => { } },
                    };

                    // Convert value to correct type
                    try
                    {
                        convertToType[type]();
                    }
                    catch (FormatException ex)
                    {
                        _logger.LogError($"Error converting value '{valueString}' to type '{type}'. Make sure the value is a correct '{type}'");
                        throw ex;
                    }
                }
            }

            // Check, log and throw error if dictionary contains parameters that are not in the Type object
            if (dictionary.Count() > 0)
            {
                var unmatchedProperties = string.Join(", ", dictionary.Keys);
                var errorMessage = $"Error converting file. File contains properties that don't exist in {typeof(T)}: {unmatchedProperties}";

                _logger.LogError(errorMessage);
                throw new FormatException(errorMessage);
            }

            // Send back succesfull converted record
            return record;
        }

        private string ConvertCSVErrorMessage(string ErrorMessage, string Exception)
        {
            var ErrorMessageValues = ErrorMessage.Split(Environment.NewLine);

            var RowNumber = ErrorMessageValues[9].Split(':')[1].Trim();

            var Content = ErrorMessageValues[13];

            return $"Error at row {RowNumber} with the content \"{Content}\"{Environment.NewLine}- {Exception}";
        }

        #endregion

        #region Write csv file
        public string ConvertToCsv<T>(T objects, string separator)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = separator,
                NewLine = Environment.NewLine,
            };

            using var writer = new StringWriter();
            using var csvWriter = new CsvWriter(writer, config);

            csvWriter.Context.TypeConverterCache.AddConverter<List<string>>(new ListConverter());

            if (objects is IEnumerable enumerable)
                csvWriter.WriteRecords(enumerable);
            else
                csvWriter.WriteRecords(new[] { objects });
            return writer.ToString();
        }

        #endregion

        #region utilities

        private class ListConverter : ITypeConverter
        {
            public object? ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var list = text.Replace("[", "").Replace("]", "").Split("|").ToList();

                    if (list.Count > 0)
                    {
                        return list;
                    }

                }

                return null;
            }

            public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                if (value is List<string>)
                {
                    var list = string.Join("|", value as List<string>);
                    return $"[{list}]";
                }

                return string.Empty;
            }
        }

        #endregion
    }
}
