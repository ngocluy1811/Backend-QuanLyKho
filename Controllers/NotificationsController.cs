using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.DTOs;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly List<NotificationDto> _notifications;

    public NotificationsController()
    {
        _notifications = new List<NotificationDto>
        {
            new NotificationDto
            {
                Id = 1,
                CompanyId = 1,
                UserId = 1,
                Message = "Cảnh báo tồn kho thấp - Sản phẩm NPK 16-16-8 có tồn kho dưới 100 đơn vị",
                NotificationType = "Alert",
                IsRead = false,
                SentAt = DateTime.Now.AddHours(-1),
                EntityType = "Product",
                EntityId = 1
            },
            new NotificationDto
            {
                Id = 2,
                CompanyId = 1,
                UserId = 1,
                Message = "Đơn hàng mới - Có đơn hàng mới #SO-001 cần xử lý",
                NotificationType = "Info",
                IsRead = false,
                SentAt = DateTime.Now.AddHours(-2),
                EntityType = "Order",
                EntityId = 1
            },
            new NotificationDto
            {
                Id = 3,
                CompanyId = 1,
                UserId = 1,
                Message = "Bảo trì thiết bị - Máy đóng gói số 3 cần bảo trì định kỳ",
                NotificationType = "System",
                IsRead = true,
                SentAt = DateTime.Now.AddDays(-1),
                EntityType = "Machine",
                EntityId = 3
            },
            new NotificationDto
            {
                Id = 4,
                CompanyId = 1,
                UserId = 1,
                Message = "Kiểm kê kho - Đã hoàn thành kiểm kê kho A - Dãy B",
                NotificationType = "Info",
                IsRead = true,
                SentAt = DateTime.Now.AddDays(-2),
                EntityType = "Warehouse",
                EntityId = 1
            },
            new NotificationDto
            {
                Id = 5,
                CompanyId = 1,
                UserId = 1,
                Message = "Cập nhật hệ thống - Hệ thống đã được cập nhật lên phiên bản mới",
                NotificationType = "System",
                IsRead = false,
                SentAt = DateTime.Now.AddDays(-3),
                EntityType = "System",
                EntityId = null
            }
        };
    }

    /// <summary>
    /// Get all notifications for current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetNotifications()
    {
        try
        {
            var userId = GetCurrentUserId();
            var userNotifications = _notifications.Where(n => n.UserId == userId).ToList();
            return Ok(userNotifications);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy danh sách thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Get unread notifications count
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var unreadCount = _notifications.Count(n => n.UserId == userId && !n.IsRead);
            return Ok(unreadCount);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy số thông báo chưa đọc", error = ex.Message });
        }
    }

    /// <summary>
    /// Get notification by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationDto>> GetNotification(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var notification = _notifications.FirstOrDefault(n => n.Id == id && n.UserId == userId);
            
            if (notification == null)
            {
                return NotFound(new { message = "Không tìm thấy thông báo" });
            }

            return Ok(notification);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPost("{id}/read")]
    public async Task<ActionResult> MarkAsRead(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var notification = _notifications.FirstOrDefault(n => n.Id == id && n.UserId == userId);
            
            if (notification == null)
            {
                return NotFound(new { message = "Không tìm thấy thông báo" });
            }

            notification.IsRead = true;
            // ReadAt property doesn't exist in NotificationDto

            return Ok(new { message = "Đã đánh dấu thông báo là đã đọc" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi đánh dấu thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost("read-all")]
    public async Task<ActionResult> MarkAllAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            var userNotifications = _notifications.Where(n => n.UserId == userId && !n.IsRead).ToList();
            
            foreach (var notification in userNotifications)
            {
                notification.IsRead = true;
                // ReadAt property doesn't exist in NotificationDto
            }

            return Ok(new { message = $"Đã đánh dấu {userNotifications.Count} thông báo là đã đọc" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi đánh dấu tất cả thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteNotification(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var notification = _notifications.FirstOrDefault(n => n.Id == id && n.UserId == userId);
            
            if (notification == null)
            {
                return NotFound(new { message = "Không tìm thấy thông báo" });
            }

            _notifications.Remove(notification);

            return Ok(new { message = "Đã xóa thông báo" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi xóa thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Create new notification
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateNotificationDto createDto)
    {
        try
        {
        var notification = new NotificationDto
        {
            Id = _notifications.Max(n => n.Id) + 1,
            CompanyId = 1,
            UserId = createDto.UserId,
            Message = createDto.Message,
            NotificationType = createDto.Type,
            IsRead = false,
            SentAt = DateTime.Now,
            EntityType = "System",
            EntityId = null
        };

            _notifications.Add(notification);

            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi tạo thông báo", error = ex.Message });
        }
    }

    /// <summary>
    /// Get notifications by type
    /// </summary>
    [HttpGet("by-type/{type}")]
    public async Task<ActionResult<List<NotificationDto>>> GetNotificationsByType(string type)
    {
        try
        {
            var userId = GetCurrentUserId();
            var notifications = _notifications
                .Where(n => n.UserId == userId && n.NotificationType == type)
                .ToList();

            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy thông báo theo loại", error = ex.Message });
        }
    }

    /// <summary>
    /// Get notifications by priority
    /// </summary>
    [HttpGet("by-priority/{priority}")]
    public async Task<ActionResult<List<NotificationDto>>> GetNotificationsByPriority(string priority)
    {
        try
        {
            var userId = GetCurrentUserId();
            var notifications = _notifications
                .Where(n => n.UserId == userId) // Priority property doesn't exist in NotificationDto
                .ToList();

            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi khi lấy thông báo theo mức độ ưu tiên", error = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        // In a real application, you would get this from the JWT token
        // For now, return a default user ID
        return 1;
    }
}

// DTOs for Notifications
public class CreateNotificationDto
{
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int UserId { get; set; }
}