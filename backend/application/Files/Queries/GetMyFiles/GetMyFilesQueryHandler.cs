using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Models;
using app.Application.Files.DTOs;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.Files.Queries.GetMyFiles;

public class GetMyFilesQueryHandler : IRequestHandler<GetMyFilesQuery, Result<PagedResult<FileUploadDto>>>
{
    private readonly IRepository<FileUpload> _fileRepository;

    public GetMyFilesQueryHandler(IRepository<FileUpload> fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<Result<PagedResult<FileUploadDto>>> Handle(GetMyFilesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _fileRepository.GetQueryable()
                .Include(f => f.UploadedByUser)
                .Include(f => f.RelatedUser)
                .Where(f => f.UploadedByUserId == request.UserId || f.RelatedUserId == request.UserId);

            // Filtrar por categoria se especificado
            if (!string.IsNullOrWhiteSpace(request.FileCategory))
            {
                query = query.Where(f => f.FileCategory == request.FileCategory);
            }

            // Ordenar por data de criação (mais recentes primeiro)
            query = query.OrderByDescending(f => f.CreatedAt);

            // Obter contagem total
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplicar paginação
            var files = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var fileDtos = files.Select(f => new FileUploadDto
            {
                Id = f.Id,
                FileName = f.FileName,
                OriginalFileName = f.OriginalFileName,
                ContentType = f.ContentType,
                FileSize = f.FileSize,
                FileSizeFormatted = FormatFileSize(f.FileSize),
                FileCategory = f.FileCategory,
                Description = f.Description,
                IsPublic = f.IsPublic,
                UploadedByUserId = f.UploadedByUserId,
                UploadedByUserName = f.UploadedByUser.FullName,
                RelatedUserId = f.RelatedUserId,
                RelatedUserName = f.RelatedUser?.FullName,
                CreatedAt = f.CreatedAt,
                DownloadUrl = $"/api/files/{f.Id}/download"
            }).ToList();

            var pagedResult = PagedResult<FileUploadDto>.Create(fileDtos, totalCount, request.PageNumber, request.PageSize);

            return Result<PagedResult<FileUploadDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<FileUploadDto>>.Failure($"Erro ao buscar arquivos: {ex.Message}");
        }
    }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
