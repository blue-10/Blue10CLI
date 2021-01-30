using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Xml;
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
    
    public static class Output
    {
        public static string Format<T>(this EFormatType format, T input) where T:IEnumerable
        {
            return format switch
            {
                EFormatType.JSON => JsonSerializer.Serialize(input, new JsonSerializerOptions {WriteIndented = true}),
                EFormatType.CSV => ConvertToCsv(input,","),
                EFormatType.TSV => ConvertToCsv(input,"\t"),
                EFormatType.SSV => ConvertToCsv(input,";"),
                EFormatType.XML => ConvertToXml(input),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        private static string ConvertToXml<T>(T origin)
        {
            using var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, origin);
            return stringwriter.ToString();
        }
        
        private static string ConvertToCsv<T>(T origin, string seperator)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                Delimiter = seperator
            };
            var wr = new StringWriter();
            using var csv = new CsvWriter(wr, config);
            csv.WriteRecords(new[]{origin});
            return wr.ToString();
        }
        
        private static string ConvertAllToCsv<T>(T origin, string seperator) where T:IEnumerable
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                Delimiter = seperator
            };
            
            var wr = new StringWriter();
            using var csv = new CsvWriter(wr, config);
            csv.WriteRecords(origin);
            return wr.ToString();
        }
    }
    
}