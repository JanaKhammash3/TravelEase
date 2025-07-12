using System;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using TravelEase.TravelEase.Application.Features.Auth;
using TravelEase.TravelEase.Application.Interfaces;
using TravelEase.TravelEase.Domain.Entities;
using TravelEase.TravelEase.Domain.Enums;

namespace TravelEase.TravelEase.Tests.UserUnitTests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _configMock = new Mock<IConfiguration>();

        _configMock.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyThatIsLongEnoughToWork");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(_userRepoMock.Object, _configMock.Object);
    }

    [Fact(DisplayName = "✅ Valid registration — RegisterAsync returns token")]
    public async Task RegisterAsync_ShouldReturnToken_WhenUserIsNew()
    {
        var command = new RegisterCommand
        {
            FullName = "John Doe",
            Email = "john@example.com",
            Password = "password123"
        };

        _userRepoMock.Setup(r => r.ExistsByEmailAsync(command.Email)).ReturnsAsync(false);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var result = await _authService.RegisterAsync(command);

        Assert.NotNull(result.Token);
        Assert.Equal("John Doe", result.FullName);
        Assert.Equal("john@example.com", result.Email);
        Assert.Equal("User", result.Role);
    }

    [Fact(DisplayName = "❌ Duplicate registration — RegisterAsync throws Exception")]
    public async Task RegisterAsync_ShouldThrow_WhenUserExists()
    {
        var command = new RegisterCommand
        {
            FullName = "Existing User",
            Email = "exists@example.com",
            Password = "123"
        };

        _userRepoMock.Setup(r => r.ExistsByEmailAsync(command.Email)).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(command));
        Assert.Equal("User already exists", ex.Message);
    }

    [Fact(DisplayName = "✅ Valid login — LoginAsync returns token")]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreCorrect()
    {
        var plainPassword = "mypassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

        var user = new User
        {
            Id = 1,
            FullName = "Valid User",
            Email = "valid@example.com",
            PasswordHash = hashedPassword,
            Role = UserRole.User
        };

        var command = new LoginCommand
        {
            Email = "valid@example.com",
            Password = plainPassword
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync(user);

        var result = await _authService.LoginAsync(command);

        Assert.NotNull(result.Token);
        Assert.Equal("Valid User", result.FullName);
        Assert.Equal("valid@example.com", result.Email);
        Assert.Equal("User", result.Role);
    }

    [Fact(DisplayName = "❌ Wrong password — LoginAsync throws Exception")]
    public async Task LoginAsync_ShouldThrow_WhenPasswordIsWrong()
    {
        var user = new User
        {
            Id = 1,
            FullName = "Wrong Password User",
            Email = "wrong@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpass"),
            Role = UserRole.User
        };

        var command = new LoginCommand
        {
            Email = "wrong@example.com",
            Password = "wrongpass"
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync(user);

        var ex = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(command));
        Assert.Equal("Invalid credentials", ex.Message);
    }

    [Fact(DisplayName = "❌ User not found — LoginAsync throws Exception")]
    public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
    {
        var command = new LoginCommand
        {
            Email = "ghost@example.com",
            Password = "irrelevant"
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email)).ReturnsAsync((User)null);

        var ex = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(command));
        Assert.Equal("Invalid credentials", ex.Message);
    }
}
