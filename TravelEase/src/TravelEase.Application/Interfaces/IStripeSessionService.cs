namespace TravelEase.Application.Interfaces;
public interface IStripeSessionService
{
    (string SessionId, string Url) CreateCheckoutSession(object options);
}