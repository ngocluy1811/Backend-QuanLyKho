using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities;

public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string CustomerCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string CustomerName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    [MaxLength(50)]
    public string? TaxCode { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Active"; // Active, Inactive

    public int? CompanyId { get; set; }
    [ForeignKey("CompanyId")]
    public Company? Company { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<ImportOrder> ImportOrders { get; set; } = new List<ImportOrder>();
    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}