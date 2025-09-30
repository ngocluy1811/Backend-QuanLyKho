using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class UserWarehouse : BaseEntity
    {
        public int UserId { get; set; }
        public int WarehouseId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public int? AssignedBy { get; set; }
        public new bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual User User { get; set; } = null!;
        public virtual Warehouse Warehouse { get; set; } = null!;
        public virtual User? AssignedByUser { get; set; }
    }
}
