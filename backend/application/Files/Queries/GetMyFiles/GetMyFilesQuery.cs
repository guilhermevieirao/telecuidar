using MediatR;
using app.Application.Common.Models;
using app.Application.Files.DTOs;

namespace app.Application.Files.Queries.GetMyFiles;

public class GetMyFilesQuery : IRequest<Result<PagedResult<FileUploadDto>>>
{
    public int UserId { get; set; }
    public string? FileCategory { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
