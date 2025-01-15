using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MEDCHGWEB.Middleware
{
    public class XssProtectionMiddleware
    {
        private readonly RequestDelegate _next;

        public XssProtectionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            if (context.Request.ContentType != null && context.Request.ContentType.Contains("multipart/form-data"))
            {
                var boundary = GetBoundary(MediaTypeHeaderValue.Parse(context.Request.ContentType));
                var reader = new MultipartReader(boundary, context.Request.Body);

                var section = await reader.ReadNextSectionAsync();
                while (section != null)
                {
                    if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                    {
                        if (contentDisposition.IsFileDisposition())
                        {
                            // Handle file content if needed
                        }
                        else if (contentDisposition.IsFormDisposition())
                        {
                            using (var streamReader = new StreamReader(section.Body, Encoding.UTF8))
                            {
                                var formDataValue = await streamReader.ReadToEndAsync();
                                if (ContainsHtmlTags(formDataValue))
                                {
                                    var response = new { isSuccess = false, message = "Potential XSS attack detected." };
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    context.Response.ContentType = "application/json";
                                    var jsonResponse = JsonSerializer.Serialize(response);
                                    await context.Response.WriteAsync(jsonResponse);
                                    return;
                                }
                            }
                        }
                    }
                    section = await reader.ReadNextSectionAsync();
                }

                // Reset the stream position for further processing
                context.Request.Body.Position = 0;
            }
            else if (context.Request.ContentType != null && context.Request.ContentType.Contains("application/json"))
            {
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    if (ContainsHtmlTags(body))
                    {
                        var response = new { isSuccess = false, message = "Potential XSS attack detected." };
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        context.Response.ContentType = "application/json";
                        var jsonResponse = JsonSerializer.Serialize(response);
                        await context.Response.WriteAsync(jsonResponse);
                        return;
                    }
                }
            }

            await _next(context);
        }

        private bool ContainsHtmlTags(string input)
        {
            // Check for any HTML tags
            //var htmlTagRegex = new Regex(@"<[^>]+>", RegexOptions.IgnoreCase);
            var htmlTagRegex = new Regex(@"<(?!\/?br\s*\/?)[^>]+>", RegexOptions.IgnoreCase);
            return htmlTagRegex.IsMatch(input);
        }

        private static string GetBoundary(MediaTypeHeaderValue contentType)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }
            return boundary;
        }
    }
}
