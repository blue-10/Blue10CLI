using Blue10CLI.Enums;
using Blue10CLI.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using DevLab.JmesPath;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Blue10CLI.Services
{
    public class InOutService : IInOutService
    {
        public ILogger<InOutService> _logger { get; }

        public InOutService(ILogger<InOutService> logger)
        {
            _logger = logger;
        }

        #region Input/Reader
        public IList<T>? ReadAs<T>(EFormatType format, string origin)
        {
            IList<T>? objectList;
            try
            {
                objectList = format switch
                {
                    EFormatType.JSON => JsonSerializer.Deserialize<IList<T>>(origin),
                    EFormatType.CSV => CsvRecords<T>(origin, ","),
                    EFormatType.TSV => CsvRecords<T>(origin, "\t"),
                    EFormatType.SCSV => CsvRecords<T>(origin, ";"),
                    EFormatType.XML => XmlRecords<T>(origin),
                    _ => throw LogAndThrow(format, $"{format} is not supported for reading")
                };
            }
            catch (Exception ex) when (
                ex is JsonException
                || ex is InvalidOperationException)
            {
                _logger.LogError("Invalid input file. Check if format of the file is correct and if Id values of GLAccounts are valid");
                throw;
            }

            return objectList;
        }

        public string GetExtension(EFormatType format)
        {
            return format switch
            {
                EFormatType.JSON => ".json",
                EFormatType.CSV => ".csv",
                EFormatType.TSV => ".tsv",
                EFormatType.SCSV => ".scsv",
                EFormatType.XML => ".xml",
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, $"{format} is not supported for getting extension")
            };
        }

        private IList<T>? CsvRecords<T>(string origin, string separator)
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
                csvReader.Context.TypeConverterCache.AddConverter<List<string>>(new ListConverter());

                while (csvReader.Read())
                {
                    var record = csvReader.GetRecord<T>();
                    if (!recordIsBad)
                    {
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

        private string ConvertCSVErrorMessage(string ErrorMessage, string Exception)
        {
            var ErrorMessageValues = ErrorMessage.Split(Environment.NewLine);

            var RowNumber = ErrorMessageValues[9].Split(':')[1].Trim();

            var Content = ErrorMessageValues[13];

            return $"Error at row {RowNumber} with the content \"{Content}\"{Environment.NewLine}- {Exception}";
        }

        private IList<T>? XmlRecords<T>(string input)
        {
            using var reader = new StringReader(input);

            // Must be List<T> instead of IList<T>, otherwise it will throw exception
            var serializer = new XmlSerializer(typeof(List<T>), new XmlRootAttribute($"ArrayOf{typeof(T).Name}"));

            var res = serializer.Deserialize(reader) as IList<T>;
            return res;
        }

        #endregion

        #region Output/Writer

        public async Task<bool> HandleOutput<T>(EFormatType format, T input, FileInfo? file, string? query = null)
        {
            string? resultString;
            if (input is IList inputEnumerable)
                resultString = Format(format, inputEnumerable);
            else
                resultString = Format(format, new[] { input });

            if (!string.IsNullOrWhiteSpace(query))
                resultString = Filter(format, resultString, query!);

            if (!string.IsNullOrWhiteSpace(resultString))
            {
                Console.WriteLine(resultString);
                if (file != null)
                    await File.WriteAllTextAsync(file.FullName, resultString);

                return true;
            }

            return false;
        }

        public async Task<bool> HandleOutputToFilePath<T>(EFormatType format, T input, string filepath, string? query = null)
        {
            string? resultString;
            if (input is IList inputEnumerable)
                resultString = Format(format, inputEnumerable);
            else
                resultString = Format(format, new[] { input });

            if (!string.IsNullOrWhiteSpace(query))
                resultString = Filter(format, resultString, query!);

            if (!string.IsNullOrWhiteSpace(resultString))
            {
                Console.WriteLine(resultString);
                await File.WriteAllTextAsync(filepath, resultString);
                return true;
            }

            return false;
        }

        private string Format<T>(EFormatType format, T input) where T : IEnumerable
        {
            return format switch
            {
                EFormatType.JSON => ConvertToJson(input),
                EFormatType.CSV => ConvertToCsv(input, ","),
                EFormatType.TSV => ConvertToCsv(input, "\t"),
                EFormatType.SCSV => ConvertToCsv(input, ";"),
                EFormatType.XML => ConvertToXml(input),
                _ => throw LogAndThrow(format, $"{format} is not supported for formatting")
            };
        }

        private string Filter(EFormatType format, string inputString, string query)
        {
            return format switch
            {
                EFormatType.JSON => FilterWithJmesPath(inputString, query),
                EFormatType.XML => FilterWithXPath(inputString, query),
                EFormatType.CSV => throw LogAndThrow(format, $"{format} is not supported for filtering with query"),
                EFormatType.TSV => throw LogAndThrow(format, $"{format} is not supported for filtering with query"),
                EFormatType.SCSV => throw LogAndThrow(format, $"{format} is not supported for filtering with query"),
                _ => throw LogAndThrow(format, $"{format} is not supported for filtering with query")
            };

        }

        private string FilterWithXPath(string xmlString, string xPathQuery)
        {
            using var xmlReader = new StringReader(xmlString);
            var xPathDocument = new XPathDocument(xmlReader);
            var xPathNav = xPathDocument.CreateNavigator();

            var resList = new List<string>();
            try
            {
                var res = xPathNav.Select(xPathQuery);

                foreach (dynamic? bla in res)
                {
                    resList.Add(bla is null ? "" : bla.OuterXml ?? "");
                }
                return string.Join('\n', resList);
            }
            catch (XPathException xpe)
            {
                _logger.LogError("Filter '{0}' is not a valid XPATH", xPathQuery, xpe.Message);
                throw;
            }
        }

        private string FilterWithJmesPath(string jsonString, string jmesPath)
        {
            JmesPath filterer = new JmesPath();
            return filterer.Transform(jsonString, jmesPath);
        }

        private string ConvertToJson<T>(T subject, string jmesPath = "[]") =>
            JsonSerializer.Serialize(subject, new JsonSerializerOptions { WriteIndented = true });

        private class StringWriterWithEncoding : StringWriter
        {
            public override Encoding Encoding => _encoding ?? base.Encoding;
            private readonly Encoding _encoding;
            public StringWriterWithEncoding() { }
            public StringWriterWithEncoding(Encoding encoding)
            {
                _encoding = encoding;
            }
        }

        private string ConvertToXml<T>(T input)
        {
            using var stringwriter = new StringWriter();
            var serializer1 = new XmlSerializer(input?.GetType());
            serializer1.Serialize(stringwriter, input);
            var xmlString = stringwriter.ToString();
            return xmlString;

        }

        private string ConvertToCsv<T>(T objects, string separator)
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

        #region Utilities
        private ArgumentOutOfRangeException LogAndThrow(EFormatType format, string message)
        {
            _logger.LogError(message);
            return new ArgumentOutOfRangeException(nameof(format), format, message);
        }

        public class ListConverter : ITypeConverter
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