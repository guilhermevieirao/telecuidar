using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Reports.DTOs;
using app.Domain.Interfaces;
using app.Domain.Entities;

namespace app.Application.Reports.Queries.GetFilesReport;

public class GetFilesReportQueryHandler : IRequestHandler<GetFilesReportQuery, FilesReportDto>
{
    private readonly IRepository<FileUpload> _fileRepository;

    public GetFilesReportQueryHandler(IRepository<FileUpload> fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<FilesReportDto> Handle(GetFilesReportQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.Now.AddMonths(-1);
        var endDate = request.EndDate ?? DateTime.Now;

        var query = _fileRepository.GetQueryable()
            .Where(f => f.CreatedAt >= startDate && f.CreatedAt <= endDate)
            .Include(f => f.UploadedByUser);

        var files = await query.ToListAsync(cancellationToken);

        var totalSize = files.Sum(f => f.FileSize);

        var report = new FilesReportDto
        {
            TotalFiles = files.Count,
            TotalSizeBytes = totalSize,
            TotalSizeFormatted = FormatFileSize(totalSize),
            StartDate = startDate,
            EndDate = endDate,
            GeneratedAt = DateTime.Now,
            CategoryCounts = files.GroupBy(f => f.FileCategory)
                .ToDictionary(g => g.Key, g => g.Count()),
            UserUploadCounts = files.GroupBy(f => f.UploadedByUser!.FullName)
                .ToDictionary(g => g.Key, g => g.Count()),
            FileDetails = files.Select(f => new FileDetailDto
            {
                Id = f.Id,
                OriginalFileName = f.OriginalFileName,
                FileCategory = f.FileCategory,
                FileSizeBytes = f.FileSize,
                FileSizeFormatted = FormatFileSize(f.FileSize),
                UploadedByUserName = f.UploadedByUser?.FullName ?? "Desconhecido",
                CreatedAt = f.CreatedAt
            }).OrderByDescending(f => f.CreatedAt).ToList()
        };

        return report;
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
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
