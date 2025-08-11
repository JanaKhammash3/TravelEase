using FluentValidation;

namespace TravelEase.TravelEase.Application.Features.City.Validators
{
    public class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
    {
        public CreateCityCommandValidator()
        {
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