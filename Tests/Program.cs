using Application.Common.Exceptions;
using Application.Features.Authentication.EmailSignUp;
using Application.Features.Authentication.RefreshAccessToken;
using Application.Features.Workout.Session.SaveSession;
using Application.Features.Workout.Template.CreateWorkoutTemplate;
using Domain.Entities.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Onefold.Filters;

var checks = 0;

void Check(bool condition, string description)
{
    if (!condition)
        throw new Exception($"Failed: {description}");

    checks++;
}

ObjectResult Filter(Exception exception)
{
    var context = new ExceptionContext(
        new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
        [])
    {
        Exception = exception
    };

    new ExceptionFilter(NullLogger<ExceptionFilter>.Instance).OnException(context);
    Check(context.ExceptionHandled, "exception marked handled");
    return (ObjectResult)context.Result!;
}

async Task<JsonDocument> Execute(ObjectResult result)
{
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddControllers();
    using var provider = services.BuildServiceProvider();
    await using var body = new MemoryStream();
    var httpContext = new DefaultHttpContext
    {
        RequestServices = provider,
        Response = { Body = body }
    };
    await result.ExecuteResultAsync(new ActionContext(
        httpContext, new RouteData(), new ActionDescriptor()));
    body.Position = 0;
    return await JsonDocument.ParseAsync(body);
}

void CheckProblem(Exception exception, int status, string message)
{
    var result = Filter(exception);
    var problem = (ProblemDetails)result.Value!;
    Check(result.StatusCode == status && problem.Status == status, $"status {status}");
    Check(problem.Detail == message, $"message {message}");
    Check(problem.Extensions.ContainsKey("traceId"), "trace ID included");
    Check(result.ContentTypes.Contains("application/problem+json"), "problem content type");
}

CheckProblem(new UnAuthorizedException("Invalid credentials."), 401, "Invalid credentials.");
CheckProblem(new ForbiddenException("Access denied."), 403, "Access denied.");
CheckProblem(new NotFoundException("Missing."), 404, "Missing.");
CheckProblem(new ConflictException("Duplicate."), 409, "Duplicate.");
CheckProblem(new CredentialAlreadyExistException(), 409, "Credential already exists.");
CheckProblem(new WorkoutExerciseDoesNotExistException("Exercise missing."), 404, "Exercise missing.");
CheckProblem(new DomainException("Invalid set."), 400, "Invalid set.");
CheckProblem(new InvalidOperationException("Sensitive internal detail."), 500, "An unexpected error occurred.");

var validation = new ValidationException([
    new ValidationFailure("Name", "Name is required."),
    new ValidationFailure("Name", "Name is too long."),
    new ValidationFailure("Exercises[0].TargetReps", "Target reps must be positive.")
]);
var validationResult = Filter(validation);
var validationProblem = (ValidationProblemDetails)validationResult.Value!;
Check(validationResult.StatusCode == 400, "FluentValidation returns 400");
Check(validationProblem.Errors["Name"].Length == 2, "all errors for one field returned");
Check(validationProblem.Errors["Exercises[0].TargetReps"].Single() == "Target reps must be positive.",
    "nested field path retained");
using (var json = await Execute(validationResult))
{
    var root = json.RootElement;
    Check(root.GetProperty("status").GetInt32() == 400, "serialized status");
    Check(root.GetProperty("errors").GetProperty("Name").GetArrayLength() == 2,
        "serialized errors contain every message");
    Check(root.GetProperty("errors").GetProperty("Exercises[0].TargetReps")[0].GetString() ==
          "Target reps must be positive.", "serialized nested field path");
    Check(!string.IsNullOrWhiteSpace(root.GetProperty("traceId").GetString()),
        "serialized trace ID");
}

var modelState = new ModelStateDictionary();
modelState.AddModelError("Name", "Name is required.");
modelState.AddModelError("Name", "Name is too long.");
modelState.AddModelError("Exercises[0].TargetReps", "Invalid number.");
var modelResult = ErrorResponses.Validation(new DefaultHttpContext(), modelState);
var modelProblem = (ValidationProblemDetails)modelResult.Value!;
Check(modelResult.StatusCode == 400, "model binding returns 400");
Check(modelProblem.Errors["Name"].Length == 2, "model binding returns all messages");
Check(modelProblem.Errors["Exercises[0].TargetReps"].Single() == "Invalid number.",
    "model binding retains nested field path");

var signupErrors = new EmailSignUpRequestValidator()
    .Validate(new EmailSignUpRequest("invalid", ""));
Check(signupErrors.Errors.Any(error => error.PropertyName == "Email"), "email format validated");
Check(signupErrors.Errors.Any(error => error.PropertyName == "Password"), "password required");
Check(!new RefreshAccessTokenRequestValidator()
    .Validate(new RefreshAccessTokenRequest("")).IsValid, "refresh token required");

var nullExercises = new SaveSessionRequestValidator().Validate(new SaveSessionRequest(
    Guid.NewGuid(), DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMinutes(1), null!));
Check(nullExercises.Errors.Any(error => error.PropertyName == "Exercises"),
    "null exercise collection reports validation error");

var nullExerciseItem = new SaveSessionRequestValidator().Validate(new SaveSessionRequest(
    Guid.NewGuid(), DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMinutes(1),
    [null!]));
Check(nullExerciseItem.Errors.Any(error => error.PropertyName == "Exercises[0]"),
    "null exercise item reports validation error");

var nullSets = new SaveSessionExerciseRequestValidator().Validate(new SaveSessionExerciseRequest(
    Guid.NewGuid(), 0, null!));
Check(nullSets.Errors.Any(error => error.PropertyName == "Sets"),
    "null set collection reports validation error");

var templateErrors = new CreateWorkoutTemplateRequestValidator().Validate(
    new CreateWorkoutTemplateRequest("", null, 0, null!));
Check(templateErrors.Errors.Select(error => error.PropertyName)
    .Intersect(["Name", "RestInMinutes", "Exercises"]).Count() == 3,
    "independent template errors collected together");

var startedAt = DateTimeOffset.UtcNow;
try
{
    await new SaveSessionRequestValidator().ValidateAndThrowAsync(new SaveSessionRequest(
        Guid.NewGuid(), startedAt, startedAt.AddMinutes(-1),
        [new SaveSessionExerciseRequest(
            Guid.NewGuid(), 0,
            [new SaveSessionWorkoutSetRequest(0, 0, null, null)])]));
    throw new Exception("Expected nested validation to fail.");
}
catch (ValidationException exception)
{
    var nestedProblem = (ValidationProblemDetails)Filter(exception).Value!;
    Check(nestedProblem.Errors.ContainsKey("CompletedAt"), "request rule returned");
    Check(nestedProblem.Errors.ContainsKey("Exercises[0].Sets[0].SetNumber"),
        "first nested set rule returned");
    Check(nestedProblem.Errors.ContainsKey("Exercises[0].Sets[0].Reps"),
        "second nested set rule returned");
}

Console.WriteLine($"Passed {checks} regression checks.");
