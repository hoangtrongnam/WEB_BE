using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace WEB_BANHANG_BE.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, WEB_BANHANG_BE.Services.Logger.ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        logger.LogError($"Something went wrong: {contextFeature.Error}");

                        // await context.Response.WriteAsync(new WEB_BANHANG_BE.Models.Globally.ErrorModel()
                        // {
                        //     Code = "SYS_ERROR",
                        //     Message = "Internal Server Error."
                        // }.ToString());
                    }
                });
            });
        }
    }
}
