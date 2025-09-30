using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.DTOs
{
    public class VehicleDto
    {
        public int Id { get; set; }
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string VehicleCode { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string VehicleName { get; set; } = string.Empty;
        [StringLength(50)]
        public string VehicleType { get; set; } = string.Empty; // Truck, Van, Forklift, etc.
        [StringLength(50)]
        public string LicensePlate { get; set; } = string.Empty;
        [StringLength(100)]
        public string? Brand { get; set; }
        [StringLength(100)]
        public string? Model { get; set; }
        public int? ManufactureYear { get; set; }
        [StringLength(100)]
        public string? Color { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? LoadCapacity { get; set; } // in kg or tons
        [StringLength(100)]
        public string? FuelType { get; set; } // Gasoline, Diesel, Electric, etc.
        [Range(0, double.MaxValue)]
        public decimal? FuelConsumption { get; set; } // L/100km or similar
        public DateTime? PurchaseDate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? PurchasePrice { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public DateTime? InspectionExpiryDate { get; set; }
        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Maintenance, Retired, Damaged
        public int? CurrentDriverId { get; set; }
        [StringLength(500)]
        public string? CurrentLocation { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Mileage { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserDto? CurrentDriver { get; set; }
    }

    public class CreateVehicleDto
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [StringLength(100)]
        public string VehicleCode { get; set; } = string.Empty;
        [Required]
        [StringLength(255)]
        public string VehicleName { get; set; } = string.Empty;
        [StringLength(50)]
        public string VehicleType { get; set; } = string.Empty;
        [StringLength(50)]
        public string LicensePlate { get; set; } = string.Empty;
        [StringLength(100)]
        public string? Brand { get; set; }
        [StringLength(100)]
        public string? Model { get; set; }
        public int? ManufactureYear { get; set; }
        [StringLength(100)]
        public string? Color { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? LoadCapacity { get; set; }
        [StringLength(100)]
        public string? FuelType { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? FuelConsumption { get; set; }
        public DateTime? PurchaseDate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? PurchasePrice { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public DateTime? InspectionExpiryDate { get; set; }
        public int? CurrentDriverId { get; set; }
        [StringLength(500)]
        public string? CurrentLocation { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Mileage { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class UpdateVehicleDto
    {
        [StringLength(100)]
        public string? VehicleCode { get; set; }
        [StringLength(255)]
        public string? VehicleName { get; set; }
        [StringLength(50)]
        public string? VehicleType { get; set; }
        [StringLength(50)]
        public string? LicensePlate { get; set; }
        [StringLength(100)]
        public string? Brand { get; set; }
        [StringLength(100)]
        public string? Model { get; set; }
        public int? ManufactureYear { get; set; }
        [StringLength(100)]
        public string? Color { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? LoadCapacity { get; set; }
        [StringLength(100)]
        public string? FuelType { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? FuelConsumption { get; set; }
        public DateTime? PurchaseDate { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? PurchasePrice { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public DateTime? InspectionExpiryDate { get; set; }
        [StringLength(50)]
        public string? Status { get; set; }
        public int? CurrentDriverId { get; set; }
        [StringLength(500)]
        public string? CurrentLocation { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Mileage { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool? IsActive { get; set; }
    }

    public class VehicleSummaryDto
    {
        public int Id { get; set; }
        public string VehicleCode { get; set; } = string.Empty;
        public string VehicleName { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CurrentDriver { get; set; }
        public string? CurrentLocation { get; set; }
        public decimal? Mileage { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
    }

    public class VehicleMaintenanceDto
    {
        public int Id { get; set; }
        [Required]
        public int VehicleId { get; set; }
        [Required]
        [StringLength(100)]
        public string MaintenanceType { get; set; } = string.Empty; // Regular, Repair, Inspection
        [Required]
        public DateTime MaintenanceDate { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Cost { get; set; }
        [StringLength(255)]
        public string? ServiceProvider { get; set; }
        public decimal? MileageAtMaintenance { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public VehicleDto? Vehicle { get; set; }
    }

    public class CreateVehicleMaintenanceDto
    {
        [Required]
        public int VehicleId { get; set; }
        [Required]
        [StringLength(100)]
        public string MaintenanceType { get; set; } = string.Empty;
        [Required]
        public DateTime MaintenanceDate { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Cost { get; set; }
        [StringLength(255)]
        public string? ServiceProvider { get; set; }
        public decimal? MileageAtMaintenance { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
