using Application.DTOs.Anexos;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de anexos (arquivos de consulta e chat)
/// </summary>
public class AnexoService : IAnexoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AnexoService> _logger;
    private readonly string _basePath;

    public AnexoService(ApplicationDbContext context, ILogger<AnexoService> logger)
    {
        _context = context;
        _logger = logger;
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<AnexoDto?> ObterAnexoPorIdAsync(Guid id)
    {
        var anexo = await _context.Anexos
            .Include(a => a.EnviadoPor)
            .FirstOrDefaultAsync(a => a.Id == id);

        return anexo != null ? MapearParaDto(anexo) : null;
    }

    public async Task<List<AnexoDto>> ObterAnexosPorConsultaAsync(Guid consultaId)
    {
        var anexos = await _context.Anexos
            .Include(a => a.EnviadoPor)
            .Where(a => a.ConsultaId == consultaId)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();

        return anexos.Select(MapearParaDto).ToList();
    }

    public async Task<AnexoDto> SalvarAnexoAsync(Guid consultaId, Guid usuarioId, Stream arquivo, string nomeOriginal, string tipoMime)
    {
        // Verificar se consulta existe
        var consulta = await _context.Consultas.FindAsync(consultaId);
        if (consulta == null)
        {
            throw new InvalidOperationException("Consulta não encontrada.");
        }

        // Gerar nome único para o arquivo
        var extensao = Path.GetExtension(nomeOriginal);
        var nomeArquivo = $"{consultaId}_{Guid.NewGuid()}{extensao}";
        var caminhoCompleto = Path.Combine(_basePath, "consultas", consultaId.ToString(), nomeArquivo);

        // Criar diretório se não existir
        var diretorio = Path.GetDirectoryName(caminhoCompleto);
        if (!string.IsNullOrEmpty(diretorio) && !Directory.Exists(diretorio))
        {
            Directory.CreateDirectory(diretorio);
        }

        // Salvar arquivo
        using var memoryStream = new MemoryStream();
        await arquivo.CopyToAsync(memoryStream);
        var bytes = memoryStream.ToArray();
        await File.WriteAllBytesAsync(caminhoCompleto, bytes);

        // Criar registro no banco
        var anexo = new Anexo
        {
            ConsultaId = consultaId,
            EnviadoPorId = usuarioId,
            NomeOriginal = nomeOriginal,
            NomeArquivo = nomeArquivo,
            TipoMime = tipoMime,
            TamanhoBytes = bytes.Length,
            CaminhoArquivo = caminhoCompleto,
            CriadoEm = DateTime.UtcNow
        };

        _context.Anexos.Add(anexo);
        await _context.SaveChangesAsync();

        return MapearParaDto(anexo);
    }

    public async Task<bool> DeletarAnexoAsync(Guid id, Guid usuarioId)
    {
        var anexo = await _context.Anexos.FindAsync(id);
        if (anexo == null)
        {
            return false;
        }

        // Verificar se usuário pode deletar (enviou o anexo ou é admin)
        if (anexo.EnviadoPorId != usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario?.Tipo != Domain.Enums.TipoUsuario.Administrador)
            {
                throw new InvalidOperationException("Apenas quem enviou o anexo pode deletá-lo.");
            }
        }

        // Deletar arquivo físico
        if (!string.IsNullOrEmpty(anexo.CaminhoArquivo) && File.Exists(anexo.CaminhoArquivo))
        {
            File.Delete(anexo.CaminhoArquivo);
        }

        _context.Anexos.Remove(anexo);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<(Stream Arquivo, string TipoMime, string NomeOriginal)?> BaixarAnexoAsync(Guid id)
    {
        var anexo = await _context.Anexos.FindAsync(id);
        if (anexo == null || string.IsNullOrEmpty(anexo.CaminhoArquivo) || !File.Exists(anexo.CaminhoArquivo))
        {
            return null;
        }

        var stream = new FileStream(anexo.CaminhoArquivo, FileMode.Open, FileAccess.Read);
        return (stream, anexo.TipoMime, anexo.NomeOriginal);
    }

    // Métodos para anexos de chat
    public async Task<AnexoChatDto?> ObterAnexoChatPorIdAsync(Guid id)
    {
        var anexo = await _context.AnexosChat
            .Include(a => a.EnviadoPor)
            .FirstOrDefaultAsync(a => a.Id == id);

        return anexo != null ? MapearParaChatDto(anexo) : null;
    }

    public async Task<List<AnexoChatDto>> ObterAnexosChatPorConsultaAsync(Guid consultaId)
    {
        var anexos = await _context.AnexosChat
            .Include(a => a.EnviadoPor)
            .Where(a => a.ConsultaId == consultaId)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();

        return anexos.Select(MapearParaChatDto).ToList();
    }

    public async Task<AnexoChatDto> SalvarAnexoChatAsync(Guid consultaId, Guid usuarioId, Stream arquivo, string nomeOriginal, string tipoMime)
    {
        // Verificar se consulta existe
        var consulta = await _context.Consultas.FindAsync(consultaId);
        if (consulta == null)
        {
            throw new InvalidOperationException("Consulta não encontrada.");
        }

        // Gerar nome único para o arquivo
        var extensao = Path.GetExtension(nomeOriginal);
        var nomeArquivo = $"chat_{consultaId}_{Guid.NewGuid()}{extensao}";
        var caminhoCompleto = Path.Combine(_basePath, "chat", consultaId.ToString(), nomeArquivo);

        // Criar diretório se não existir
        var diretorio = Path.GetDirectoryName(caminhoCompleto);
        if (!string.IsNullOrEmpty(diretorio) && !Directory.Exists(diretorio))
        {
            Directory.CreateDirectory(diretorio);
        }

        // Salvar arquivo
        using var memoryStream = new MemoryStream();
        await arquivo.CopyToAsync(memoryStream);
        var bytes = memoryStream.ToArray();
        await File.WriteAllBytesAsync(caminhoCompleto, bytes);

        // Criar registro no banco
        var anexo = new AnexoChat
        {
            ConsultaId = consultaId,
            EnviadoPorId = usuarioId,
            NomeOriginal = nomeOriginal,
            NomeArquivo = nomeArquivo,
            TipoMime = tipoMime,
            TamanhoBytes = bytes.Length,
            CaminhoArquivo = caminhoCompleto,
            CriadoEm = DateTime.UtcNow
        };

        _context.AnexosChat.Add(anexo);
        await _context.SaveChangesAsync();

        return MapearParaChatDto(anexo);
    }

    public async Task<bool> DeletarAnexoChatAsync(Guid id, Guid usuarioId)
    {
        var anexo = await _context.AnexosChat.FindAsync(id);
        if (anexo == null)
        {
            return false;
        }

        // Verificar se usuário pode deletar
        if (anexo.EnviadoPorId != usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario?.Tipo != Domain.Enums.TipoUsuario.Administrador)
            {
                throw new InvalidOperationException("Apenas quem enviou o anexo pode deletá-lo.");
            }
        }

        // Deletar arquivo físico
        if (!string.IsNullOrEmpty(anexo.CaminhoArquivo) && File.Exists(anexo.CaminhoArquivo))
        {
            File.Delete(anexo.CaminhoArquivo);
        }

        _context.AnexosChat.Remove(anexo);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<(Stream Arquivo, string TipoMime, string NomeOriginal)?> BaixarAnexoChatAsync(Guid id)
    {
        var anexo = await _context.AnexosChat.FindAsync(id);
        if (anexo == null || string.IsNullOrEmpty(anexo.CaminhoArquivo) || !File.Exists(anexo.CaminhoArquivo))
        {
            return null;
        }

        var stream = new FileStream(anexo.CaminhoArquivo, FileMode.Open, FileAccess.Read);
        return (stream, anexo.TipoMime, anexo.NomeOriginal);
    }

    private static AnexoDto MapearParaDto(Anexo anexo)
    {
        return new AnexoDto
        {
            Id = anexo.Id,
            ConsultaId = anexo.ConsultaId,
            EnviadoPorId = anexo.EnviadoPorId,
            NomeEnviador = anexo.EnviadoPor != null ? $"{anexo.EnviadoPor.Nome} {anexo.EnviadoPor.Sobrenome}".Trim() : null,
            NomeOriginal = anexo.NomeOriginal,
            TipoMime = anexo.TipoMime,
            TamanhoBytes = anexo.TamanhoBytes,
            UrlDownload = $"/api/anexos/{anexo.Id}/download",
            CriadoEm = anexo.CriadoEm
        };
    }

    private static AnexoChatDto MapearParaChatDto(AnexoChat anexo)
    {
        return new AnexoChatDto
        {
            Id = anexo.Id,
            ConsultaId = anexo.ConsultaId,
            EnviadoPorId = anexo.EnviadoPorId,
            NomeEnviador = anexo.EnviadoPor != null ? $"{anexo.EnviadoPor.Nome} {anexo.EnviadoPor.Sobrenome}".Trim() : null,
            NomeOriginal = anexo.NomeOriginal,
            TipoMime = anexo.TipoMime,
            TamanhoBytes = anexo.TamanhoBytes,
            UrlDownload = $"/api/anexos/chat/{anexo.Id}/download",
            CriadoEm = anexo.CriadoEm
        };
    }
}
