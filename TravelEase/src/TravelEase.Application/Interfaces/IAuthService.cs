using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Features.Auth;

namespace TravelEase.TravelEase.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginCommand command);
        Task<LoginResponseDto> RegisterAsync(RegisterCommand command);
    }
}