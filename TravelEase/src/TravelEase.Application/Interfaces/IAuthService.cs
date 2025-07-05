using TravelEase.TravelEase.Application.Features.Auth;

namespace TravelEase.TravelEase.Application.Interfaces;

public interface IAuthService
{
    Task<AuthService.LoginResponseDto> LoginAsync(LoginCommand command);
    Task<AuthService.LoginResponseDto> RegisterAsync(RegisterCommand command);

}