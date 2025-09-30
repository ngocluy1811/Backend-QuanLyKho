namespace FertilizerWarehouseAPI.Models.Entities;

public class Barcode : BaseEntity
{
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string BarcodeType { get; set; } = string.Empty; // QR, Code128, EAN13
    public string BarcodeValue { get; set; } = string.Empty;
    public string? BarcodeImage { get; set; } // Base64
    public DateTime GeneratedAt { get; set; }
    public int PrintedCount { get; set; } = 0;
}
