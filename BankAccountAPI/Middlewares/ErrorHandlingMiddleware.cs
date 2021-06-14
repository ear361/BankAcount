using System;
using System.Net;
using System.Threading.Tasks;
using BankAccountAPI.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BankAccountAPI.Middlewares
{
    /// <summary>
    ///     custom error handling middleware
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var path = context.Request.Path.HasValue ? context.Request.Path.ToString() : "no path provided";
            var user = context.Request.Headers["Username"].ToString();

            var response = new ErrorResponseModel();
            var statusCode = (int) HttpStatusCode.InternalServerError;
            var customException = exception as BaseCustomException;
            if (customException != null)
            {
                statusCode = customException.StatusCode;
            }

            response.Message = exception.Message;
            response.ReferenceNumber = DateTimeOffset.Now.ToUnixTimeSeconds();
            _logger.LogError(exception,
                $"'{context.Request.Method} {path}' by User: {user}, [Reference Number]{response.ReferenceNumber} - stack trace: \n{exception}");

            var responseStr = JsonConvert.SerializeObject(response);

            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsync(responseStr);
        }

        private class ErrorResponseModel
        {
            public long ReferenceNumber { get; set; }
            public string Message { get; set; }
        }
    }
}