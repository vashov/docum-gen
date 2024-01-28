using System.Collections.Generic;

namespace DocumGen.Api.Common.Responses
{
    public class BaseResponse
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }
        public List<string> Errors { get; private set; }

        private BaseResponse() { }

        public static BaseResponse Ok()
        {
            return new BaseResponse
            {
                Success = true,
            };
        }

        public static BaseResponse Failed(string message, List<string> errors)
        {
            return new BaseResponse
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
