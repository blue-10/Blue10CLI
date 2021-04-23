﻿using CsvHelper;
using CsvHelper.Configuration;
using DevLab.JmesPath;
using Newtonsoft.Json;
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

namespace Blue10CLI
{
    public enum EFormatType
    {
        JSON,
        CSV,
        TSV,
        SCSV,
        XML,
    }

    public static class Region
    {
        private static CultureInfo[] Cultures => CultureInfo.GetCultures(CultureTypes.SpecificCultures);
        private static RegionInfo[] All => Cultures.Select(x => new RegionInfo(x.LCID)).ToArray();

        public static string[] Codes => All
            .Select(x => x.TwoLetterISORegionName.ToUpper())
            .ToArray();

        public static string[] Currencies => All
            .Select(x => x.ISOCurrencySymbol.ToUpper())
            .ToArray();
    }


    internal static class Read
    {
        internal static IList<T> ReadAs<T>(this EFormatType format, string origin)
        {
            return format switch
            {
                EFormatType.JSON => JsonConvert.DeserializeObject<IList<T>>(origin),
                EFormatType.CSV => CsvRecords<T>(origin, ","),
                EFormatType.TSV => CsvRecords<T>(origin, "\t"),
                EFormatType.SCSV => CsvRecords<T>(origin, ";"),
                EFormatType.XML => XmlRecords<T>(origin),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, $"{format} is not supported for reading")
            };
        }

        internal static IList<T> CsvRecords<T>(string origin, string separator)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture, delimiter: separator, newLine: Environment.NewLine);

            using var reader = new StringReader(origin);
            using var csv = new CsvReader(reader, config);
            return csv.GetRecords<T>().ToList();
        }

        internal static IList<T> XmlRecords<T>(string input)
        {
            using var reader = new StringReader(input);

            // Must be List<T> instead of IList<T>, otherwise it will throw exception
            var serializer = new XmlSerializer(typeof(List<T>), new XmlRootAttribute($"ArrayOf{typeof(T).Name}"));

            var res = serializer.Deserialize(reader) as IList<T>;
            return res ?? new List<T>();
        }
    }

    public static class Output
    {
        public static string Format<T>(this EFormatType format, T input) where T : IEnumerable
        {
            return format switch
            {
                EFormatType.JSON => ConvertToJson(input),
                EFormatType.CSV => ConvertToCsv(input, ","),
                EFormatType.TSV => ConvertToCsv(input, "\t"),
                EFormatType.SCSV => ConvertToCsv(input, ";"),
                EFormatType.XML => ConvertToXml(input),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, $"{format} is not supported for formatting")
            };
        }

        public static string Filter(this EFormatType format, string inputString, string query)
        {
            return format switch
            {
                EFormatType.JSON => FilterWithJmesPath(inputString, query),
                EFormatType.XML => FilterWithXPath(inputString, query),
                EFormatType.CSV => throw new ArgumentOutOfRangeException(nameof(format), format, $"{format} is not supported for filtering with query"),
                EFormatType.TSV => throw new ArgumentOutOfRangeException(nameof(format), format, $"{format} is not supported for filtering with query"),
                EFormatType.SCSV => throw new ArgumentOutOfRangeException(nameof(format), format, $"{format} is not supported for filtering with query"),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, $"{format} is not supported for filtering with query")
            };
        }

        private static string FilterWithXPath(string xmlString, string xPathQuery)
        {
            using var xmlReader = new StringReader(xmlString);
            var xPathDocument = new XPathDocument(xmlReader);
            var xPathNav = xPathDocument.CreateNavigator();

            var resList = new List<string>();
            var res = xPathNav.Select(xPathQuery);

            foreach (dynamic? bla in res)
            {
                resList.Add((bla is null) ? "" : (bla.OuterXml ?? ""));
            }
            return string.Join('\n', resList);
        }

        private static string FilterWithJmesPath(string jsonString, string jmesPath)
        {
            JmesPath filterer = new JmesPath();
            return filterer.Transform(jsonString, jmesPath);
        }


        public static string ConvertToJson<T>(T subject, string jmesPath = "[]") =>
            JsonSerializer.Serialize(subject, new JsonSerializerOptions { WriteIndented = true });

        public static async Task HandleOutput<T>(this EFormatType format, T input, FileInfo? file, string? query = null)
        {
            string? resultString;
            if (input is IList inputEnumerable)
            {
                resultString = format.Format(inputEnumerable);
            }
            else
            {
                resultString = format.Format(new[] { input });
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                resultString = format.Filter(resultString, query!);
            }

            Console.WriteLine(resultString);
            if (file != null)
            {
                await File.WriteAllTextAsync(file.FullName, resultString);
            }
        }

        public static async Task HandleOutputToFilePath<T>(this EFormatType format, T input, string filepath, string? query = null)
        {
            string? resultString;
            if (input is IList inputEnumerable)
            {
                resultString = format.Format(inputEnumerable);
            }
            else
            {
                resultString = format.Format(new[] { input });
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                resultString = format.Filter(resultString, query!);
            }

            Console.WriteLine(resultString);
            await File.WriteAllTextAsync(filepath, resultString);
        }

        public class StringWriterWithEncoding : StringWriter
        {
            public override Encoding Encoding => _encoding ?? base.Encoding;
            private readonly Encoding _encoding;
            public StringWriterWithEncoding() { }
            public StringWriterWithEncoding(Encoding encoding)
            {
                _encoding = encoding;
            }
        }

        private static string ConvertToXml<T>(T input)
        {
            using var stringwriter = new StringWriter();
            var serializer1 = new XmlSerializer(input?.GetType());
            serializer1.Serialize(stringwriter, input);
            var xmlString = stringwriter.ToString();
            return xmlString;

        }

        private static string ConvertToCsv<T>(T origin, string separator)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture, delimiter: separator, newLine: Environment.NewLine);

            var wr = new StringWriter();
            using var csv = new CsvWriter(wr, config);

            if (origin is IEnumerable enumerable)
                csv.WriteRecords(enumerable);
            else
                csv.WriteRecords(new[] { origin });
            return wr.ToString();
        }
    }
}