using FluentValidation;

namespace TravelEase.Application.Features.Hotel.Validators
{
    public class SearchHotelsQueryValidator : AbstractValidator<SearchHotelsQuery>
    {
        public SearchHotelsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page number must be greater than zero.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue, () =>
            {
                RuleFor(x => x.MaxPrice.Value)
                    .GreaterThanOrEqualTo(x => x.MinPrice.Value)
                    .WithMessage("MaxPrice must be greater than or equal to MinPrice.");
            });

            When(x => x.CheckIn.HasValue && x.CheckOut.HasValue, () =>
            {
                RuleFor(x => x.CheckOut.Value)
                    .GreaterThan(x => x.CheckIn.Value)
                    .WithMessage("Check-out date must be after check-in date.");
            });

            RuleFor(x => x.StarRating)
                .InclusiveBetween(1, 5)
                .When(x => x.StarRating.HasValue)
                .WithMessage("Star rating must be between 1 and 5.");
        }
    }
}