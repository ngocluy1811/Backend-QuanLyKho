using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class WarehouseZoneDto
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string ZoneName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public int WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
        
        public decimal Capacity { get; set; }
        public decimal UsedCapacity { get; set; }
        public decimal AvailableCapacity => Capacity - UsedCapacity;
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public List<WarehousePositionDto> Positions { get; set; } = new();
    }

    public class CreateWarehouseZoneDto
    {
        [Required]
        [StringLength(100)]
        public string ZoneName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Capacity { get; set; }
        
        public int WarehouseId { get; set; }
    }

    public class UpdateWarehouseZoneDto
    {
        [Required]
        [StringLength(100)]
        public string ZoneName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Capacity { get; set; }
        
        public bool IsActive { get; set; }
    }
}
