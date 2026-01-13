using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RentalService.Application.Exceptions;
using RentalService.Domain.Exceptions;

namespace RentalService.Api.Controllers;

public sealed class ErrorHandlingFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var ex = context.Exception;

        if (ex is NotFoundException nf)
        {
            context.Result = new NotFoundObjectResult(new { error = nf.Message });
            context.ExceptionHandled = true;
            return;
        }

        if (ex is DomainException or InvalidOperationException)
        {
            context.Result = new BadRequestObjectResult(new { error = ex.Message });
            context.ExceptionHandled = true;
            return;
        }

        // Default: 500
        context.Result = new ObjectResult(new { error = "Unexpected error", detail = ex.Message })
        {
            StatusCode = 500
        };
        context.ExceptionHandled = true;
    }
}
