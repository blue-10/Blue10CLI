using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;

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
                EFormatType.JSON => JsonSerializer.Serialize(input, new JsonSerializerOptions {WriteIndented = true}),
                EFormatType.CSV => ConvertToCsv(input, ","),
                EFormatType.TSV => ConvertToCsv(input, "\t"),
                EFormatType.SSV => ConvertToCsv(input, ";"),
                EFormatType.XML => ConvertToXml(input),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        public static async Task HandleOutput<T>(this EFormatType format, T input, FileInfo? file)
        {
            var resultString = format.Format(new[] {input});
            Console.WriteLine(resultString);
            if (file != null)
            {
                await File.WriteAllTextAsync(file.FullName, resultString);
            }
        }

        private static string ConvertToXml<T>(T origin)
        {
            using var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, origin);
            return stringwriter.ToString();
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