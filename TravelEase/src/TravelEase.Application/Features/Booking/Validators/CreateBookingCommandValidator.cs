using FluentValidation;

namespace TravelEase.Application.Features.Booking.Validators
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be provided.");

            RuleFor(x => x.RoomId)
                .GreaterThan(0).WithMessage("Room ID must be provided.");

            RuleFor(x => x.CheckIn)
                .GreaterThan(DateTime.Today).WithMessage("Check-in date must be in the future.");

            RuleFor(x => x.CheckOut)
                .GreaterThan(x => x.CheckIn).WithMessage("Check-out date must be after check-in date.");

            RuleFor(x => x.Adults)
                .InclusiveBetween(1, 10).WithMessage("Adults must be between 1 and 10.");

            RuleFor(x => x.Children)
                .InclusiveBetween(0, 10).WithMessage("Children must be between 0 and 10.");

            RuleFor(x => x.SpecialRequests)
                .MaximumLength(500).WithMessage("Special requests cannot exceed 500 characters.");
        }
    }
}