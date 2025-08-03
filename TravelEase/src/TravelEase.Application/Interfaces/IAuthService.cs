using TravelEase.Application.DTOs;
using TravelEase.Application.Features.Auth;

namespace TravelEase.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginCommand command);
        Task<LoginResponseDto> RegisterAsync(RegisterCommand command);
    }
}