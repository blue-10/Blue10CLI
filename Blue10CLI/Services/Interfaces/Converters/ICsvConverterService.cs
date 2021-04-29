using Blue10SDK.Models;
using System.Collections.Generic;

namespace Blue10CLI.Services.Interfaces.Converters
{
    public interface ICsvConverterService
    {
        IList<T>? GetRecords<T>(string origin, string separator) where T : BaseObject, new();

        string ConvertToCsv<T>(T objects, string separator);
    }
}
