using Common;
using Common.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using WebFramework.Api;

namespace WebFramework.Middlewares
{
    public class CustomExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;
        private readonly IHostingEnvironment _env;
        public CustomExceptionHandlingMiddleware(RequestDelegate next,ILogger<CustomExceptionHandlingMiddleware> logger,IHostingEnvironment env)
        {
             _next=next;            
            _logger=logger;
            _env=env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string message = null;
            var apiResultStatusCode = ApiResultStatusCode.ServerError;
            var httpStatusCode = HttpStatusCode.InternalServerError;

            try
            {
                await _next(httpContext);
            }
            catch (AppException exception)
            {
                apiResultStatusCode = exception.ApiStatusCode;
                httpStatusCode = exception.HttpStatusCode;

                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>();
                    dic["Message"] = exception.Message;
                    dic["StackTrace"] = exception.StackTrace;

                    if (exception.InnerException != null)
                    {
                        dic["InnerException.Message"] = exception.InnerException.Message;
                        dic["InnerException.StackTrace"] = exception.InnerException.StackTrace;
                    }

                    if (exception.AdditionalData != null)
                        dic["AdditionalDate"] = JsonConvert.SerializeObject(exception.AdditionalData);
                    message = JsonConvert.SerializeObject(dic);
                }
                else
                    message = exception.Message;

                _logger.LogError(message, exception);
                await WriteToResponseAsync();
            }
            catch (UnauthorizedAccessException exception)
            {
                await SetUnAuthorizedResponse(exception);
                await WriteToResponseAsync();
            }
            catch (SecurityTokenExpiredException exception)
            {
                await SetUnAuthorizedResponse(exception);
                await WriteToResponseAsync();
            }
            catch (Exception exception)
            {
                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>();
                    dic["Message"] = exception.Message;
                    dic["StackTrace"] = exception.StackTrace;
                    if (exception.InnerException != null)
                    {
                        dic["InnerException.Message"] = exception.InnerException.Message;
                        dic["InnerException.StackTrace"] = exception.InnerException.StackTrace;
                    }
                    message = JsonConvert.SerializeObject(dic);
                }
                else
                    message = exception.Message;

                _logger.LogError(message, exception);
                await WriteToResponseAsync();
            }

            async Task WriteToResponseAsync()
            {
                if (httpContext.Response.HasStarted)
                    await httpContext.Response.WriteAsync("The Response Process Has Started");
                var result = new ApiResult(false, apiResultStatusCode, message);
                httpContext.Response.StatusCode = (int) httpStatusCode;
                httpContext.Response.ContentType = "application/json";
                var json = JsonConvert.SerializeObject(result);
                await httpContext.Response.WriteAsync(json);
            }

            async Task SetUnAuthorizedResponse(Exception exception)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;
                apiResultStatusCode = ApiResultStatusCode.UnAuthorized;
                if (_env.IsDevelopment())
                {
                    var dic = new Dictionary<string, string>();
                    dic["Message"] = exception.Message;
                    dic["StackTrace"] = exception.StackTrace;
                    if (exception.InnerException != null)
                    {
                        dic["InnerException.Message"] = exception.InnerException.Message;
                        dic["InnerException.StackTrace"] = exception.InnerException.StackTrace;
                    }

                    if (exception is SecurityTokenExpiredException ex)
                        dic["Expires"] = ex.Expires.ToString();
                    message = JsonConvert.SerializeObject(dic);
                }
                else
                    message = exception.Message;
                _logger.LogError(message, exception);
            }
        }
    }
}
