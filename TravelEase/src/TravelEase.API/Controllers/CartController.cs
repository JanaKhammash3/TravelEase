using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Infrastructure.Data;

namespace TravelEase.TravelEase.API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly TravelEaseDbContext _context;

        public CartController(TravelEaseDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemDto dto)
        {
            var cartItem = new CartItem
            {
                UserId = dto.UserId,
                RoomId = dto.RoomId,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                Adults = dto.Adults,
                Children = dto.Children
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();

            return Ok("Item added to cart");
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await _context.CartItems
                .Include(c => c.Room)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return Ok(cart);
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var item = await _context.CartItems.FindAsync(itemId);
            if (item == null) return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok("Item removed");
        }
    }
}