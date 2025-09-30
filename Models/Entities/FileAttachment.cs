namespace FertilizerWarehouseAPI.Models.Entities;

public class FileAttachment : BaseEntity
{
    public int CompanyId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
    public virtual User Uploader { get; set; } = null!;
}
