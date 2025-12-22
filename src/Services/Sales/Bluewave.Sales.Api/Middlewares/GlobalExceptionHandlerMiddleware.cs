using FluentValidation;
using System.Net;
using System.Text.Json;

namespace Bluewave.Sales.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Try to process the request normally
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Verifies if it's a validation error (FluentValidation)
        if (exception is ValidationException validationException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = new
            {
                status = 400,
                message = "Erro de validação",
                errors = validationException.Errors.Select(e => new
                {
                    field = e.PropertyName,
                    error = e.ErrorMessage
                })
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        // If it's any other unhandled error (real 500 error)
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errorResponse = new
        {
            status = 500,
            message = "Ocorreu um erro interno no servidor.",
            detail = exception.Message
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}