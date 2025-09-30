using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models.Entities;

public class Department
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public int? ManagerId { get; set; }
    
    public int? CompanyId { get; set; }
    
    [StringLength(50)]
    public string Status { get; set; } = "Active";
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? UpdatedAt { get; set; }
    
    public int CreatedBy { get; set; }
    
    public int? UpdatedBy { get; set; }
    
    // Navigation properties
    public Company? Company { get; set; }
    public User? Manager { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
}