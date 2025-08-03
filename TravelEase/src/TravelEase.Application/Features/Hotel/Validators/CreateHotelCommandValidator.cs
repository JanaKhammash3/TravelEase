using FluentValidation;

namespace TravelEase.TravelEase.Application.Features.Hotel.Validators
{
    public class CreateHotelCommandValidator : AbstractValidator<CreateHotelCommand>
    {
        public CreateHotelCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Hotel name is required.")
                .MaximumLength(200).WithMessage("Hotel name cannot exceed 200 characters.");

            RuleFor(x => x.CityId)
                .GreaterThan(0).WithMessage("City ID must be provided.");

            RuleFor(x => x.Owner)
                .NotEmpty().WithMessage("Owner name is required.")
                .MaximumLength(100).WithMessage("Owner name cannot exceed 100 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(300).WithMessage("Location cannot exceed 300 characters.");

            RuleFor(x => x.StarRating)
                .InclusiveBetween(1, 5).WithMessage("Star rating must be between 1 and 5.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
        }
    }
}