using Application.DTOs.Jitsi;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de integração com Jitsi Meet
/// </summary>
public class JitsiService : IJitsiService
{
    private readonly IConfiguration _configuration;
    private readonly string _domain;
    private readonly string _appId;
    private readonly string _appSecret;

    public JitsiService(IConfiguration configuration)
    {
        _configuration = configuration;
        _domain = _configuration["Jitsi:Domain"] ?? "meet.telecuidar.local";
        _appId = _configuration["Jitsi:AppId"] ?? "telecuidar";
        _appSecret = _configuration["Jitsi:AppSecret"] ?? "seu-app-secret-aqui";
    }

    public Task<SalaJitsiDto> CriarSalaAsync(CriarSalaJitsiDto dto)
    {
        var roomName = $"{dto.ConsultaId}_{Guid.NewGuid():N}";
        
        var sala = new SalaJitsiDto
        {
            NomeSala = roomName,
            UrlCompleta = $"https://{_domain}/{roomName}",
            Domain = _domain,
            ConsultaId = dto.ConsultaId
        };

        return Task.FromResult(sala);
    }

    public Task<TokenJitsiDto> GerarTokenAsync(GerarTokenJitsiDto dto)
    {
        var now = DateTime.UtcNow;
        var exp = now.AddHours(2); // Token válido por 2 horas

        var claims = new List<Claim>
        {
            new("iss", _appId),
            new("sub", _domain),
            new("aud", "jitsi"),
            new("room", dto.NomeSala),
            new("iat", ((DateTimeOffset)now).ToUnixTimeSeconds().ToString()),
            new("exp", ((DateTimeOffset)exp).ToUnixTimeSeconds().ToString()),
            new("nbf", ((DateTimeOffset)now).ToUnixTimeSeconds().ToString())
        };

        // Adicionar contexto do usuário
        var context = new JitsiUserContext
        {
            User = new JitsiUserInfo
            {
                Id = dto.UsuarioId.ToString(),
                Name = dto.NomeUsuario,
                Email = dto.EmailUsuario,
                Avatar = dto.AvatarUrl,
                Moderator = dto.Moderador
            }
        };

        claims.Add(new Claim("context", System.Text.Json.JsonSerializer.Serialize(context)));

        // Adicionar features se especificado
        if (dto.Features != null && dto.Features.Any())
        {
            claims.Add(new Claim("features", System.Text.Json.JsonSerializer.Serialize(dto.Features)));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(new TokenJitsiDto
        {
            Token = tokenString,
            ExpiraEm = exp,
            NomeSala = dto.NomeSala,
            UrlCompleta = $"https://{_domain}/{dto.NomeSala}?jwt={tokenString}"
        });
    }

    public async Task<TokenJitsiDto> GerarTokenModeradoAsync(string nomeSala, Guid usuarioId, string nomeUsuario, string emailUsuario)
    {
        return await GerarTokenAsync(new GerarTokenJitsiDto
        {
            NomeSala = nomeSala,
            UsuarioId = usuarioId,
            NomeUsuario = nomeUsuario,
            EmailUsuario = emailUsuario,
            Moderador = true,
            Features = new Dictionary<string, bool>
            {
                ["lobby"] = true,
                ["recording"] = true,
                ["livestreaming"] = false,
                ["transcription"] = false,
                ["outbound-call"] = false
            }
        });
    }

    public async Task<TokenJitsiDto> GerarTokenParticipanteAsync(string nomeSala, Guid usuarioId, string nomeUsuario, string emailUsuario)
    {
        return await GerarTokenAsync(new GerarTokenJitsiDto
        {
            NomeSala = nomeSala,
            UsuarioId = usuarioId,
            NomeUsuario = nomeUsuario,
            EmailUsuario = emailUsuario,
            Moderador = false
        });
    }

    public ConfiguracaoJitsiDto ObterConfiguracao()
    {
        return new ConfiguracaoJitsiDto
        {
            Domain = _domain,
            InterfaceConfigOverwrite = new Dictionary<string, object>
            {
                ["SHOW_JITSI_WATERMARK"] = false,
                ["SHOW_WATERMARK_FOR_GUESTS"] = false,
                ["DEFAULT_BACKGROUND"] = "#474location-pin757",
                ["DISABLE_JOIN_LEAVE_NOTIFICATIONS"] = true,
                ["MOBILE_APP_PROMO"] = false,
                ["HIDE_INVITE_MORE_HEADER"] = true,
                ["TOOLBAR_BUTTONS"] = new[]
                {
                    "camera", "chat", "desktop", "fullscreen",
                    "hangup", "microphone", "participants-pane",
                    "settings", "tileview", "toggle-camera",
                    "videoquality", "filmstrip"
                }
            },
            ConfigOverwrite = new Dictionary<string, object>
            {
                ["startWithAudioMuted"] = true,
                ["startWithVideoMuted"] = false,
                ["enableWelcomePage"] = false,
                ["prejoinPageEnabled"] = true,
                ["disableDeepLinking"] = true,
                ["enableClosePage"] = false,
                ["disableInviteFunctions"] = true,
                ["requireDisplayName"] = true,
                ["enableEmailInStats"] = false
            }
        };
    }

    private class JitsiUserContext
    {
        public JitsiUserInfo? User { get; set; }
    }

    private class JitsiUserInfo
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public bool Moderator { get; set; }
    }
}
