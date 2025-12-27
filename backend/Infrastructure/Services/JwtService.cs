using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

/// <summary>
/// Serviço para geração e validação de tokens JWT
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;
    
    // Cache simples em memória para refresh tokens (em produção usar Redis ou banco)
    private static readonly Dictionary<Guid, (string Token, DateTime Expiry)> _refreshTokens = new();
    private static readonly object _lock = new();

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") 
            ?? _configuration["JwtSettings:SecretKey"] 
            ?? throw new InvalidOperationException("JWT Secret não configurado");
            
        _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") 
            ?? _configuration["JwtSettings:Issuer"] 
            ?? "TeleCuidar";
            
        _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") 
            ?? _configuration["JwtSettings:Audience"] 
            ?? "TeleCuidarApp";

        var expirationStr = Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES") 
            ?? _configuration["JwtSettings:ExpirationMinutes"] 
            ?? "60";
        _expirationMinutes = int.Parse(expirationStr);
    }

    public string GerarAccessToken(Guid usuarioId, string email, string nome, string tipo)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, nome),
            new Claim(ClaimTypes.Role, tipo),
            new Claim("tipo", tipo),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GerarRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public (bool Valido, Guid? UsuarioId, string? Email, string? Tipo) ValidarToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value 
                ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value 
                ?? principal.FindFirst(ClaimTypes.Email)?.Value;
            var tipo = principal.FindFirst("tipo")?.Value 
                ?? principal.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var guidUserId))
            {
                return (false, null, null, null);
            }

            return (true, guidUserId, email, tipo);
        }
        catch
        {
            return (false, null, null, null);
        }
    }

    public bool ValidarRefreshToken(string refreshToken, Guid usuarioId)
    {
        lock (_lock)
        {
            if (_refreshTokens.TryGetValue(usuarioId, out var stored))
            {
                return stored.Token == refreshToken && stored.Expiry > DateTime.UtcNow;
            }
            return false;
        }
    }

    public Task SalvarRefreshTokenAsync(Guid usuarioId, string refreshToken, TimeSpan expiracao)
    {
        lock (_lock)
        {
            _refreshTokens[usuarioId] = (refreshToken, DateTime.UtcNow.Add(expiracao));
        }
        return Task.CompletedTask;
    }

    public Task RevogarRefreshTokenAsync(Guid usuarioId)
    {
        lock (_lock)
        {
            _refreshTokens.Remove(usuarioId);
        }
        return Task.CompletedTask;
    }

    public Task<bool> RefreshTokenExisteAsync(Guid usuarioId, string refreshToken)
    {
        lock (_lock)
        {
            if (_refreshTokens.TryGetValue(usuarioId, out var stored))
            {
                return Task.FromResult(stored.Token == refreshToken && stored.Expiry > DateTime.UtcNow);
            }
            return Task.FromResult(false);
        }
    }
}
