using Blue10SDK.Models;

namespace Blue10CLI.models
{
    public class BaseResultModel<T> where T : BaseObject
    {
        public BaseResultModel(T? pObject, string? pErrorMessage)
        {
            Object = pObject;
            ErrorMessage = pErrorMessage;
        }

        public T? Object { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
