using Microsoft.AspNetCore.Mvc;
using TravelEase.TravelEase.Application.Features.Booking;

namespace TravelEase.TravelEase.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly BookingService _bookingService;

    public BookingController(BookingService bookingService)
    {
        _bookingService = bookingService;
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