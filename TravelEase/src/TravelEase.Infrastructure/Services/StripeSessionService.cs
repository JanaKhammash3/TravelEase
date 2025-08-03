using Stripe.Checkout;
using TravelEase.Application.Interfaces;

namespace TravelEase.Infrastructure.Services;

public class StripeSessionService : IStripeSessionService
{
    public (string SessionId, string Url) CreateCheckoutSession(object request)
    {
        var options = request as SessionCreateOptions; 

        var session = new SessionService().Create(options!);
        return (session.Id, session.Url);
    }
}