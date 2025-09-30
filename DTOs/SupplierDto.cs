using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class SupplierDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string SupplierName { get; set; } = string.Empty;
        [StringLength(255)]
        public string? ContactPerson { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? TaxCode { get; set; }
        [StringLength(255)]
        public string? Website { get; set; }
        [StringLength(100)]
        public string? BankAccount { get; set; }
        [StringLength(255)]
        public string? BankName { get; set; }
        [StringLength(255)]
        public string? PaymentTerms { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? CreditLimit { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        [StringLength(50)]
        public string Status { get; set; } = "Active";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CompanyDto? Company { get; set; }
    }

    public class CreateSupplierDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string SupplierName { get; set; } = string.Empty;
        [StringLength(255)]
        public string? ContactPerson { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? TaxCode { get; set; }
        [StringLength(255)]
        public string? Website { get; set; }
        [StringLength(100)]
        public string? BankAccount { get; set; }
        [StringLength(255)]
        public string? BankName { get; set; }
        [StringLength(255)]
        public string? PaymentTerms { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? CreditLimit { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class UpdateSupplierDto
    {
        [StringLength(100)]
        public string? Code { get; set; }
        [StringLength(255)]
        public string? SupplierName { get; set; }
        [StringLength(255)]
        public string? ContactPerson { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? TaxCode { get; set; }
        [StringLength(255)]
        public string? Website { get; set; }
        [StringLength(100)]
        public string? BankAccount { get; set; }
        [StringLength(255)]
        public string? BankName { get; set; }
        [StringLength(255)]
        public string? PaymentTerms { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? CreditLimit { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
