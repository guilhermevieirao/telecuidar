using Application.DTOs.Usuarios;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de usuários
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly ApplicationDbContext _context;
    private readonly IHashSenhaService _hashSenhaService;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(
        ApplicationDbContext context,
        IHashSenhaService hashSenhaService,
        ILogger<UsuarioService> logger)
    {
        _context = context;
        _hashSenhaService = hashSenhaService;
        _logger = logger;
    }

    public async Task<UsuariosPaginadosDto> ObterUsuariosAsync(int pagina, int tamanhoPagina, string? busca, string? tipo, string? status, Guid? especialidadeId = null)
    {
        var query = _context.Usuarios
            .Include(u => u.PerfilPaciente)
            .Include(u => u.PerfilProfissional)
                .ThenInclude(p => p!.Especialidade)
            .AsQueryable();

        if (!string.IsNullOrEmpty(busca))
        {
            busca = busca.ToLower();
            query = query.Where(u => 
                u.Nome.ToLower().Contains(busca) ||
                u.Sobrenome.ToLower().Contains(busca) ||
                u.Email.ToLower().Contains(busca) ||
                u.Cpf.Contains(busca));
        }

        if (!string.IsNullOrEmpty(tipo) && Enum.TryParse<TipoUsuario>(tipo, true, out var tipoEnum))
        {
            query = query.Where(u => u.Tipo == tipoEnum);
        }

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusUsuario>(status, true, out var statusEnum))
        {
            query = query.Where(u => u.Status == statusEnum);
        }

        if (especialidadeId.HasValue)
        {
            query = query.Where(u => u.PerfilProfissional != null && u.PerfilProfissional.EspecialidadeId == especialidadeId);
        }

        var total = await query.CountAsync();
        var usuarios = await query
            .OrderByDescending(u => u.CriadoEm)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new UsuariosPaginadosDto
        {
            Dados = usuarios.Select(MapearUsuarioParaDto).ToList(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalPaginas = (int)Math.Ceiling(total / (double)tamanhoPagina)
        };
    }

    public async Task<UsuarioDto?> ObterUsuarioPorIdAsync(Guid id)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.PerfilPaciente)
            .Include(u => u.PerfilProfissional)
                .ThenInclude(p => p!.Especialidade)
            .FirstOrDefaultAsync(u => u.Id == id);

        return usuario != null ? MapearUsuarioParaDto(usuario) : null;
    }

    public async Task<UsuarioDto?> ObterUsuarioPorEmailAsync(string email)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.PerfilPaciente)
            .Include(u => u.PerfilProfissional)
            .FirstOrDefaultAsync(u => u.Email == email);

        return usuario != null ? MapearUsuarioParaDto(usuario) : null;
    }

    public async Task<UsuarioDto?> ObterUsuarioPorCpfAsync(string cpf)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.PerfilPaciente)
            .Include(u => u.PerfilProfissional)
            .FirstOrDefaultAsync(u => u.Cpf == cpf);

        return usuario != null ? MapearUsuarioParaDto(usuario) : null;
    }

    public async Task<UsuarioDto?> ObterUsuarioPorTelefoneAsync(string telefone)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.PerfilPaciente)
            .Include(u => u.PerfilProfissional)
            .FirstOrDefaultAsync(u => u.Telefone == telefone);

        return usuario != null ? MapearUsuarioParaDto(usuario) : null;
    }

    public async Task<UsuarioDto> CriarUsuarioAsync(CriarUsuarioDto dto)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
        {
            throw new InvalidOperationException("E-mail já está em uso.");
        }

        if (await _context.Usuarios.AnyAsync(u => u.Cpf == dto.Cpf))
        {
            throw new InvalidOperationException("CPF já está em uso.");
        }

        var tipoUsuario = Enum.Parse<TipoUsuario>(dto.Tipo, true);

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Sobrenome = dto.Sobrenome,
            Email = dto.Email,
            Cpf = dto.Cpf,
            Telefone = dto.Telefone,
            SenhaHash = _hashSenhaService.GerarHash(dto.Senha),
            Tipo = tipoUsuario,
            Status = StatusUsuario.Ativo,
            EmailVerificado = true, // Admin criando usuário, já verificado
            CriadoEm = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);

        if (tipoUsuario == TipoUsuario.Paciente && dto.PerfilPaciente != null)
        {
            var perfil = new PerfilPaciente
            {
                UsuarioId = usuario.Id,
                Cns = dto.PerfilPaciente.Cns,
                NomeSocial = dto.PerfilPaciente.NomeSocial,
                Sexo = dto.PerfilPaciente.Sexo,
                DataNascimento = !string.IsNullOrEmpty(dto.PerfilPaciente.DataNascimento) ? DateTime.Parse(dto.PerfilPaciente.DataNascimento) : null,
                NomeMae = dto.PerfilPaciente.NomeMae,
                NomePai = dto.PerfilPaciente.NomePai,
                Nacionalidade = dto.PerfilPaciente.Nacionalidade,
                Cep = dto.PerfilPaciente.Cep,
                Endereco = dto.PerfilPaciente.Endereco,
                Cidade = dto.PerfilPaciente.Cidade,
                Estado = dto.PerfilPaciente.Estado,
                CriadoEm = DateTime.UtcNow
            };
            _context.PerfisPaciente.Add(perfil);
        }
        else if (tipoUsuario == TipoUsuario.Profissional && dto.PerfilProfissional != null)
        {
            var perfil = new PerfilProfissional
            {
                UsuarioId = usuario.Id,
                Crm = dto.PerfilProfissional.Crm,
                Cbo = dto.PerfilProfissional.Cbo,
                EspecialidadeId = dto.PerfilProfissional.EspecialidadeId,
                Sexo = dto.PerfilProfissional.Sexo,
                DataNascimento = !string.IsNullOrEmpty(dto.PerfilProfissional.DataNascimento) ? DateTime.Parse(dto.PerfilProfissional.DataNascimento) : null,
                Nacionalidade = dto.PerfilProfissional.Nacionalidade,
                Cep = dto.PerfilProfissional.Cep,
                Endereco = dto.PerfilProfissional.Endereco,
                Cidade = dto.PerfilProfissional.Cidade,
                Estado = dto.PerfilProfissional.Estado,
                CriadoEm = DateTime.UtcNow
            };
            _context.PerfisProfissional.Add(perfil);
        }

        await _context.SaveChangesAsync();

        return await ObterUsuarioPorIdAsync(usuario.Id) ?? throw new InvalidOperationException("Erro ao criar usuário.");
    }

    public async Task<UsuarioDto?> AtualizarUsuarioAsync(Guid id, AtualizarUsuarioDto dto)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.PerfilPaciente)
            .Include(u => u.PerfilProfissional)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(dto.Nome)) usuario.Nome = dto.Nome;
        if (!string.IsNullOrEmpty(dto.Sobrenome)) usuario.Sobrenome = dto.Sobrenome;
        if (!string.IsNullOrEmpty(dto.Telefone)) usuario.Telefone = dto.Telefone;
        if (!string.IsNullOrEmpty(dto.Status) && Enum.TryParse<StatusUsuario>(dto.Status, true, out var statusEnum))
        {
            usuario.Status = statusEnum;
        }
        usuario.AtualizadoEm = DateTime.UtcNow;

        // Atualizar perfil de paciente
        if (dto.PerfilPaciente != null && usuario.PerfilPaciente != null)
        {
            var perfil = usuario.PerfilPaciente;
            if (dto.PerfilPaciente.Cns != null) perfil.Cns = dto.PerfilPaciente.Cns;
            if (dto.PerfilPaciente.NomeSocial != null) perfil.NomeSocial = dto.PerfilPaciente.NomeSocial;
            if (dto.PerfilPaciente.Sexo != null) perfil.Sexo = dto.PerfilPaciente.Sexo;
            if (dto.PerfilPaciente.DataNascimento != null) perfil.DataNascimento = DateTime.Parse(dto.PerfilPaciente.DataNascimento);
            if (dto.PerfilPaciente.NomeMae != null) perfil.NomeMae = dto.PerfilPaciente.NomeMae;
            if (dto.PerfilPaciente.NomePai != null) perfil.NomePai = dto.PerfilPaciente.NomePai;
            if (dto.PerfilPaciente.Nacionalidade != null) perfil.Nacionalidade = dto.PerfilPaciente.Nacionalidade;
            if (dto.PerfilPaciente.Cep != null) perfil.Cep = dto.PerfilPaciente.Cep;
            if (dto.PerfilPaciente.Endereco != null) perfil.Endereco = dto.PerfilPaciente.Endereco;
            if (dto.PerfilPaciente.Cidade != null) perfil.Cidade = dto.PerfilPaciente.Cidade;
            if (dto.PerfilPaciente.Estado != null) perfil.Estado = dto.PerfilPaciente.Estado;
            perfil.AtualizadoEm = DateTime.UtcNow;
        }

        // Atualizar perfil de profissional
        if (dto.PerfilProfissional != null && usuario.PerfilProfissional != null)
        {
            var perfil = usuario.PerfilProfissional;
            if (dto.PerfilProfissional.Crm != null) perfil.Crm = dto.PerfilProfissional.Crm;
            if (dto.PerfilProfissional.Cbo != null) perfil.Cbo = dto.PerfilProfissional.Cbo;
            if (dto.PerfilProfissional.EspecialidadeId.HasValue) perfil.EspecialidadeId = dto.PerfilProfissional.EspecialidadeId;
            if (dto.PerfilProfissional.Sexo != null) perfil.Sexo = dto.PerfilProfissional.Sexo;
            if (dto.PerfilProfissional.DataNascimento != null) perfil.DataNascimento = DateTime.Parse(dto.PerfilProfissional.DataNascimento);
            if (dto.PerfilProfissional.Nacionalidade != null) perfil.Nacionalidade = dto.PerfilProfissional.Nacionalidade;
            if (dto.PerfilProfissional.Cep != null) perfil.Cep = dto.PerfilProfissional.Cep;
            if (dto.PerfilProfissional.Endereco != null) perfil.Endereco = dto.PerfilProfissional.Endereco;
            if (dto.PerfilProfissional.Cidade != null) perfil.Cidade = dto.PerfilProfissional.Cidade;
            if (dto.PerfilProfissional.Estado != null) perfil.Estado = dto.PerfilProfissional.Estado;
            perfil.AtualizadoEm = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return await ObterUsuarioPorIdAsync(id);
    }

    public async Task<bool> DeletarUsuarioAsync(Guid id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return false;
        }

        usuario.Status = StatusUsuario.Inativo;
        usuario.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PerfilPacienteDto?> ObterPerfilPacienteAsync(Guid usuarioId)
    {
        var perfil = await _context.PerfisPaciente.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        if (perfil == null) return null;

        return new PerfilPacienteDto
        {
            Id = perfil.Id,
            Cns = perfil.Cns,
            NomeSocial = perfil.NomeSocial,
            Sexo = perfil.Sexo,
            DataNascimento = perfil.DataNascimento?.ToString("yyyy-MM-dd"),
            NomeMae = perfil.NomeMae,
            NomePai = perfil.NomePai,
            Nacionalidade = perfil.Nacionalidade,
            Cep = perfil.Cep,
            Endereco = perfil.Endereco,
            Cidade = perfil.Cidade,
            Estado = perfil.Estado
        };
    }

    public async Task<PerfilProfissionalDto?> ObterPerfilProfissionalAsync(Guid usuarioId)
    {
        var perfil = await _context.PerfisProfissional
            .Include(p => p.Especialidade)
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        if (perfil == null) return null;

        return new PerfilProfissionalDto
        {
            Id = perfil.Id,
            Crm = perfil.Crm,
            Cbo = perfil.Cbo,
            EspecialidadeId = perfil.EspecialidadeId,
            NomeEspecialidade = perfil.Especialidade?.Nome,
            Sexo = perfil.Sexo,
            DataNascimento = perfil.DataNascimento?.ToString("yyyy-MM-dd"),
            Nacionalidade = perfil.Nacionalidade,
            Cep = perfil.Cep,
            Endereco = perfil.Endereco,
            Cidade = perfil.Cidade,
            Estado = perfil.Estado
        };
    }

    public async Task<PerfilPacienteDto> CriarOuAtualizarPerfilPacienteAsync(Guid usuarioId, CriarPerfilPacienteDto dto)
    {
        var perfil = await _context.PerfisPaciente.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        
        if (perfil == null)
        {
            perfil = new PerfilPaciente
            {
                UsuarioId = usuarioId,
                CriadoEm = DateTime.UtcNow
            };
            _context.PerfisPaciente.Add(perfil);
        }

        perfil.Cns = dto.Cns;
        perfil.NomeSocial = dto.NomeSocial;
        perfil.Sexo = dto.Sexo;
        perfil.DataNascimento = !string.IsNullOrEmpty(dto.DataNascimento) ? DateTime.Parse(dto.DataNascimento) : null;
        perfil.NomeMae = dto.NomeMae;
        perfil.NomePai = dto.NomePai;
        perfil.Nacionalidade = dto.Nacionalidade;
        perfil.Cep = dto.Cep;
        perfil.Endereco = dto.Endereco;
        perfil.Cidade = dto.Cidade;
        perfil.Estado = dto.Estado;
        perfil.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await ObterPerfilPacienteAsync(usuarioId))!;
    }

    public async Task<PerfilProfissionalDto> CriarOuAtualizarPerfilProfissionalAsync(Guid usuarioId, CriarPerfilProfissionalDto dto)
    {
        var perfil = await _context.PerfisProfissional.FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        
        if (perfil == null)
        {
            perfil = new PerfilProfissional
            {
                UsuarioId = usuarioId,
                CriadoEm = DateTime.UtcNow
            };
            _context.PerfisProfissional.Add(perfil);
        }

        perfil.Crm = dto.Crm;
        perfil.Cbo = dto.Cbo;
        perfil.EspecialidadeId = dto.EspecialidadeId;
        perfil.Sexo = dto.Sexo;
        perfil.DataNascimento = !string.IsNullOrEmpty(dto.DataNascimento) ? DateTime.Parse(dto.DataNascimento) : null;
        perfil.Nacionalidade = dto.Nacionalidade;
        perfil.Cep = dto.Cep;
        perfil.Endereco = dto.Endereco;
        perfil.Cidade = dto.Cidade;
        perfil.Estado = dto.Estado;
        perfil.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (await ObterPerfilProfissionalAsync(usuarioId))!;
    }

    public async Task<string?> AtualizarAvatarAsync(Guid usuarioId, string avatarBase64)
    {
        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null) return null;

        // Salvar avatar como base64 ou processar para salvar em arquivo
        usuario.Avatar = avatarBase64;
        usuario.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return usuario.Avatar;
    }

    public async Task<bool> RemoverAvatarAsync(Guid usuarioId)
    {
        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null) return false;

        usuario.Avatar = null;
        usuario.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    private static UsuarioDto MapearUsuarioParaDto(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Nome = usuario.Nome,
            Sobrenome = usuario.Sobrenome,
            Cpf = usuario.Cpf,
            Telefone = usuario.Telefone,
            Avatar = usuario.Avatar,
            Tipo = usuario.Tipo.ToString(),
            Status = usuario.Status.ToString(),
            EmailVerificado = usuario.EmailVerificado,
            CriadoEm = usuario.CriadoEm,
            AtualizadoEm = usuario.AtualizadoEm,
            PerfilPaciente = usuario.PerfilPaciente != null ? new PerfilPacienteDto
            {
                Id = usuario.PerfilPaciente.Id,
                Cns = usuario.PerfilPaciente.Cns,
                NomeSocial = usuario.PerfilPaciente.NomeSocial,
                Sexo = usuario.PerfilPaciente.Sexo,
                DataNascimento = usuario.PerfilPaciente.DataNascimento?.ToString("yyyy-MM-dd"),
                NomeMae = usuario.PerfilPaciente.NomeMae,
                NomePai = usuario.PerfilPaciente.NomePai,
                Nacionalidade = usuario.PerfilPaciente.Nacionalidade,
                Cep = usuario.PerfilPaciente.Cep,
                Endereco = usuario.PerfilPaciente.Endereco,
                Cidade = usuario.PerfilPaciente.Cidade,
                Estado = usuario.PerfilPaciente.Estado
            } : null,
            PerfilProfissional = usuario.PerfilProfissional != null ? new PerfilProfissionalDto
            {
                Id = usuario.PerfilProfissional.Id,
                Crm = usuario.PerfilProfissional.Crm,
                Cbo = usuario.PerfilProfissional.Cbo,
                EspecialidadeId = usuario.PerfilProfissional.EspecialidadeId,
                NomeEspecialidade = usuario.PerfilProfissional.Especialidade?.Nome,
                Sexo = usuario.PerfilProfissional.Sexo,
                DataNascimento = usuario.PerfilProfissional.DataNascimento?.ToString("yyyy-MM-dd"),
                Nacionalidade = usuario.PerfilProfissional.Nacionalidade,
                Cep = usuario.PerfilProfissional.Cep,
                Endereco = usuario.PerfilProfissional.Endereco,
                Cidade = usuario.PerfilProfissional.Cidade,
                Estado = usuario.PerfilProfissional.Estado
            } : null
        };
    }
}
