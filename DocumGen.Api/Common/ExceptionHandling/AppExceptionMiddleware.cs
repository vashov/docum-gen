using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DocumGen.Api.Common.ExceptionHandling
{
    public class AppExceptionMiddleware : IMiddleware
    {
        private readonly AppExceptionHandler _exceptionHandler;

        public AppExceptionMiddleware(AppExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                await _exceptionHandler.HandleExceptionAsync(ex, context);
            }
        }
    }
}
