using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using MEDCHGWEB.Services;

namespace MEDCHGWEB.Middleware
{
    public class ActivityTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        public ActivityTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            // Check if the user is authenticated
            if (context.User.Identity.IsAuthenticated)
            {
                // Resolve the SessionTracker from the service provider
                //var sessionTracker = serviceProvider.GetRequiredService<SessionTracker>();
                //sessionTracker.UpdateActivity(); // Update last activity time

                //if (sessionTracker.IsTimedOut())
                //{
                //    // Sign out the user
                //    await context.SignOutAsync();
                //    context.Response.Redirect("/login"); // Redirect to login page
                //    return;
                //}
            }

            await _next(context); // Call the next middleware
        }
    }
}
