using FluentValidation;

namespace TravelEase.TravelEase.Application.Features.Room.Validators
{
    public class SearchRoomsQueryValidator : AbstractValidator<SearchRoomsQuery>
    {
        public SearchRoomsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page number must be greater than zero.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue, () =>
            {
                RuleFor(x => x.MaxPrice.Value)
                    .GreaterThanOrEqualTo(x => x.MinPrice.Value)
                    .WithMessage("Max price must be greater than or equal to Min price.");
            });

            When(x => x.CheckIn.HasValue && x.CheckOut.HasValue, () =>
            {
                RuleFor(x => x.CheckOut.Value)
                    .GreaterThan(x => x.CheckIn.Value)
                    .WithMessage("Check-out date must be after check-in date.");
            });

            When(x => x.Adults.HasValue, () =>
            {
                RuleFor(x => x.Adults.Value)
                    .GreaterThan(0).WithMessage("Adults must be at least 1.");
            });

            When(x => x.Children.HasValue, () =>
            {
                RuleFor(x => x.Children.Value)
                    .GreaterThanOrEqualTo(0).WithMessage("Children must be 0 or more.");
            });
        }
    }
}