using FluentValidation;

namespace TravelEase.Application.Features.Room.Validators
{
    public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
    {
        public CreateRoomCommandValidator()
        {
            RuleFor(x => x.Number)
                .NotEmpty().WithMessage("Room number is required.")
                .MaximumLength(50).WithMessage("Room number cannot exceed 50 characters.");

            RuleFor(x => x.CapacityAdults)
                .GreaterThan(0).WithMessage("Adult capacity must be at least 1.")
                .LessThanOrEqualTo(10).WithMessage("Adult capacity cannot exceed 10.");

            RuleFor(x => x.CapacityChildren)
                .GreaterThanOrEqualTo(0).WithMessage("Children capacity must be 0 or more.")
                .LessThanOrEqualTo(10).WithMessage("Children capacity cannot exceed 10.");

            RuleFor(x => x.PricePerNight)
                .GreaterThan(0).WithMessage("Price per night must be greater than zero.");

            RuleFor(x => x.HotelId)
                .GreaterThan(0).WithMessage("Hotel ID must be provided.");
        }
    }
}