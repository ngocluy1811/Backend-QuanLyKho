using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class FileAttachmentDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string OriginalName { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;
        [StringLength(100)]
        public string? MimeType { get; set; }
        [Range(0, long.MaxValue)]
        public long FileSize { get; set; }
        [StringLength(100)]
        public string? EntityType { get; set; } // Product, Order, Task, etc.
        public int? EntityId { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? Category { get; set; } // Document, Image, Certificate, etc.
        public bool IsPublic { get; set; } = false;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UploadedBy { get; set; }
        public UserDto? UploadedByUser { get; set; }
    }

    public class CreateFileAttachmentDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string OriginalName { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;
        [StringLength(100)]
        public string? MimeType { get; set; }
        [Range(0, long.MaxValue)]
        public long FileSize { get; set; }
        [StringLength(100)]
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? Category { get; set; }
        public bool IsPublic { get; set; } = false;
    }

    public class UpdateFileAttachmentDto
    {
        [StringLength(255)]
        public string? OriginalName { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? Category { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsActive { get; set; }
    }

    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; } = null!;
        [StringLength(100)]
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? Category { get; set; }
        public bool IsPublic { get; set; } = false;
    }

    public class FileDownloadDto
    {
        public string FileName { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public byte[] FileContent { get; set; } = Array.Empty<byte>();
    }

    public class FileInfoDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;
        public string? MimeType { get; set; }
        public long FileSize { get; set; }
        public string? Category { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class BarcodeDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string BarcodeValue { get; set; } = string.Empty;
        [StringLength(50)]
        public string BarcodeType { get; set; } = "Code128"; // Code128, QR, EAN13, etc.
        [StringLength(100)]
        public string? EntityType { get; set; } // Product, Batch, Position, etc.
        public int? EntityId { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public DateTime GeneratedDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public UserDto? Creator { get; set; }
    }

    public class CreateBarcodeDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string BarcodeValue { get; set; } = string.Empty;
        [StringLength(50)]
        public string BarcodeType { get; set; } = "Code128";
        [StringLength(100)]
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class UpdateBarcodeDto
    {
        [StringLength(100)]
        public string? BarcodeValue { get; set; }
        [StringLength(50)]
        public string? BarcodeType { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    public class BarcodeGenerateDto
    {
        [Required]
        [StringLength(100)]
        public string BarcodeValue { get; set; } = string.Empty;
        [StringLength(50)]
        public string BarcodeType { get; set; } = "Code128";
        public int Width { get; set; } = 300;
        public int Height { get; set; } = 100;
    }

    public class BarcodeImageDto
    {
        public string BarcodeValue { get; set; } = string.Empty;
        public string BarcodeType { get; set; } = string.Empty;
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public string MimeType { get; set; } = "image/png";
    }
}
