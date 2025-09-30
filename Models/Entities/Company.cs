using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class Company
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string CompanyName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Code { get; set; }
        
        [MaxLength(50)]
        public string? TaxCode { get; set; }
        
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [MaxLength(100)]
        public string? Email { get; set; }
        
        [MaxLength(500)]
        public string? Address { get; set; }
        
        [MaxLength(100)]
        public string? Department { get; set; }
        
        [MaxLength(50)]
        public string? Status { get; set; } = "Active";
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
        public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}