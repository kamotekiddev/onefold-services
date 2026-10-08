using System.ComponentModel.DataAnnotations;
using Application.Common.Exceptions;
using Domain.Entities.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Onefold.Filters;

public sealed class ExceptionFilter(
    ILogger<ExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        context.Result = exception switch
        {
            ValidationException ex => CreateResponse(
                StatusCodes.Status400BadRequest,
                ex.Message),

            NotFoundException ex => CreateResponse(
                StatusCodes.Status404NotFound,
                ex.Message),

            ConflictException ex => CreateResponse(
                StatusCodes.Status409Conflict,
                ex.Message),

            DomainException ex => CreateResponse(
                StatusCodes.Status400BadRequest,
                ex.Message),

            _ => HandleUnexpectedException(exception)
        };

        context.ExceptionHandled = true;
    }

    private static ObjectResult CreateResponse(
        int statusCode,
        string message)
    {
        return new ObjectResult(new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(statusCode),
            Detail = message
        })
        {
            StatusCode = statusCode
        };
    }

    private ObjectResult HandleUnexpectedException(Exception exception)
    {
        logger.LogError(
            exception,
            "An unexpected error occurred.");

        return CreateResponse(
            StatusCodes.Status500InternalServerError,
            "An unexpected error occurred.");
    }

    private static string GetTitle(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status409Conflict => "Conflict",
            StatusCodes.Status500InternalServerError => "Internal Server Error",
            _ => "Error"
        };
    }
}