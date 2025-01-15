using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Web;

namespace MEDCHGWEB.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(context, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error from the custom middleware.",
                Detailed = exception.Message
            };
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["statusCode"] = response.StatusCode.ToString();
            query["message"] = response.Message;
            query["detailed"] = response.Detailed;

            string redirectUrl = $"/ErrorPage?{query.ToString()}";
            context.Response.Redirect(redirectUrl);
            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
