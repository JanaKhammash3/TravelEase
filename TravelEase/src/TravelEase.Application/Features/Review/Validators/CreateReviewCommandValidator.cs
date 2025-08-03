using FluentValidation;

namespace TravelEase.Application.Features.Review.Validators
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.HotelId)
                .GreaterThan(0).WithMessage("Hotel ID must be provided.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be provided.");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5 stars.");

            RuleFor(x => x.Comment)
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
        }
    }
}