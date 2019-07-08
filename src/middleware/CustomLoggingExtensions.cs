using System;
using Microsoft.AspNetCore.Builder;

namespace request_response_middleware.middleare
{
    public static class ApiLoggingHandlerExtensions {

        public static IApplicationBuilder UseCustomRequestResponseLogger (this IApplicationBuilder app) {
            if (app == null) {
                throw new ArgumentException (nameof (app));
            }
            return app.UseMiddleware<CustomLoggingMiddleware> ();
        }
    }
}
