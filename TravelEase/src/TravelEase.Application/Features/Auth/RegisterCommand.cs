namespace TravelEase.TravelEase.Application.Features.Auth;

public class RegisterCommand
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}