using Microsoft.AspNetCore.Mvc;
using TravelEase.Application.Features.Auth;
using TravelEase.Application.Interfaces;

namespace TravelEase.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var token = await _authService.RegisterAsync(command);
            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var token = await _authService.LoginAsync(command);
            return Ok(new { Token = token });
        }
    }
}