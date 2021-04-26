using Blue10CLI.Enums;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.Services.Interfaces
{
    public interface IInOutService
    {
        #region Input/Reader
        IList<T> ReadAs<T>(EFormatType format, string origin);

        string GetExtension(EFormatType format);

        #endregion

        #region Ouput/Writer

        Task HandleOutput<T>(EFormatType format, T input, FileInfo? file, string? query = null);

        Task HandleOutputToFilePath<T>(EFormatType format, T input, string filepath, string? query = null);

        #endregion
    }
}
