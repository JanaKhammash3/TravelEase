using FluentValidation;

namespace TravelEase.Application.Features.City.Validators
{
    public class UpdateCityCommandValidator : AbstractValidator<UpdateCityCommand>
    {
        public UpdateCityCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("City ID must be provided.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("City name is required.")
                .MaximumLength(100).WithMessage("City name cannot exceed 100 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country name cannot exceed 100 characters.");

            RuleFor(x => x.PostOffice)
                .NotEmpty().WithMessage("Post Office is required.")
                .MaximumLength(50).WithMessage("Post Office cannot exceed 50 characters.");
        }
    }
}