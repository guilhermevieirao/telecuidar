using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Services;

public class AuthServiceTests
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;

    public AuthServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        // Setup configuration
        var inMemorySettings = new Dictionary<string, string>
        {
            {"JwtSettings:SecretKey", "TelecuidarSecretKey2024SuperSecureKey!@#$%"},
            {"JwtSettings:Issuer", "TelecuidarAPI"},
            {"JwtSettings:Audience", "TelecuidarClient"},
            {"JwtSettings:ExpirationMinutes", "60"},
            {"JwtSettings:RefreshTokenExpirationDays", "7"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _jwtService = new JwtService(_configuration);
        _passwordHasher = new PasswordHasher();
        _authService = new AuthService(_context, _jwtService, _passwordHasher, _configuration);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateNewUser_WhenDataIsValid()
    {
        // Arrange
        var name = "João";
        var lastName = "Silva";
        var email = "joao@test.com";
        var cpf = "12345678901";
        var phone = "11999999999";
        var password = "Password123!";

        // Act
        var user = await _authService.RegisterAsync(name, lastName, email, cpf, phone, password);

        // Assert
        user.Should().NotBeNull();
        user.Email.Should().Be(email);
        user.Name.Should().Be(name);
        user.LastName.Should().Be(lastName);
        user.Cpf.Should().Be(cpf);
        user.Role.Should().Be(UserRole.PATIENT);
        user.EmailVerified.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var email = "duplicate@test.com";
        await _authService.RegisterAsync("Test", "User", email, "11111111111", null, "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _authService.RegisterAsync("Another", "User", email, "22222222222", null, "Password123!"));
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenCpfAlreadyExists()
    {
        // Arrange
        var cpf = "12345678901";
        await _authService.RegisterAsync("Test", "User", "test1@test.com", cpf, null, "Password123!");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _authService.RegisterAsync("Another", "User", "test2@test.com", cpf, null, "Password123!"));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnUserAndTokens_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "login@test.com";
        var password = "Password123!";
        await _authService.RegisterAsync("Test", "User", email, "33333333333", null, password);

        // Act
        var (user, accessToken, refreshToken) = await _authService.LoginAsync(email, password, false);

        // Assert
        user.Should().NotBeNull();
        user!.Email.Should().Be(email);
        accessToken.Should().NotBeNullOrEmpty();
        refreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsInvalid()
    {
        // Arrange
        var email = "wrongpass@test.com";
        await _authService.RegisterAsync("Test", "User", email, "44444444444", null, "Password123!");

        // Act
        var (user, _, _) = await _authService.LoginAsync(email, "WrongPassword", false);

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenEmailDoesNotExist()
    {
        // Act
        var (user, _, _) = await _authService.LoginAsync("nonexistent@test.com", "Password123!", false);

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        // Arrange
        var email = "refresh@test.com";
        var password = "Password123!";
        await _authService.RegisterAsync("Test", "User", email, "55555555555", null, password);
        var (_, _, refreshToken) = await _authService.LoginAsync(email, password, false);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        result.Should().NotBeNull();
        result!.Value.AccessToken.Should().NotBeNullOrEmpty();
        result.Value.RefreshToken.Should().NotBeNullOrEmpty();
        result.Value.RefreshToken.Should().NotBe(refreshToken); // Should be a new token
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsInvalid()
    {
        // Act
        var result = await _authService.RefreshTokenAsync("invalid-token");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldChangePassword_WhenTokenIsValid()
    {
        // Arrange
        var email = "reset@test.com";
        var oldPassword = "OldPassword123!";
        var newPassword = "NewPassword123!";
        var user = await _authService.RegisterAsync("Test", "User", email, "66666666666", null, oldPassword);
        await _authService.ForgotPasswordAsync(email);
        
        var userInDb = await _context.Users.FirstAsync(u => u.Email == email);
        var resetToken = userInDb.PasswordResetToken!;

        // Act
        var result = await _authService.ResetPasswordAsync(resetToken, newPassword);

        // Assert
        result.Should().BeTrue();
        
        // Verify can login with new password
        var (loginUser, _, _) = await _authService.LoginAsync(email, newPassword, false);
        loginUser.Should().NotBeNull();
    }

    [Fact]
    public async Task VerifyEmailAsync_ShouldSetEmailVerifiedToTrue_WhenTokenIsValid()
    {
        // Arrange
        var email = "verify@test.com";
        await _authService.RegisterAsync("Test", "User", email, "77777777777", null, "Password123!");
        var user = await _context.Users.FirstAsync(u => u.Email == email);
        var verificationToken = user.EmailVerificationToken!;

        // Act
        var result = await _authService.VerifyEmailAsync(verificationToken);

        // Assert
        result.Should().BeTrue();
        var verifiedUser = await _context.Users.FirstAsync(u => u.Email == email);
        verifiedUser.EmailVerified.Should().BeTrue();
    }
}
