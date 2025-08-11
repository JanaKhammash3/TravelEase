using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TravelEase.TravelEase.Application.DTOs;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;

namespace TravelEase.TravelEase.Application.Features.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginCommand command)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            var token = GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterCommand command)
        {
            if (await _userRepository.ExistsByEmailAsync(command.Email))
                throw new Exception("User already exists");

            var user = new User
            {
                FullName = command.FullName,
                Email = command.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Role = Domain.Enums.UserRole.User
            };

            await _userRepository.AddAsync(user);
            var token = GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        private string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
