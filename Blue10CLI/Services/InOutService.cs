using Blue10CLI.Enums;
using Blue10CLI.Services.Interfaces;
using Blue10CLI.Services.Interfaces.Converters;
using Blue10SDK.Models;
using DevLab.JmesPath;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Blue10CLI.Services
{
    public class InOutService : IInOutService
    {
        private readonly ICsvConverterService _csvConverter;
        private readonly ILogger<InOutService> _logger;

        public InOutService(ICsvConverterService csvConverter, ILogger<InOutService> logger)
        {
            _csvConverter = csvConverter;
            _logger = logger;
        }

        #region Input/Reader
        public IList<T>? ReadAs<T>(EFormatType format, string origin) where T : BaseObject, new()
        {
            IList<T>? objectList;
            try
            {
                objectList = format switch
                {
                    EFormatType.JSON => JsonSerializer.Deserialize<IList<T>>(origin),
                    EFormatType.CSV => _csvConverter.GetRecords<T>(origin, ","),
                    EFormatType.TSV => _csvConverter.GetRecords<T>(origin, "\t"),
                    EFormatType.SCSV => _csvConverter.GetRecords<T>(origin, ";"),
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

        public async Task HandleOutput<T>(EFormatType format, T input, FileInfo? file, string? query = null)
        {
            string? resultString;
            if (input is IList inputEnumerable)
            {
                resultString = Format(format, inputEnumerable);
            }
            else
            {
                resultString = Format(format, new[] { input });
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                resultString = Filter(format, resultString, query!);
            }

            Console.WriteLine(resultString);
            if (file != null)
            {
                await File.WriteAllTextAsync(file.FullName, resultString);
            }
        }

        public async Task HandleOutputToFilePath<T>(EFormatType format, T input, string filepath, string? query = null)
        {
            string? resultString;
            if (input is IList inputEnumerable)
            {
                resultString = Format(format, inputEnumerable);
            }
            else
            {
                resultString = Format(format, new[] { input });
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                resultString = Filter(format, resultString, query!);
            }

            Console.WriteLine(resultString);
            await File.WriteAllTextAsync(filepath, resultString);
        }

        private string Format<T>(EFormatType format, T input) where T : IEnumerable
        {
            return format switch
            {
                EFormatType.JSON => ConvertToJson(input),
                EFormatType.CSV => _csvConverter.ConvertToCsv(input, ","),
                EFormatType.TSV => _csvConverter.ConvertToCsv(input, "\t"),
                EFormatType.SCSV => _csvConverter.ConvertToCsv(input, ";"),
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

        #endregion

        #region Utilities
        private ArgumentOutOfRangeException LogAndThrow(EFormatType format, string message)
        {
            _logger.LogError(message);
            return new ArgumentOutOfRangeException(nameof(format), format, message);
        }

        #endregion
    }

}