using FluentValidation;

namespace TravelEase.TravelEase.Application.Features.Booking.Validators
{
    public class SearchBookingsQueryValidator : AbstractValidator<SearchBookingsQuery>
    {
        public SearchBookingsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page number must be greater than zero.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

            When(x => x.FromDate.HasValue && x.ToDate.HasValue, () =>
            {
                RuleFor(x => x.ToDate.Value)
                    .GreaterThanOrEqualTo(x => x.FromDate.Value)
                    .WithMessage("ToDate must be after or equal to FromDate.");
            });
        }
    }
}