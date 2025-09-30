using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? TaxCode { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? LegalRepresentative { get; set; }
        public string? BusinessType { get; set; }
        public string Status { get; set; } = "Active"; // Active, Inactive
        public DateTime? EstablishedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Related entities
        public List<WarehouseDto>? Warehouses { get; set; }
        public List<DepartmentDto>? Departments { get; set; }
        public int EmployeeCount { get; set; }
        public int WarehouseCount { get; set; }
    }

    public class CreateCompanyDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TaxCode { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        [Url]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? LegalRepresentative { get; set; }

        [StringLength(100)]
        public string? BusinessType { get; set; }

        public DateTime? EstablishedDate { get; set; }
    }

    public class UpdateCompanyDto
    {
        [StringLength(200)]
        public string? Name { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        [Url]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? LegalRepresentative { get; set; }

        [StringLength(100)]
        public string? BusinessType { get; set; }

        public string? Status { get; set; }

        public DateTime? EstablishedDate { get; set; }
    }

    public class CompanyStatsDto
    {
        public int TotalEmployees { get; set; }
        public int TotalWarehouses { get; set; }
        public int TotalDepartments { get; set; }
        public decimal TotalWarehouseCapacity { get; set; }
        public decimal UsedWarehouseCapacity { get; set; }
        public int ActiveProjects { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}