namespace app.Application.Files.DTOs;

public class FileUploadDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string FileCategory { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    
    public int UploadedByUserId { get; set; }
    public string UploadedByUserName { get; set; } = string.Empty;
    
    public int? RelatedUserId { get; set; }
    public string? RelatedUserName { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
}
