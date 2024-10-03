using Effortless.Core.Domain.Definitions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Effortless.Core.Middlewares;

public sealed class ExceptionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Handle the exception and generate an appropriate response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = AppConstant.Status.Code.InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = ex.Message.ToString(), code = AppConstant.Status.Key.InternalServerError });
        }
    }
}

public static class ExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionMiddleware(this WebApplication app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
