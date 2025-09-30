using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertilizerWarehouseAPI.Models.Entities
{
    public class StockItem : BaseEntity
    {
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int? BatchId { get; set; }
        public int? PositionId { get; set; }

        [Column(TypeName = "decimal(18,3)")]
        public decimal Quantity { get; set; } = 0;

        [Column(TypeName = "decimal(18,3)")]
        public decimal ReservedQuantity { get; set; } = 0;

        [Column(TypeName = "decimal(18,3)")]
        public decimal AvailableQuantity => Quantity - ReservedQuantity;

        public DateTime? LastMovementDate { get; set; }

        // Navigation properties
        public virtual Warehouse Warehouse { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual ProductBatch? Batch { get; set; }
        public virtual WarehousePosition? Position { get; set; }
    }
}
