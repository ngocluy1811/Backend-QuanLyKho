using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/notification/all
        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllNotifications()
        {
            try
            {
                var notifications = await _context.Notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new
                    {
                        id = n.Id,
                        title = n.Title,
                        message = n.Message,
                        type = n.Type,
                        priority = n.Priority,
                        isRead = n.IsRead,
                        createdAt = n.CreatedAt,
                        updatedAt = n.UpdatedAt,
                        userId = n.UserId,
                        userName = n.User != null ? n.User.FullName : "System"
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = notifications });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy danh sách thông báo", error = ex.Message });
            }
        }

        // GET: api/notification/user/{userId}
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserNotifications(int userId)
        {
            try
            {
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId || n.UserId == null) // Global notifications
                    .OrderByDescending(n => n.CreatedAt)
                    .Select(n => new
                    {
                        id = n.Id,
                        title = n.Title,
                        message = n.Message,
                        type = n.Type,
                        priority = n.Priority,
                        isRead = n.IsRead,
                        createdAt = n.CreatedAt,
                        updatedAt = n.UpdatedAt,
                        userId = n.UserId,
                        userName = n.User != null ? n.User.FullName : "System"
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = notifications });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy thông báo người dùng", error = ex.Message });
            }
        }

        // POST: api/notification/create
        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Message))
                {
                    return BadRequest(new { success = false, message = "Tiêu đề và nội dung thông báo không được để trống" });
                }

                var notification = new Notification
                {
                    Title = request.Title,
                    Message = request.Message,
                    Type = request.Type ?? "info",
                    Priority = request.Priority ?? "medium",
                    IsRead = false,
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Tạo thông báo thành công", data = new { id = notification.Id } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tạo thông báo", error = ex.Message });
            }
        }

        // PUT: api/notification/{id}/read
        [HttpPut("{id}/read")]
        [AllowAnonymous]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy thông báo" });
                }

                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Đã đánh dấu thông báo là đã đọc" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật thông báo", error = ex.Message });
            }
        }

        // PUT: api/notification/{id}/unread
        [HttpPut("{id}/unread")]
        [AllowAnonymous]
        public async Task<IActionResult> MarkAsUnread(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy thông báo" });
                }

                notification.IsRead = false;
                notification.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Đã đánh dấu thông báo là chưa đọc" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật thông báo", error = ex.Message });
            }
        }

        // DELETE: api/notification/{id}
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy thông báo" });
                }

                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Xóa thông báo thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa thông báo", error = ex.Message });
            }
        }

        // PUT: api/notification/mark-all-read
        [HttpPut("mark-all-read")]
        [AllowAnonymous]
        public async Task<IActionResult> MarkAllAsRead([FromBody] MarkAllReadRequest request)
        {
            try
            {
                var query = _context.Notifications.AsQueryable();
                
                if (request.UserId.HasValue)
                {
                    query = query.Where(n => n.UserId == request.UserId || n.UserId == null);
                }

                var notifications = await query.Where(n => !n.IsRead).ToListAsync();
                
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                    notification.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = $"Đã đánh dấu {notifications.Count} thông báo là đã đọc" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi đánh dấu tất cả thông báo", error = ex.Message });
            }
        }

        // GET: api/notification/stats
        [HttpGet("stats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNotificationStats()
        {
            try
            {
                var totalNotifications = await _context.Notifications.CountAsync();
                var unreadNotifications = await _context.Notifications.CountAsync(n => !n.IsRead);
                var todayNotifications = await _context.Notifications.CountAsync(n => n.CreatedAt.Date == DateTime.UtcNow.Date);
                
                var statsByType = await _context.Notifications
                    .GroupBy(n => n.Type)
                    .Select(g => new { type = g.Key, count = g.Count() })
                    .ToListAsync();

                var statsByPriority = await _context.Notifications
                    .GroupBy(n => n.Priority)
                    .Select(g => new { priority = g.Key, count = g.Count() })
                    .ToListAsync();

                var stats = new
                {
                    total = totalNotifications,
                    unread = unreadNotifications,
                    today = todayNotifications,
                    byType = statsByType,
                    byPriority = statsByPriority
                };

                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy thống kê thông báo", error = ex.Message });
            }
        }
    }

    public class CreateNotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Type { get; set; }
        public string? Priority { get; set; }
        public int? UserId { get; set; }
    }

    public class MarkAllReadRequest
    {
        public int? UserId { get; set; }
    }
}
