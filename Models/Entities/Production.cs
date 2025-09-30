using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models.Entities;

public class Production
{
    public int Id { get; set; }
    public string ProductionOrderNumber { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal PlannedQuantity { get; set; }
    public decimal ActualQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime PlannedStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public string Status { get; set; } = "Planned"; // Planned, InProgress, Completed, Cancelled
    public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical
    public int? AssignedTo { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Product? Product { get; set; }
    public User? AssignedUser { get; set; }
    public ICollection<ProductionMachine> Machines { get; set; } = new List<ProductionMachine>();
}

public class ProductionMachine
{
    public int Id { get; set; }
    public int ProductionId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string MachineType { get; set; } = string.Empty;
    public string MachineCode { get; set; } = string.Empty;
    public string Status { get; set; } = "Available"; // Available, InUse, Maintenance, Offline
    public decimal Efficiency { get; set; } = 100; // Percentage
    public int? AssignedOperator { get; set; }
    public DateTime? LastMaintenance { get; set; }
    public DateTime? NextMaintenance { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Production? Production { get; set; }
    public User? AssignedOperatorUser { get; set; }
}
