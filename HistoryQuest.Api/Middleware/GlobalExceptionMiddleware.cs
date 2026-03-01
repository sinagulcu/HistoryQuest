using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using HistoryQuest.Domain.Exceptions;

namespace HistoryQuest.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (FluentValidation.ValidationException ex)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var response = new
            {
                status = 400,
                errors
            };

            await context.Response.WriteAsJsonAsync(response);
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = ex.StatusCode,
                message = ex.Message
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        //catch (Exception)
        //{
        //    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //    context.Response.ContentType = "application/json";
        //    var response = new
        //    {
        //        status = 500,
        //        message = "An unexpected error occurred."
        //    };
        //    await context.Response.WriteAsJsonAsync(response);
        //}
    }
}
