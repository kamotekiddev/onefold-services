using FluentValidation;

namespace Application.Features.Workout.Exercise.SearchExercises;

public sealed class SearchExercisesRequestValidator : AbstractValidator<SearchExercisesRequest>
{
    public SearchExercisesRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .Must(name => name is null || name.Trim().Length <= 255)
            .WithMessage("Name must be 255 characters or fewer after trimming.");
        RuleFor(request => request.Page)
            .GreaterThan(0)
            .Must((request, page) => ((long)page - 1) * request.PageSize <= int.MaxValue)
            .WithMessage("Page and PageSize exceed the supported result offset.");
        RuleFor(request => request.PageSize).InclusiveBetween(1, 100);
    }
}
