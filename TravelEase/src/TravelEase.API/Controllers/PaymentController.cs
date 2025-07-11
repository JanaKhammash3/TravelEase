using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using TravelEase.TravelEase.API.Models;

namespace TravelEase.TravelEase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession([FromBody] StripeCheckoutRequestDto dto)
    {
        // Price estimation (simple: 1 night = $100, or replace with real logic)
        var nights = (dto.CheckOut - dto.CheckIn).Days;
        var unitAmount = 10000L * nights; // $100/night -> Stripe uses cents

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = unitAmount, // Stripe expects amount in cents
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Hotel Room Booking for User #{dto.UserId}"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = dto.SuccessUrl + "?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = dto.CancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "userId", dto.UserId.ToString() },
                { "roomId", dto.RoomId.ToString() },
                { "checkIn", dto.CheckIn.ToString("o") },
                { "checkOut", dto.CheckOut.ToString("o") },
                { "adults", dto.Adults.ToString() },
                { "children", dto.Children.ToString() },
                { "specialRequests", dto.SpecialRequests }
            }
        };

        var service = new SessionService();
        var session = service.Create(options);

        return Ok(new { sessionId = session.Id, url = session.Url });
    }
}
