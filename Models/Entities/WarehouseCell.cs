using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models.Entities;

public class WarehouseCell
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public int ZoneId { get; set; }
    public string CellCode { get; set; } = string.Empty;
    public string CellType { get; set; } = string.Empty; // Shelf, Floor, Rack
    public int Row { get; set; }
    public int Column { get; set; }
    public int MaxCapacity { get; set; }
    public int CurrentAmount { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Supplier { get; set; }
    public decimal? UnitPrice { get; set; }
    public int? ProductBatchId { get; set; }
    public DateTime? LastMoved { get; set; }
    public string Status { get; set; } = "Empty"; // Empty, Occupied, Full, Reserved
    public string? ClusterName { get; set; }
    public string? AssignedStaff { get; set; }
    
    // Environment fields
    public string? Temperature { get; set; }
    public string? Humidity { get; set; }
    public string? Ventilation { get; set; }
    public string? SensorStatus { get; set; }
    public string? ElectronicScale { get; set; }
    public string? Dimensions { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Warehouse? Warehouse { get; set; }
    public WarehouseZone? Zone { get; set; }
    public Product? Product { get; set; }
}

public class WarehouseZone : BaseEntity
{
    public int WarehouseId { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public string ZoneCode { get; set; } = string.Empty;
    public string ZoneType { get; set; } = string.Empty; // Storage, Loading, Unloading
    public int MaxCapacity { get; set; }
    public int CurrentCapacity { get; set; }
    public string Status { get; set; } = "Active";

    // Navigation properties
    public Warehouse? Warehouse { get; set; }
    public ICollection<WarehouseCell> Cells { get; set; } = new List<WarehouseCell>();
}

public class WarehouseCluster
{
    public string Id { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string ClusterName { get; set; } = string.Empty;
    public string ClusterType { get; set; } = string.Empty;
    public string Color { get; set; } = "#000000";
    public List<int> CellIds { get; set; } = new();
    public string Status { get; set; } = "Active";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Warehouse? Warehouse { get; set; }
}
