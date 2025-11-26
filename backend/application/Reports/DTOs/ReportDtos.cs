namespace app.Application.Reports.DTOs;

public class ReportFilterDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? ReportType { get; set; }
    public int? UserId { get; set; }
    public string? EntityType { get; set; }
}

public class UsersReportDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int EmailConfirmedCount { get; set; }
    public int PacientesCount { get; set; }
    public int ProfissionaisCount { get; set; }
    public int AdministradoresCount { get; set; }
    public DateTime GeneratedAt { get; set; }
    public List<UserDetailDto> UserDetails { get; set; } = new();
}

public class UserDetailDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class AuditLogsReportDto
{
    public int TotalLogs { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public Dictionary<string, int> ActionCounts { get; set; } = new();
    public Dictionary<string, int> EntityCounts { get; set; } = new();
    public List<AuditLogDetailDto> LogDetails { get; set; } = new();
}

public class AuditLogDetailDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FilesReportDto
{
    public int TotalFiles { get; set; }
    public long TotalSizeBytes { get; set; }
    public string TotalSizeFormatted { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public Dictionary<string, int> CategoryCounts { get; set; } = new();
    public Dictionary<string, int> UserUploadCounts { get; set; } = new();
    public List<FileDetailDto> FileDetails { get; set; } = new();
}

public class FileDetailDto
{
    public int Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileCategory { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
    public string UploadedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class NotificationsReportDto
{
    public int TotalNotifications { get; set; }
    public int ReadNotifications { get; set; }
    public int UnreadNotifications { get; set; }
    public double ReadPercentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime GeneratedAt { get; set; }
    public Dictionary<string, int> TypeCounts { get; set; } = new();
    public List<NotificationDetailDto> NotificationDetails { get; set; } = new();
}

public class NotificationDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
