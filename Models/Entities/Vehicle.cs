namespace FertilizerWarehouseAPI.Models.Entities;

public class Vehicle : BaseEntity
{
    public int CompanyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Truck, Van, Forklift
    public string LicensePlate { get; set; } = string.Empty;
    public decimal Capacity { get; set; } // Kg
    public string? DriverName { get; set; }
    public string? DriverPhone { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime? LastMaintenance { get; set; }
    public DateTime? NextMaintenance { get; set; }

    // Navigation properties
    public virtual Company Company { get; set; } = null!;
}
