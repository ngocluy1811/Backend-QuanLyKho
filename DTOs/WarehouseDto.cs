using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal TotalCapacity { get; set; }
        public decimal UsedCapacity { get; set; }
        public decimal AvailableCapacity { get; set; }
        public int TotalCells { get; set; }
        public int OccupiedCells { get; set; }
        public int EmptyCells { get; set; }
        public string Status { get; set; } = string.Empty; // Active, Inactive, Maintenance
        public string? Description { get; set; }
        public string? ManagerName { get; set; }
        public DateTime? LastInventoryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // For warehouse management grid
        public List<WarehouseCellDto> Cells { get; set; } = new();
        public List<WarehouseClusterDto> Clusters { get; set; } = new();
        public WarehouseStatsDto Stats { get; set; } = new();
    }

    public class CreateWarehouseDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal TotalCapacity { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ManagerName { get; set; }

        // Grid configuration
        public int GridRows { get; set; } = 10;
        public int GridColumns { get; set; } = 10;
    }

    public class UpdateWarehouseDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? TotalCapacity { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? ManagerName { get; set; }

        public string? Status { get; set; }
    }

    public class WarehouseCellDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Type { get; set; } = "cell"; // cell, path, empty
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public string? ClusterId { get; set; }
        public string? Product { get; set; }
        public decimal Capacity { get; set; }
        public string Status { get; set; } = string.Empty; // empty, low, medium, high, full
        public string? Dimensions { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal MaxCapacity { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public string? LotNumber { get; set; }
        public DateTime? ProductionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? Supplier { get; set; }
        public DateTime? LastUpdated { get; set; }
        public List<WarehouseStaffDto> StaffResponsible { get; set; } = new();
        public List<WarehouseActivityDto> RecentActivities { get; set; } = new();
    }

    public class WarehouseClusterDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public List<int> CellIds { get; set; } = new();
        public int CellCount => CellIds.Count;
    }

    public class CreateWarehouseClusterDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Color { get; set; } = "#3b82f6";

        public List<int> CellIds { get; set; } = new();
    }

    public class UpdateWarehouseClusterDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        public string? Color { get; set; }

        public List<int>? CellIds { get; set; }
    }

    public class WarehouseStaffDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Present { get; set; }
    }

    public class WarehouseActivityDto
    {
        public string Activity { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string User { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }

    public class WarehouseStatsDto
    {
        public int TotalCells { get; set; }
        public int OccupiedCells { get; set; }
        public int FullCells { get; set; }
        public decimal AverageCapacity { get; set; }
        public int Alerts { get; set; }
    }

    public class WarehouseConfigDto
    {
        public int WarehouseId { get; set; }
        public int GridRows { get; set; } = 10;
        public int GridColumns { get; set; } = 10;
        public Dictionary<string, object> Settings { get; set; } = new();
        public DateTime? LastModified { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class WarehouseTransferDto
    {
        public int SourceCellId { get; set; }
        public int TargetCellId { get; set; }
        public decimal TransferAmount { get; set; }
        public string? Notes { get; set; }
        public string? Reason { get; set; }
    }
}