namespace FertilizerWarehouseAPI.Models.Enums;

public enum OrderStatus
{
    Draft = 1,
    Pending = 2,
    Approved = 3,
    Processing = 4,
    Completed = 5,
    Delivered = 6,
    Cancelled = 7,
    Rejected = 8,
    PartiallyCompleted = 9
}
