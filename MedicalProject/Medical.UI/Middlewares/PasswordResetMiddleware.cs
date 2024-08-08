using System;
using Medical.UI.Extentions;

namespace Medical.UI.Middlewares
{
    public class PasswordResetMiddleware
    {
        private readonly RequestDelegate _next;

        public PasswordResetMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/admin") &&
                !context.Session.GetBool("PasswordResetRequired") &&
                context.Request.Cookies.ContainsKey("token"))
            {
                context.Response.Redirect("/Account/ResetPassword");
                return;
            }

            await _next(context);
        }
    }
}

