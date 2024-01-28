using DocumGen.Api.Common.Responses;
using DocumGen.Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DocumGen.Api.Common.ExceptionHandling
{
    public class AppExceptionHandler
    {
        private ILogger<AppExceptionHandler> _logger;

        public AppExceptionHandler(ILogger<AppExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleExceptionAsync(Exception ex, HttpContext context)
        {
            if (ex is ValidationException validationEx)
            {
                _logger.LogInformation(validationEx.Message);

                context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                List<string> errors = validationEx.Errors.Select(x => x.ErrorMessage).ToList();
                var response = BaseResponse.Failed("Validation failed", errors);
                var responseJson = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(responseJson);
            }
            else if (ex is ConfigurationException configurationEx)
            {
                _logger.LogError(configurationEx.Message);
                await WriteInternalError(context);
            }
            else
            {
                _logger.LogError(ex, ex.Message);
                await WriteInternalError(context);
            }
        }

        private static async Task WriteInternalError(HttpContext context)
        {
            context.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Json;
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var response = BaseResponse.Failed("Internal error", errors: null);
            var responseJson = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(responseJson);
        }
    }
}
