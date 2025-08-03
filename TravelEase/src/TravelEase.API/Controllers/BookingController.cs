using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using TravelEase.API.Models;
using TravelEase.Application.DTOs;
using TravelEase.Application.Features.Booking;
using TravelEase.Application.Interfaces;
using TravelEase.Domain.Entities;
using TravelEase.Infrastructure.Data;
using TravelEase.Infrastructure.Services;
using PaymentMethod = TravelEase.Domain.Enums.PaymentMethod;

namespace TravelEase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly TravelEaseDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IBookingService _bookingService;
        private readonly IConfiguration _config;

        public BookingController(
            TravelEaseDbContext context,
            IEmailService emailService,
            IBookingService bookingService,
            IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _bookingService = bookingService;
            _config = config;
        }

        [HttpPost("stripe-checkout")]
        public async Task<IActionResult> CreateStripeSession([FromBody] StripeCheckoutRequestDto dto)
        {
            var room = await _context.Rooms.Include(r => r.Hotel).FirstOrDefaultAsync(r => r.Id == dto.RoomId);
            if (room == null) return NotFound("Room not found");

            var days = (dto.CheckOut - dto.CheckIn).Days;
            var total = room.PricePerNight * days;

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
                            UnitAmount = (long)(total * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Hotel Booking - {room.Hotel.Name}"
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

        [HttpPost("stripe-webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeSecret = _config["Stripe:WebhookSecret"];
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                stripeSecret
            );

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Session;

                var userId = int.Parse(session.Metadata["userId"]);
                var roomId = int.Parse(session.Metadata["roomId"]);
                var checkIn = DateTime.Parse(session.Metadata["checkIn"]);
                var checkOut = DateTime.Parse(session.Metadata["checkOut"]);
                var adults = int.Parse(session.Metadata["adults"]);
                var children = int.Parse(session.Metadata["children"]);
                var specialRequests = session.Metadata["specialRequests"];

                var room = await _context.Rooms.Include(r => r.Hotel).FirstOrDefaultAsync(r => r.Id == roomId);
                if (room == null) return BadRequest("Room not found");

                var days = (checkOut - checkIn).Days;
                var totalPrice = room.PricePerNight * days;

                var booking = new Booking
                {
                    UserId = userId,
                    RoomId = roomId,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    Adults = adults,
                    Children = children,
                    SpecialRequests = specialRequests,
                    TotalPrice = totalPrice,
                    PaymentStatus = "Paid",
                    PaymentMethod = PaymentMethod.Stripe 
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                var confirmationNumber = $"HTL-{booking.Id:000000}";
                var user = await _context.Users.FindAsync(userId);

                var message = $"Booking Confirmed: {confirmationNumber}\n" +
                              $"Hotel: {room.Hotel.Name}\nRoom: {room.Number}\n" +
                              $"Check-In: {checkIn:yyyy-MM-dd} | Check-Out: {checkOut:yyyy-MM-dd}\n" +
                              $"Total: ${totalPrice}\nPaid via: Stripe\n" +
                              $"Status: Paid\n\nThank you for booking with TravelEase!";

                var confirmationDto = new BookingConfirmationDto
                {
                    BookingId = booking.Id,
                    ConfirmationNumber = confirmationNumber,
                    HotelName = room.Hotel.Name,
                    HotelAddress = room.Hotel.Location ?? "N/A",
                    RoomDetails = room.Number,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    TotalPrice = totalPrice,
                    PaymentStatus = "Paid"
                };

                var pdfBytes = PdfReceiptGenerator.GenerateBookingReceipt(confirmationDto);

                if (user != null)
                    await _emailService.SendEmailWithAttachmentAsync(user.Email, "Booking Confirmation", message, pdfBytes, "receipt.pdf");
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            return booking == null ? NotFound() : Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand cmd)
        {
            await _bookingService.CreateAsync(cmd);
            return Ok(new { message = "Booking created successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bookingService.DeleteAsync(id);
            return Ok(new { message = "Booking deleted" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingCommand cmd)
        {
            if (id != cmd.Id) return BadRequest("Mismatched ID");
            await _bookingService.UpdateAsync(cmd);
            return Ok(new { message = "Booking updated" });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchBookingsQuery query)
        {
            var results = await _bookingService.SearchBookingsAsync(query);
            return Ok(results);
        }
    }
}
