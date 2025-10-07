using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.Services;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using System.Security.Claims;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(ISecurityService securityService, ApplicationDbContext context, ILogger<SecurityController> logger)
        {
            _securityService = securityService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get user's login history
        /// </summary>
        [HttpGet("login-history")]
        public async Task<IActionResult> GetLoginHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var loginHistory = await _securityService.GetLoginHistoryAsync(userId.Value.GetHashCode(), page, pageSize);
                return Ok(new
                {
                    success = true,
                    data = loginHistory,
                    page,
                    pageSize,
                    total = loginHistory.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting login history for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get user's security alerts
        /// </summary>
        [HttpGet("alerts")]
        public async Task<IActionResult> GetSecurityAlerts()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var alerts = await _securityService.GetActiveAlertsAsync(userId.Value.GetHashCode());
                return Ok(new
                {
                    success = true,
                    data = alerts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security alerts for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get user's trusted devices
        /// </summary>
        [HttpGet("trusted-devices")]
        public async Task<IActionResult> GetTrustedDevices()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var devices = await _context.TrustedDevices
                    .Where(td => td.UserId == userId.Value.GetHashCode() && td.IsActive)
                    .OrderByDescending(td => td.LastSeen)
                    .Select(td => new
                    {
                        td.Id,
                        td.DeviceName,
                        td.DeviceType,
                        td.OperatingSystem,
                        td.Browser,
                        td.Location,
                        td.FirstSeen,
                        td.LastSeen,
                        td.LoginCount,
                        td.TrustLevel,
                        td.IsBlocked,
                        td.BlockReason
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    data = devices
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trusted devices for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Block a trusted device
        /// </summary>
        [HttpPost("block-device/{deviceId}")]
        public async Task<IActionResult> BlockDevice(Guid deviceId, [FromBody] BlockDeviceRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                // Verify device belongs to user
                var device = await _context.TrustedDevices
                    .FirstOrDefaultAsync(td => td.Id == deviceId && td.UserId == userId.Value.GetHashCode());

                if (device == null)
                {
                    return NotFound(new { success = false, message = "Device not found" });
                }

                await _securityService.BlockDeviceAsync(deviceId, request.Reason);

                return Ok(new
                {
                    success = true,
                    message = "Device blocked successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error blocking device {DeviceId}", deviceId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get security dashboard data
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetSecurityDashboard()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var dashboard = await _securityService.GetSecurityDashboardAsync(userId.Value.GetHashCode());
                return Ok(new
                {
                    success = true,
                    data = dashboard
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security dashboard for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Get suspicious activities
        /// </summary>
        [HttpGet("suspicious-activities")]
        public async Task<IActionResult> GetSuspiciousActivities()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var activities = await _securityService.GetSuspiciousActivitiesAsync(userId.Value.GetHashCode());
                return Ok(new
                {
                    success = true,
                    data = activities
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting suspicious activities for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Mark security alert as read
        /// </summary>
        [HttpPost("alerts/{alertId}/read")]
        public async Task<IActionResult> MarkAlertAsRead(Guid alertId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var alert = await _context.SecurityAlerts
                    .FirstOrDefaultAsync(sa => sa.Id == alertId && sa.UserId == userId.Value.GetHashCode());

                if (alert == null)
                {
                    return NotFound(new { success = false, message = "Alert not found" });
                }

                alert.IsRead = true;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Alert marked as read"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking alert as read {AlertId}", alertId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Resolve security alert
        /// </summary>
        [HttpPost("alerts/{alertId}/resolve")]
        public async Task<IActionResult> ResolveAlert(Guid alertId, [FromBody] ResolveAlertRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var alert = await _context.SecurityAlerts
                    .FirstOrDefaultAsync(sa => sa.Id == alertId && sa.UserId == userId.Value.GetHashCode());

                if (alert == null)
                {
                    return NotFound(new { success = false, message = "Alert not found" });
                }

                alert.IsResolved = true;
                alert.ResolvedAt = DateTime.UtcNow;
                alert.ResolutionNotes = request.ResolutionNotes;
                alert.Status = "Resolved";

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Alert resolved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving alert {AlertId}", alertId);
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        /// <summary>
        /// Invalidate all user sessions (force logout from all devices)
        /// </summary>
        [HttpPost("invalidate-all-sessions")]
        public async Task<IActionResult> InvalidateAllSessions()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                await _securityService.InvalidateAllUserSessionsAsync(userId.Value.GetHashCode());

                return Ok(new
                {
                    success = true,
                    message = "All sessions invalidated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating all sessions for user");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }
    }

    public class BlockDeviceRequest
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class ResolveAlertRequest
    {
        public string? ResolutionNotes { get; set; }
    }
}
