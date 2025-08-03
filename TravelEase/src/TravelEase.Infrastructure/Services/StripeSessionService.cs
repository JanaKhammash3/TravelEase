using Stripe.Checkout;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.TravelEase.Infrastructure.Services;

public class StripeSessionService : IStripeSessionService
{
    public (string SessionId, string Url) CreateCheckoutSession(object request)
    {
        var options = request as SessionCreateOptions; 

        var session = new SessionService().Create(options!);
        return (session.Id, session.Url);
    }
}