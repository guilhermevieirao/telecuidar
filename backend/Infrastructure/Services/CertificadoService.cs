using Application.DTOs.Certificados;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Infrastructure.Services;

/// <summary>
/// Serviço de armazenamento de certificados digitais
/// </summary>
public class CertificadoService : ICertificadoService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CertificadoService> _logger;
    private readonly string _storagePath;

    public CertificadoService(ApplicationDbContext context, ILogger<CertificadoService> logger)
    {
        _context = context;
        _logger = logger;
        _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "certificates");
        
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<List<CertificadoDto>> ObterCertificadosPorUsuarioAsync(Guid usuarioId)
    {
        var certificados = await _context.CertificadosSalvos
            .Where(c => c.UsuarioId == usuarioId)
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync();

        return certificados.Select(MapearParaDto).ToList();
    }

    public async Task<CertificadoDto?> ObterCertificadoPorIdAsync(Guid id)
    {
        var certificado = await _context.CertificadosSalvos.FindAsync(id);
        return certificado != null ? MapearParaDto(certificado) : null;
    }

    public async Task<CertificadoDto> SalvarCertificadoAsync(Guid usuarioId, Stream arquivoCertificado, string senha, string apelido)
    {
        try
        {
            // Ler o arquivo em memória
            using var memoryStream = new MemoryStream();
            await arquivoCertificado.CopyToAsync(memoryStream);
            var certificadoBytes = memoryStream.ToArray();

            // Carregar e validar o certificado
            var x509 = X509CertificateLoader.LoadPkcs12(certificadoBytes, senha, X509KeyStorageFlags.Exportable);

            // Verificar validade
            if (x509.NotAfter <= DateTime.UtcNow)
            {
                throw new InvalidOperationException("O certificado está expirado.");
            }

            // Extrair informações
            var nomeProprietario = x509.GetNameInfo(X509NameType.SimpleName, false);
            var cpfCnpj = ExtrairCpfCnpj(x509);

            // Encriptar o certificado para armazenamento seguro
            var certificadoEncriptado = CriptografarCertificado(certificadoBytes, senha);

            // Gerar nome do arquivo
            var nomeArquivo = $"{usuarioId}_{Guid.NewGuid()}.p12enc";
            var caminhoArquivo = Path.Combine(_storagePath, nomeArquivo);

            // Salvar arquivo encriptado
            await File.WriteAllBytesAsync(caminhoArquivo, certificadoEncriptado);

            // Salvar registro no banco
            var certificado = new CertificadoSalvo
            {
                UsuarioId = usuarioId,
                Apelido = apelido,
                NomeProprietario = nomeProprietario,
                CpfCnpj = cpfCnpj,
                DataValidade = x509.NotAfter,
                CaminhoArquivo = caminhoArquivo,
                Thumbprint = x509.Thumbprint,
                HashSenha = HashSenha(senha),
                CriadoEm = DateTime.UtcNow
            };

            _context.CertificadosSalvos.Add(certificado);
            await _context.SaveChangesAsync();

            return MapearParaDto(certificado);
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex, "Erro ao processar certificado digital");
            throw new InvalidOperationException("Não foi possível processar o certificado. Verifique a senha informada.", ex);
        }
    }

    public async Task<bool> DeletarCertificadoAsync(Guid id, Guid usuarioId)
    {
        var certificado = await _context.CertificadosSalvos
            .FirstOrDefaultAsync(c => c.Id == id && c.UsuarioId == usuarioId);

        if (certificado == null)
        {
            return false;
        }

        // Remover arquivo físico
        if (!string.IsNullOrEmpty(certificado.CaminhoArquivo) && File.Exists(certificado.CaminhoArquivo))
        {
            File.Delete(certificado.CaminhoArquivo);
        }

        _context.CertificadosSalvos.Remove(certificado);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ValidarCertificadoAsync(Guid id, string senha)
    {
        var certificado = await _context.CertificadosSalvos.FindAsync(id);
        if (certificado == null)
        {
            return false;
        }

        // Verificar validade
        if (certificado.DataValidade <= DateTime.UtcNow)
        {
            return false;
        }

        // Verificar senha
        return VerificarSenha(senha, certificado.HashSenha ?? string.Empty);
    }

    public async Task<byte[]?> AssinarDocumentoAsync(Guid certificadoId, string senha, byte[] documento)
    {
        var certificado = await _context.CertificadosSalvos.FindAsync(certificadoId);
        if (certificado == null || string.IsNullOrEmpty(certificado.CaminhoArquivo))
        {
            return null;
        }

        // Verificar validade
        if (certificado.DataValidade <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("O certificado está expirado.");
        }

        // Verificar senha
        if (!VerificarSenha(senha, certificado.HashSenha ?? string.Empty))
        {
            throw new InvalidOperationException("Senha do certificado inválida.");
        }

        try
        {
            // Ler e decriptar certificado
            var certificadoEncriptado = await File.ReadAllBytesAsync(certificado.CaminhoArquivo);
            var certificadoBytes = DecriptografarCertificado(certificadoEncriptado, senha);

            var x509 = X509CertificateLoader.LoadPkcs12(certificadoBytes, senha, X509KeyStorageFlags.Exportable);

            // Assinar documento
            using var rsa = x509.GetRSAPrivateKey();
            if (rsa == null)
            {
                throw new InvalidOperationException("O certificado não possui chave privada RSA.");
            }

            return rsa.SignData(documento, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex, "Erro ao assinar documento");
            throw new InvalidOperationException("Erro ao assinar documento com o certificado.", ex);
        }
    }

    private static byte[] CriptografarCertificado(byte[] certificado, string senha)
    {
        // Em produção, usar chave segura armazenada em Key Vault
        using var aes = Aes.Create();
        aes.GenerateKey();
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var encrypted = encryptor.TransformFinalBlock(certificado, 0, certificado.Length);

        // Retornar IV + Key + Encrypted data
        var result = new byte[aes.IV.Length + aes.Key.Length + encrypted.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(aes.Key, 0, result, aes.IV.Length, aes.Key.Length);
        Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length + aes.Key.Length, encrypted.Length);

        return result;
    }

    private static byte[] DecriptografarCertificado(byte[] encryptedData, string senha)
    {
        using var aes = Aes.Create();
        
        // Extrair IV e Key
        var iv = new byte[16];
        var key = new byte[32];
        Buffer.BlockCopy(encryptedData, 0, iv, 0, 16);
        Buffer.BlockCopy(encryptedData, 16, key, 0, 32);

        aes.IV = iv;
        aes.Key = key;

        using var decryptor = aes.CreateDecryptor();
        var encrypted = new byte[encryptedData.Length - 48];
        Buffer.BlockCopy(encryptedData, 48, encrypted, 0, encrypted.Length);

        return decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
    }

    private static string? ExtrairCpfCnpj(X509Certificate2 cert)
    {
        // Tentar extrair CPF/CNPJ do Subject
        var subject = cert.Subject;
        
        // Padrão ICP-Brasil
        var match = System.Text.RegularExpressions.Regex.Match(subject, @"(?:CPF:|2\.16\.76\.1\.3\.1=)(\d{11})|(?:CNPJ:|2\.16\.76\.1\.3\.3=)(\d{14})");
        if (match.Success)
        {
            return match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
        }

        return null;
    }

    private static string HashSenha(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha);
    }

    private static bool VerificarSenha(string senha, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(senha, hash);
    }

    private static CertificadoDto MapearParaDto(CertificadoSalvo certificado)
    {
        return new CertificadoDto
        {
            Id = certificado.Id,
            Apelido = certificado.Apelido,
            NomeProprietario = certificado.NomeProprietario,
            CpfCnpj = certificado.CpfCnpj,
            DataValidade = certificado.DataValidade,
            Thumbprint = certificado.Thumbprint,
            Valido = certificado.DataValidade > DateTime.UtcNow,
            CriadoEm = certificado.CriadoEm
        };
    }
}
