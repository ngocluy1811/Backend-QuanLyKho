namespace FertilizerWarehouseAPI.Models.Enums;

public enum UserStatus
{
    Pending = 0,    // Chờ xác nhận
    Active = 1,     // Hoạt động
    Inactive = 2,   // Không hoạt động
    Suspended = 3   // Tạm khóa
}
