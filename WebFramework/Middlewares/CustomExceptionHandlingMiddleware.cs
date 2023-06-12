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
            string message=null;

            var httpStatusCode=HttpStatusCode.InternalServerError;
            var apiResultStatusCode=ApiResultStatusCode.ServerError;

            try
            {
                await _next(httpContext);
            }
            catch (AppException exception)
            {
                
                _logger.LogError(exception.Message,exception);
                if(_env.IsDevelopment())
                {
                    httpStatusCode=exception.HttpStatusCode;
                    apiResultStatusCode=exception.ApiStatusCode;

                    var dic=new Dictionary<string,string>();

                    dic["Exception"]=exception.Message;
                    dic["StackTrace"]=exception.StackTrace;

                    if(exception.InnerException!=null)
                    {
                        dic["InnerException.Exception"]=exception.InnerException.Message;
                        dic["InnerException.StackTrace"]=exception.InnerException.StackTrace;
                    }
                    if (exception.AdditionalData != null)
                    {
                        dic["AdditionalData"]=JsonConvert.SerializeObject(exception.AdditionalData);
                    }
                    message=JsonConvert.SerializeObject(dic);
                }
                else
                {
                    message=exception.Message;
                }
                await WriteToResponseAsync();
            }
            catch(UnauthorizedAccessException exception)
            {
                _logger.LogError(exception.Message,exception);
                await SetUnAuthorizedResponse(exception);
                await WriteToResponseAsync();
            }
            catch(SecurityTokenExpiredException exception)
            {
                _logger.LogError(exception.Message,exception);
                await SetUnAuthorizedResponse(exception);
                await WriteToResponseAsync();
            }
            catch(Exception exception)
            {
                _logger.LogError(exception.Message,exception);
                var result=new ApiResult(false,Common.ApiResultStatusCode.ServerError);
                await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }


            async Task WriteToResponseAsync()
            {
                if(httpContext.Response.HasStarted)
                    await httpContext.Response.WriteAsync("The Respnse Process Has Started");

                var result=new ApiResult(false,apiResultStatusCode,message);
                var json=JsonConvert.SerializeObject(result);
                httpContext.Response.StatusCode=(int)httpStatusCode;
                httpContext.Response.ContentType="application/json";
                await httpContext.Response.WriteAsync(json);
            }

            async Task SetUnAuthorizedResponse(Exception ex)
            {
                httpStatusCode=HttpStatusCode.Unauthorized;
                apiResultStatusCode=ApiResultStatusCode.UnAuthorized;
                if (_env.IsDevelopment())
                {
                    var dic=new Dictionary<string,string>();
                    dic["Message"]=ex.Message;
                    dic["StackTrace"]=ex.StackTrace;

                    if (ex.InnerException != null)
                    {
                        dic["InnerException.Message"]=ex.InnerException.Message;
                        dic["InnerException.StackTrace"]=ex.InnerException.StackTrace;
                    }

                    if(ex is SecurityTokenExpiredException exception)
                        dic["Expired"]=exception.Expires.ToString();
                    message=JsonConvert.SerializeObject(dic);
                }
            }
        }
    }
}
