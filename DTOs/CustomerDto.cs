using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; } = string.Empty;
        [StringLength(50)]
        public string CustomerType { get; set; } = "Company"; // Individual, Company
        [StringLength(255)]
        public string? ContactPerson { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(500)]
        public string? BillingAddress { get; set; }
        [StringLength(500)]
        public string? ShippingAddress { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? TaxCode { get; set; }
        [StringLength(100)]
        public string? BankAccount { get; set; }
        [StringLength(255)]
        public string? BankName { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? CreditLimit { get; set; }
        [StringLength(255)]
        public string? PaymentTerms { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        [Range(0, 100)]
        public decimal? DiscountRate { get; set; }
        [StringLength(50)]
        public string Status { get; set; } = "Active";
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public CompanyDto? Company { get; set; }
    }

    public class CreateCustomerDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string CustomerName { get; set; } = string.Empty;
        [StringLength(50)]
        public string CustomerType { get; set; } = "Company";
        [StringLength(255)]
        public string? ContactPerson { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(500)]
        public string? BillingAddress { get; set; }
        [StringLength(500)]
        public string? ShippingAddress { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? TaxCode { get; set; }
        [StringLength(100)]
        public string? BankAccount { get; set; }
        [StringLength(255)]
        public string? BankName { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? CreditLimit { get; set; }
        [StringLength(255)]
        public string? PaymentTerms { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        [Range(0, 100)]
        public decimal? DiscountRate { get; set; }
    }

    public class UpdateCustomerDto
    {
        [StringLength(100)]
        public string? Code { get; set; }
        [StringLength(255)]
        public string? CustomerName { get; set; }
        [StringLength(50)]
        public string? CustomerType { get; set; }
        [StringLength(255)]
        public string? ContactPerson { get; set; }
        [StringLength(500)]
        public string? Address { get; set; }
        [StringLength(500)]
        public string? BillingAddress { get; set; }
        [StringLength(500)]
        public string? ShippingAddress { get; set; }
        [StringLength(20)]
        public string? Phone { get; set; }
        [StringLength(255)]
        public string? Email { get; set; }
        [StringLength(50)]
        public string? TaxCode { get; set; }
        [StringLength(100)]
        public string? BankAccount { get; set; }
        [StringLength(255)]
        public string? BankName { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? CreditLimit { get; set; }
        [StringLength(255)]
        public string? PaymentTerms { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        [Range(0, 100)]
        public decimal? DiscountRate { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
