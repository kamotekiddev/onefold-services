using Application.Common.Exceptions;
using Domain.Entities.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DataAnnotationsValidationException = System.ComponentModel.DataAnnotations.ValidationException;
using FluentValidationException = FluentValidation.ValidationException;

namespace Onefold.Filters;

public sealed class ExceptionFilter(
    ILogger<ExceptionFilter> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;

        context.Result = exception switch
        {
            FluentValidationException ex => ErrorResponses.Validation(
                context.HttpContext,
                ex.Errors
                    .GroupBy(error => error.PropertyName ?? string.Empty)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray())),

            DataAnnotationsValidationException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status400BadRequest, ex.Message),

            UnAuthorizedException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status401Unauthorized, ex.Message),

            ForbiddenException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status403Forbidden, ex.Message),

            NotFoundException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status404NotFound, ex.Message),

            ConflictException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status409Conflict, ex.Message),

            CredentialAlreadyExistException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status409Conflict, ex.Message),

            WorkoutExerciseAlreadyExistsException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status409Conflict, ex.Message),

            WorkoutSessionExerciseAlreadyExistsException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status409Conflict, ex.Message),

            WorkoutSetAlreadyExistsException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status409Conflict, ex.Message),

            WorkoutExerciseDoesNotExistException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status404NotFound, ex.Message),

            WorkoutSessionExerciseDoesNotExistsException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status404NotFound, ex.Message),

            DomainException ex => ErrorResponses.Problem(
                context.HttpContext, StatusCodes.Status400BadRequest, ex.Message),

            _ => HandleUnexpectedException(context.HttpContext, exception)
        };

        context.ExceptionHandled = true;
    }

    private ObjectResult HandleUnexpectedException(
        HttpContext httpContext, Exception exception)
    {
        logger.LogError(
            exception,
            "An unexpected error occurred.");

        return ErrorResponses.Problem(
            httpContext,
            StatusCodes.Status500InternalServerError,
            "An unexpected error occurred.");
    }
}
