using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using CsvHelper;
using CsvHelper.Configuration;
using DevLab.JmesPath;

namespace Blue10CLI
{
    public enum EFormatType
    {
        JSON,
        CSV,
        TSV,
        SSV,
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


    public static class Output
    {
    

        public static string Format<T>(this EFormatType format, T input) where T : IEnumerable
        {
            return format switch
            {
                EFormatType.JSON => ConvertToJson(input),
                EFormatType.CSV => ConvertToCsv(input, ","),
                EFormatType.TSV => ConvertToCsv(input, "\t"),
                EFormatType.SSV => ConvertToCsv(input, ";"),
                EFormatType.XML => ConvertToXml(input),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }
        
        public static string Filter(this EFormatType format, string inputString, string query)
        {
            return format switch
            {
                EFormatType.JSON => FilterWithJmesPath(inputString, query),
                EFormatType.XML => FilterWithXPath(inputString, query),
                EFormatType.CSV => throw new ArgumentOutOfRangeException(nameof(format), format, null),
                EFormatType.TSV => throw new ArgumentOutOfRangeException(nameof(format), format, null),
                EFormatType.SSV => throw new ArgumentOutOfRangeException(nameof(format), format, null),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        private static string FilterWithXPath(string xmlString, string xPathQuery)
        {
            using var xmlReader = new StringReader(xmlString);
            var xPathDocument = new XPathDocument(xmlReader);
            var xPathNav = xPathDocument.CreateNavigator();
            
            var resList = new List<string>();
            var res = xPathNav.Select(xPathQuery);
            
            foreach (dynamic bla in res)
            {
                resList.Add( bla.OuterXml ?? "");
            }
            return string.Join('\n',resList);
        }

        private static string FilterWithJmesPath(string jsonString, string jmesPath)
        {
            JmesPath filterer = new ();
            return filterer.Transform(jsonString, jmesPath);
        }


        public static string ConvertToJson<T>(T subject, string jmesPath = "[]") => 
            JsonSerializer.Serialize(subject, new JsonSerializerOptions {WriteIndented = true});

        public static async Task HandleOutput<T>(this EFormatType format, T input, FileInfo? file, string? query = null)
        {
            string? resultString;
            if (input is IList inputEnumerable)
            {
                resultString = format.Format(inputEnumerable);
            }
            else
            {
                resultString = format.Format(new[] {input});
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
            /*var removeDefaultXmlNamespaces = true;
            var omitXmlDeclaration = true;
            var encoding = Encoding.UTF8;
                
                 
                
            XmlSerializerNamespaces? namespaces = (removeDefaultXmlNamespaces ? new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }) : null);
 
            var settings                = new XmlWriterSettings();
            settings.Indent             = true;
            settings.OmitXmlDeclaration = omitXmlDeclaration;
            settings.CheckCharacters    = false;
 
            using (var stream = new StringWriterWithEncoding(encoding))
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(input?.GetType()!);
                serializer.Serialize(writer, input, namespaces);
                return stream.ToString();
            }*/
        }

        private static string ConvertToCsv<T>(T origin, string separator)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                Delimiter = separator
            };
            var wr = new StringWriter();
            using var csv = new CsvWriter(wr, config);

            if (origin is IEnumerable enumerable)
                csv.WriteRecords(enumerable);
            else
                csv.WriteRecords(new[] {origin});
            return wr.ToString();
        }
    }
}