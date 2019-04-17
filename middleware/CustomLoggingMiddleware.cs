using System;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace request_response_middleware.middleare{
    public class CustomLoggingMiddleware {
        private readonly RequestDelegate _next;

        public CustomLoggingMiddleware (RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke (HttpContext httpContext, ILogger<CustomLoggingMiddleware> _logger) {
            var request = httpContext.Request;
            // if only apis will be logged
            if (request.Path.StartsWithSegments (new PathString ("/api"))) {
                var requestTime = DateTime.UtcNow;
                var requestBodyContent = await ReadRequestBody (request);
                var originalBodyStream = httpContext.Response.Body;
                string responseBodyContent = null;
                var response = httpContext.Response;

                using (var responseBody = new MemoryStream ()) {
                    response.Body = responseBody;
                    try {
                        await _next (httpContext);
                        responseBodyContent = await ReadResponseBody (response);
                    } catch (System.Exception exp) {

                        responseBodyContent = JsonConvert.SerializeObject (exp);
                    }
                    await responseBody.CopyToAsync (originalBodyStream);
                }

                _logger.LogInformation (JsonConvert.SerializeObject (responseBodyContent));


            } else
                await _next (httpContext);
        }

        private async Task<string> ReadRequestBody (HttpRequest request) {

            request.EnableRewind ();
            var buffer = new byte[Convert.ToInt32 (request.ContentLength)];
            await request.Body.ReadAsync (buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString (buffer);
            request.Body.Seek (0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody (HttpResponse response) {
            response.Body.Seek (0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader (response.Body).ReadToEndAsync ();
            response.Body.Seek (0, SeekOrigin.Begin);

            return bodyAsText;
        }
    }
}