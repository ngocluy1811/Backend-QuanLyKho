using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class TagDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string TagName { get; set; } = string.Empty;
        [StringLength(100)]
        public string? Color { get; set; } // Hex color code
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string Category { get; set; } = string.Empty; // Product, Order, Task, etc.
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public UserDto? Creator { get; set; }
        public int UsageCount { get; set; } // Number of entities using this tag
    }

    public class CreateTagDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string TagName { get; set; } = string.Empty;
        [StringLength(100)]
        public string? Color { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
    }

    public class UpdateTagDto
    {
        [StringLength(100)]
        public string? TagName { get; set; }
        [StringLength(100)]
        public string? Color { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [StringLength(100)]
        public string? Category { get; set; }
        public bool? IsActive { get; set; }
    }

    public class EntityTagDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int TagId { get; set; }
        [Required]
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty; // Product, Order, Task, etc.
        [Required]
        public int EntityId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public TagDto? Tag { get; set; }
        public UserDto? Creator { get; set; }
    }

    public class CreateEntityTagDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        public int TagId { get; set; }
        [Required]
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty;
        [Required]
        public int EntityId { get; set; }
    }

    public class UpdateEntityTagDto
    {
        public int? TagId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class TaggedEntityDto
    {
        public int EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public List<TagDto> Tags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class TagSummaryDto
    {
        public int Id { get; set; }
        public string TagName { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string Category { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class TagCloudDto
    {
        public string TagName { get; set; } = string.Empty;
        public string? Color { get; set; }
        public int Weight { get; set; } // Usage count for sizing
        public string Category { get; set; } = string.Empty;
    }

    public class BulkTagDto
    {
        [Required]
        public List<int> TagIds { get; set; } = new();
        [Required]
        [StringLength(100)]
        public string EntityType { get; set; } = string.Empty;
        [Required]
        public List<int> EntityIds { get; set; } = new();
        [StringLength(10)]
        public string Action { get; set; } = "Add"; // Add, Remove, Replace
    }

    public class TagFilterDto
    {
        public List<int>? TagIds { get; set; }
        public string? Category { get; set; }
        public string? EntityType { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public string? SearchTerm { get; set; }
    }
}
