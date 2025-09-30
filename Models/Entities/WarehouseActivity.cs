using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models.Entities;

public class WarehouseActivity
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public int? CellId { get; set; }
    public string ActivityType { get; set; } = string.Empty; // Import, Export, Transfer, Maintenance, StaffAssignment, etc.
    public string Description { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public string? BatchNumber { get; set; }
    public int? Quantity { get; set; }
    public string? Unit { get; set; }
    public string? FromLocation { get; set; }
    public string? ToLocation { get; set; }
    public string? UserName { get; set; }
    public int? UserId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
    public string Status { get; set; } = "Completed"; // Completed, Pending, Failed
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Warehouse? Warehouse { get; set; }
    public virtual WarehouseCell? Cell { get; set; }
    public virtual User? User { get; set; }
}
