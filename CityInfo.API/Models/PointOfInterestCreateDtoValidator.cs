// Ignore Spelling: Validator Dto

using FluentValidation;

namespace CityInfo.API.Models;

public class PointOfInterestCreateDtoValidator : AbstractValidator<PointOfInterestCreateDto>
{
    public PointOfInterestCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Please provide a valid name")
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .MaximumLength(500) .WithMessage("Description should be less than 500 characters")
            .When(x => x.Description != null);
    }
}
