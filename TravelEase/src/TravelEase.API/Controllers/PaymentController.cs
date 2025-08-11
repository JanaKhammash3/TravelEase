using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using TravelEase.TravelEase.API.Models;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Interfaces;

namespace TravelEase.TravelEase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IStripeSessionService _stripeService;

    public PaymentController(IStripeSessionService stripeService)
    {
        _stripeService = stripeService;
    }

    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession([FromBody] StripeCheckoutRequestDto dto)
    {
        var nights = (dto.CheckOut - dto.CheckIn).Days;
        var unitAmount = 10000L * nights;

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
                        UnitAmount = unitAmount,
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

        var (sessionId, url) = _stripeService.CreateCheckoutSession(options);

        return Ok(new StripeCheckoutResponseDto
        {
            SessionId = sessionId,
            Url = url
        });
    }
}
