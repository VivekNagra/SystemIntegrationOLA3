using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using BillingService.Domain.Exceptions;

namespace BillingService.Api.Controllers;

public sealed class ErrorHandlingFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var ex = context.Exception;

        if (ex is DomainException or InvalidOperationException)
        {
            context.Result = new BadRequestObjectResult(new { error = ex.Message });
            context.ExceptionHandled = true;
            return;
        }

        context.Result = new ObjectResult(new { error = "Unexpected error", detail = ex.Message })
        {
            StatusCode = 500
        };
        context.ExceptionHandled = true;
    }
}
