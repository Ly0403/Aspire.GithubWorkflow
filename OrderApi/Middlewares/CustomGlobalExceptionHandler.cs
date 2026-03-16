using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Shared.Abstracts;
using static OrderApi.Modules.Orders.GetOrderById;

namespace OrderApi.Middlewares;

public sealed class CustomGlobalExceptionHandler
    (ILogger<CustomGlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(message: exception.Message);

        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };


        httpContext.Response.StatusCode = statusCode;

        string errorMessage = exception switch
        {
            ValidationException => exception.Message,
            _ => "Server error",
        };

        HttpCustomResponse<string> response =
            new ()
            {
                Details = errorMessage,
                IsSuccess = false,
                Status = statusCode,
                Title = "Error"
            };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
