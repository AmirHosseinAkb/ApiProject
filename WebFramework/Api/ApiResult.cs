using Common;
using Common.Utilities;

namespace WebFramework.Api
{
    public class ApiResult
    {
        public ApiResult(bool isSucceeded,ApiResultStatusCode statusCode,string message=null)
        {
            IsSucceeded = isSucceeded;
            StatusCode= statusCode;
            Message= message?? statusCode.ToDisplay();
        }
        public string Message { get; set; }
        public ApiResultStatusCode StatusCode { get; set; }
        public bool IsSucceeded { get; set; }
    }

    public class ApiResult<TData>
    {
        public ApiResult(TData data)
        {
            Data = data;
        }
        public TData Data { get; set; }

    }
}
