namespace FertilizerWarehouseAPI.Models.Enums;

public enum MovementType
{
    Import = 1,      // Nhập kho
    Export = 2,      // Xuất kho
    Transfer = 3,    // Chuyển kho
    Adjustment = 4,  // Điều chỉnh
    Return = 5,      // Trả hàng
    Loss = 6,        // Hao hụt
    Found = 7,       // Thừa
    Production = 8,  // Sản xuất
    StockIn = 9,     // Nhập kho (alias for Import)
    StockOut = 10    // Xuất kho (alias for Export)
}
