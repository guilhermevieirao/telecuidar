namespace app.Domain.Entities;

public class FileUpload : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileCategory { get; set; } = string.Empty; // "Document", "Image", "Medical", "Other"
    
    // Relacionamento com usuário que fez upload
    public int UploadedByUserId { get; set; }
    public User UploadedByUser { get; set; } = null!;
    
    // Relacionamento opcional com usuário associado (ex: documento do paciente)
    public int? RelatedUserId { get; set; }
    public User? RelatedUser { get; set; }
    
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = false;
}
