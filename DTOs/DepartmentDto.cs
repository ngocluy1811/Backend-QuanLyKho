using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string DepartmentName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        public int? ManagerId { get; set; }
        [StringLength(50)]
        public string Status { get; set; } = "Active";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CompanyDto? Company { get; set; }
        public UserDto? Manager { get; set; }
        public ICollection<UserDto>? Users { get; set; }
    }

    public class CreateDepartmentDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string DepartmentName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        public int? ManagerId { get; set; }
    }

    public class UpdateDepartmentDto
    {
        [StringLength(100)]
        public string? Code { get; set; }
        [StringLength(255)]
        public string? DepartmentName { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public int? ManagerId { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
