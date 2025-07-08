using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Features.Booking;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Infrastructure.Data;
using TravelEase.TravelEase.Infrastructure.Services;

namespace TravelEase.TravelEase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
     private readonly TravelEaseDbContext _context;
    private readonly IEmailService _emailService; 
    private readonly IBookingService _bookingService;


    public BookingController(
        TravelEaseDbContext context,
        IEmailService emailService,
        IBookingService bookingService)
    {
        _context = context;
        _emailService = emailService;
        _bookingService = bookingService;
    }


    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] BookingRequestDto dto)
    {
        var room = await _context.Rooms.Include(r => r.Hotel).FirstOrDefaultAsync(r => r.Id == dto.RoomId);
        if (room == null) return NotFound("Room not found");

        // Price = price per night * number of nights
        var days = (dto.CheckOut - dto.CheckIn).Days;
        var totalPrice = room.PricePerNight * days;

        var booking = new Booking
        {
            UserId = dto.UserId,
            RoomId = dto.RoomId,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut,
            Adults = dto.Adults,
            Children = dto.Children,
            SpecialRequests = dto.SpecialRequests,
            TotalPrice = totalPrice,
            PaymentStatus = dto.SimulatePaymentSuccess ? "Paid" : "Failed",
            PaymentMethod = dto.PaymentMethod
        };


        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Simulated confirmation number
        var confirmationNumber = $"HTL-{booking.Id:000000}";

        // Send confirmation email
        var user = await _context.Users.FindAsync(dto.UserId);

        string message = $"Booking Confirmed: {confirmationNumber}\n" +
                         $"Hotel: {room.Hotel.Name}\nRoom: {room.Number}\n" +
                         $"Check-In: {dto.CheckIn:yyyy-MM-dd} | Check-Out: {dto.CheckOut:yyyy-MM-dd}\n" +
                         $"Total: ${totalPrice}\nPaid via: {dto.PaymentMethod}\n" +
                         $"Status: {booking.PaymentStatus}\n\nThank you for booking with TravelEase!";


        if (user != null)
        {
            await _emailService.SendEmailAsync(user.Email, "Booking Confirmation", message);
        }


        // Create confirmation DTO
        var confirmationDto = new BookingConfirmationDto
        {
            BookingId = booking.Id,
            ConfirmationNumber = confirmationNumber,
            HotelName = room.Hotel.Name,
            HotelAddress = room.Hotel.Location ?? "N/A",
            RoomDetails = room.Number,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            TotalPrice = booking.TotalPrice,
            PaymentStatus = booking.PaymentStatus
        };

// Generate PDF and attach it to the email
        var pdfBytes = PdfReceiptGenerator.GenerateBookingReceipt(confirmationDto);
        await _emailService.SendEmailWithAttachmentAsync(user.Email, "Booking Confirmation", message, pdfBytes, "receipt.pdf");

// Return confirmation DTO
        return Ok(confirmationDto);

    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _bookingService.GetAllAsync());

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
        return Ok(new { message = "✅ Booking created successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _bookingService.DeleteAsync(id);
        return Ok(new { message = "🗑️ Booking deleted" });
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingCommand cmd)
    {
        if (id != cmd.Id) return BadRequest("Mismatched ID");
        await _bookingService.UpdateAsync(cmd);
        return Ok(new { message = "✅ Booking updated" });
    }

   [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] SearchBookingsQuery query)
    {
        var results = await _bookingService.SearchBookingsAsync(query);
        return Ok(results);
    }

}