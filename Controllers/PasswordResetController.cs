using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.DTOs;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Services;
using System.Security.Cryptography;
using System.Text;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<PasswordResetController> _logger;

        public PasswordResetController(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<PasswordResetController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                // Validate user exists
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == request.Username && u.Email == request.Email);

                if (user == null)
                {
                    return BadRequest(new { success = false, message = "Không tìm thấy tài khoản với thông tin đã cung cấp" });
                }

                // Create password reset request
                var resetRequest = new PasswordResetRequest
                {
                    Username = request.Username,
                    Email = request.Email,
                    Message = request.Message,
                    Status = "Pending",
                    RequestedAt = DateTime.UtcNow
                };

                _context.PasswordResetRequests.Add(resetRequest);
                await _context.SaveChangesAsync();

                // Get admin users to notify
                var adminUsers = await _context.Users
                    .Where(u => u.Role == Models.Enums.UserRole.Admin)
                    .ToListAsync();

                // Send notification to all admins
                foreach (var admin in adminUsers)
                {
                    await _emailService.SendPasswordResetRequestNotificationAsync(
                        admin.Email, 
                        request.Username, 
                        request.Email, 
                        request.Message ?? "Yêu cầu cấp lại mật khẩu"
                    );
                }

                _logger.LogInformation("Password reset request created for user {Username}", request.Username);

                return Ok(new
                {
                    success = true,
                    message = "Yêu cầu cấp lại mật khẩu đã được gửi đến quản trị viên. Vui lòng chờ phản hồi qua email."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password request for {Username}", request.Username);
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi xử lý yêu cầu" });
            }
        }

        [HttpPost("contact-admin")]
        public async Task<IActionResult> ContactAdmin([FromBody] ContactAdminRequest request)
        {
            try
            {
                // Create contact request (same as password reset request but with different status)
                var contactRequest = new PasswordResetRequest
                {
                    Username = request.Username,
                    Email = request.Email,
                    Message = request.Message,
                    Status = "Contact",
                    RequestedAt = DateTime.UtcNow
                };

                _context.PasswordResetRequests.Add(contactRequest);
                await _context.SaveChangesAsync();

                // Get admin users to notify
                var adminUsers = await _context.Users
                    .Where(u => u.Role == Models.Enums.UserRole.Admin)
                    .ToListAsync();

                // Send notification to all admins
                foreach (var admin in adminUsers)
                {
                    await _emailService.SendPasswordResetRequestNotificationAsync(
                        admin.Email, 
                        request.Username, 
                        request.Email, 
                        request.Message
                    );
                }

                _logger.LogInformation("Contact admin request created for user {Username}", request.Username);

                return Ok(new
                {
                    success = true,
                    message = "Yêu cầu liên hệ đã được gửi đến quản trị viên. Vui lòng chờ phản hồi qua email."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact admin request for {Username}", request.Username);
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi xử lý yêu cầu" });
            }
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetPasswordResetRequests()
        {
            try
            {
                var requests = await _context.PasswordResetRequests
                    .OrderByDescending(r => r.RequestedAt)
                    .Select(r => new PasswordResetRequestResponse
                    {
                        Id = r.Id,
                        Username = r.Username,
                        Email = r.Email,
                        Message = r.Message,
                        Status = r.Status,
                        RequestedAt = r.RequestedAt,
                        ProcessedAt = r.ProcessedAt,
                        ProcessedBy = r.ProcessedBy.HasValue ? 
                            _context.Users.FirstOrDefault(u => u.Id == r.ProcessedBy.Value)!.Username : null,
                        AdminNotes = r.AdminNotes,
                        PasswordResetAt = r.PasswordResetAt
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = requests });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting password reset requests");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lấy danh sách yêu cầu" });
            }
        }

        [HttpPost("process-request")]
        public async Task<IActionResult> ProcessPasswordResetRequest([FromBody] ProcessPasswordResetRequest request)
        {
            try
            {
                var resetRequest = await _context.PasswordResetRequests
                    .FirstOrDefaultAsync(r => r.Id == request.RequestId);

                if (resetRequest == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy yêu cầu" });
                }

                // Get current admin user (in real app, get from JWT token)
                var adminUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == "admin");

                resetRequest.Status = request.Action;
                resetRequest.ProcessedAt = DateTime.UtcNow;
                resetRequest.ProcessedBy = adminUser?.Id;
                resetRequest.AdminNotes = request.AdminNotes;

                _context.PasswordResetRequests.Update(resetRequest);
                await _context.SaveChangesAsync();

                // Send response email to user
                if (request.Action == "Approve")
                {
                    await _emailService.SendPasswordResetApprovalEmailAsync(resetRequest.Email, resetRequest.Username);
                }
                else if (request.Action == "Reject")
                {
                    await _emailService.SendPasswordResetRejectionEmailAsync(
                        resetRequest.Email, 
                        resetRequest.Username, 
                        request.AdminNotes ?? "Yêu cầu không được chấp nhận"
                    );
                }

                _logger.LogInformation("Password reset request {RequestId} {Action} by admin", request.RequestId, request.Action);

                return Ok(new
                {
                    success = true,
                    message = $"Yêu cầu đã được {(request.Action == "Approve" ? "duyệt" : "từ chối")}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing password reset request {RequestId}", request.RequestId);
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi xử lý yêu cầu" });
            }
        }

        [HttpPost("reset-user-password")]
        public async Task<IActionResult> ResetUserPassword([FromBody] ResetUserPasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(request.UserId);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
                }

                // Hash the new password
                var hashedPassword = HashPassword(request.NewPassword);
                user.PasswordHash = hashedPassword;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);

                // Update password reset request if exists
                var resetRequest = await _context.PasswordResetRequests
                    .FirstOrDefaultAsync(r => r.Username == user.Username && r.Status == "Approved");

                if (resetRequest != null)
                {
                    resetRequest.Status = "Completed";
                    resetRequest.PasswordResetAt = DateTime.UtcNow;
                    _context.PasswordResetRequests.Update(resetRequest);
                }

                await _context.SaveChangesAsync();

                // Send new password to user
                await _emailService.SendPasswordResetEmailAsync(user.Email, user.Username, request.NewPassword);

                _logger.LogInformation("Password reset for user {Username} by admin", user.Username);

                return Ok(new
                {
                    success = true,
                    message = "Mật khẩu đã được cập nhật và gửi đến email người dùng"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user {UserId}", request.UserId);
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi cập nhật mật khẩu" });
            }
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
