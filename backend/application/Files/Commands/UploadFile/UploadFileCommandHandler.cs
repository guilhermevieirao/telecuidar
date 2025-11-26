using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;
using System.Linq;

namespace app.Application.Files.Commands.UploadFile;

public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, Result<int>>
{
    private readonly IRepository<FileUpload> _fileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private static readonly string[] AllowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".txt", ".xlsx", ".xls" };

    public UploadFileCommandHandler(
        IRepository<FileUpload> fileRepository,
        IUnitOfWork unitOfWork)
    {
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validações
            if (request.File == null || request.File.Length == 0)
            {
                return Result<int>.Failure("Arquivo não fornecido ou vazio");
            }

            if (request.File.Length > MaxFileSize)
            {
                return Result<int>.Failure($"Arquivo muito grande. Tamanho máximo: {MaxFileSize / 1024 / 1024}MB");
            }

            var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
            if (Array.IndexOf(AllowedExtensions, extension) == -1)
            {
                return Result<int>.Failure($"Tipo de arquivo não permitido. Permitidos: {string.Join(", ", AllowedExtensions)}");
            }

            // Criar diretório de uploads se não existir
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Gerar nome único para o arquivo
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Salvar arquivo no disco
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream, cancellationToken);
            }

            // Criar registro no banco
            var fileUpload = new FileUpload
            {
                FileName = uniqueFileName,
                OriginalFileName = request.File.FileName,
                ContentType = request.File.ContentType,
                FileSize = request.File.Length,
                FilePath = filePath,
                FileCategory = request.FileCategory,
                Description = request.Description,
                IsPublic = request.IsPublic,
                UploadedByUserId = request.UploadedByUserId,
                RelatedUserId = request.RelatedUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _fileRepository.AddAsync(fileUpload);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(fileUpload.Id, "Arquivo enviado com sucesso");
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Erro ao fazer upload do arquivo: {ex.Message}");
        }
    }
}
