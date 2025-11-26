using MediatR;
using Microsoft.AspNetCore.Http;
using app.Application.Common.Models;

namespace app.Application.Files.Commands.UploadFile;

public class UploadFileCommand : IRequest<Result<int>>
{
    public IFormFile File { get; set; } = null!;
    public string FileCategory { get; set; } = string.Empty; // "Document", "Image", "Medical", "Other"
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
    public int? RelatedUserId { get; set; }
    public int UploadedByUserId { get; set; }
}
