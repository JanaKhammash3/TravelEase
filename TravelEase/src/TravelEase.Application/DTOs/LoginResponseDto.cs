namespace TravelEase.TravelEase.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}