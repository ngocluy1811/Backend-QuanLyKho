using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models;
using FertilizerWarehouseAPI.Models.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/leave-requests")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/leave-requests
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetLeaveRequests()
        {
            try
            {
                var requests = await _context.LeaveRequests
                    .Include(lr => lr.User)
                    .ThenInclude(u => u.Department)
                    .Include(lr => lr.ApprovedByUser)
                    .Select(lr => new
                    {
                        Id = lr.Id,
                        UserId = lr.UserId,
                        EmployeeId = lr.User.Username,
                        EmployeeName = lr.User.FullName,
                        Department = lr.User.Department != null ? lr.User.Department.Name : "N/A",
                        Position = lr.User.Role.ToString(),
                        Type = lr.Type,
                        StartDate = lr.StartDate.ToString("yyyy-MM-dd"),
                        EndDate = lr.EndDate.ToString("yyyy-MM-dd"),
                        TotalDays = lr.TotalDays,
                        Reason = lr.Reason,
                        Status = lr.Status,
                        RequestDate = lr.RequestDate.ToString("yyyy-MM-dd"),
                        ApprovedBy = lr.ApprovedByUser != null ? lr.ApprovedByUser.FullName : null,
                        ApprovedDate = lr.ApprovedDate.HasValue ? lr.ApprovedDate.Value.ToString("yyyy-MM-dd") : null,
                        Comments = lr.Comments ?? ""
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = requests });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy danh sách đơn xin phép", error = ex.Message });
            }
        }

        // POST: api/leave-requests
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestDto request)
        {
            try
            {
                // Validate request data
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu yêu cầu không hợp lệ" });
                }

                if (string.IsNullOrEmpty(request.Type))
                {
                    return BadRequest(new { success = false, message = "Loại nghỉ phép không được để trống" });
                }

                if (string.IsNullOrEmpty(request.Reason))
                {
                    return BadRequest(new { success = false, message = "Lý do nghỉ phép không được để trống" });
                }

                if (request.StartDate >= request.EndDate)
                {
                    return BadRequest(new { success = false, message = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc" });
                }

                if (request.StartDate < DateTime.Today)
                {
                    return BadRequest(new { success = false, message = "Ngày bắt đầu không được nhỏ hơn ngày hiện tại" });
                }

                if (request.TotalDays <= 0)
                {
                    return BadRequest(new { success = false, message = "Số ngày nghỉ phải lớn hơn 0" });
                }

                // Use first user as default for testing
                var firstUser = await _context.Users.FirstOrDefaultAsync();
                if (firstUser == null)
                {
                    return BadRequest(new { success = false, message = "Không tìm thấy người dùng nào trong hệ thống" });
                }

                // Calculate total days if not provided
                var totalDays = request.TotalDays;
                if (totalDays <= 0)
                {
                    totalDays = (int)(request.EndDate - request.StartDate).TotalDays + 1;
                }

                var leaveRequest = new LeaveRequestModel
                {
                    UserId = firstUser.Id,
                    Type = request.Type.Trim(),
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalDays = totalDays,
                    Reason = request.Reason.Trim(),
                    Status = "pending",
                    RequestDate = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                _context.LeaveRequests.Add(leaveRequest);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Tạo đơn xin phép thành công", 
                    data = new { 
                        id = leaveRequest.Id,
                        type = leaveRequest.Type,
                        startDate = leaveRequest.StartDate.ToString("yyyy-MM-dd"),
                        endDate = leaveRequest.EndDate.ToString("yyyy-MM-dd"),
                        totalDays = leaveRequest.TotalDays,
                        reason = leaveRequest.Reason,
                        status = leaveRequest.Status,
                        requestDate = leaveRequest.RequestDate.ToString("yyyy-MM-dd")
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tạo đơn xin phép", error = ex.Message });
            }
        }

        // GET: api/leave-requests/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveRequest(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var isAdmin = await IsAdminUser();
                
                var query = _context.LeaveRequests
                    .Include(lr => lr.User)
                    .Include(lr => lr.ApprovedByUser)
                    .AsQueryable();

                // Non-admin users can only see their own requests
                if (!isAdmin && userId.HasValue)
                {
                    query = query.Where(lr => lr.UserId == userId.Value);
                }

                var leaveRequest = await query
                    .Where(lr => lr.Id == id)
                    .Select(lr => new
                    {
                        lr.Id,
                        lr.UserId,
                        EmployeeId = lr.User.Username,
                        EmployeeName = lr.User.FullName,
                        Department = lr.User.Department != null ? lr.User.Department.Name : "N/A",
                        Position = lr.User.Role.ToString(),
                        Type = lr.Type,
                        StartDate = lr.StartDate.ToString("yyyy-MM-dd"),
                        EndDate = lr.EndDate.ToString("yyyy-MM-dd"),
                        TotalDays = lr.TotalDays,
                        Reason = lr.Reason,
                        Status = lr.Status,
                        RequestDate = lr.RequestDate.ToString("yyyy-MM-dd"),
                        ApprovedBy = lr.ApprovedByUser != null ? lr.ApprovedByUser.FullName : null,
                        ApprovedDate = lr.ApprovedDate.HasValue ? lr.ApprovedDate.Value.ToString("yyyy-MM-dd") : null,
                        Comments = lr.Comments ?? ""
                    })
                    .FirstOrDefaultAsync();

                if (leaveRequest == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy đơn xin phép" });
                }

                return Ok(new { success = true, data = leaveRequest });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy thông tin đơn xin phép", error = ex.Message });
            }
        }

        // PUT: api/leave-requests/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveRequest(int id, [FromBody] UpdateLeaveRequestDto request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var leaveRequest = await _context.LeaveRequests.FindAsync(id);
                if (leaveRequest == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy đơn xin phép" });
                }

                // Only the creator can update their own request, and only if it's still pending
                if (leaveRequest.UserId != userId.Value)
                {
                    return Forbid("Bạn không có quyền chỉnh sửa đơn xin phép này");
                }

                if (leaveRequest.Status != "pending")
                {
                    return BadRequest(new { success = false, message = "Không thể chỉnh sửa đơn xin phép đã được xử lý" });
                }

                leaveRequest.Type = request.Type;
                leaveRequest.StartDate = request.StartDate;
                leaveRequest.EndDate = request.EndDate;
                leaveRequest.TotalDays = request.TotalDays;
                leaveRequest.Reason = request.Reason;
                leaveRequest.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Cập nhật đơn xin phép thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật đơn xin phép", error = ex.Message });
            }
        }

        // DELETE: api/leave-requests/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!userId.HasValue)
                {
                    return Unauthorized(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var leaveRequest = await _context.LeaveRequests.FindAsync(id);
                if (leaveRequest == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy đơn xin phép" });
                }

                // Only the creator can delete their own request, and only if it's still pending
                if (leaveRequest.UserId != userId.Value)
                {
                    return Forbid("Bạn không có quyền xóa đơn xin phép này");
                }

                if (leaveRequest.Status != "pending")
                {
                    return BadRequest(new { success = false, message = "Không thể xóa đơn xin phép đã được xử lý" });
                }

                _context.LeaveRequests.Remove(leaveRequest);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Xóa đơn xin phép thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xóa đơn xin phép", error = ex.Message });
            }
        }

        // POST: api/leave-requests/{id}/approve
        [HttpPost("{id}/approve")]
        [AllowAnonymous]
        public async Task<IActionResult> ApproveLeaveRequest(int id, [FromBody] ApproveLeaveRequestDto request)
        {
            try
            {
                var leaveRequest = await _context.LeaveRequests.FindAsync(id);
                if (leaveRequest == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy đơn xin phép" });
                }

                if (leaveRequest.Status != "pending")
                {
                    return BadRequest(new { success = false, message = "Đơn xin phép đã được xử lý" });
                }

                // Use first user as admin for testing
                var adminUser = await _context.Users.FirstOrDefaultAsync();
                if (adminUser == null)
                {
                    return BadRequest(new { success = false, message = "Không tìm thấy admin trong hệ thống" });
                }

                leaveRequest.Status = "approved";
                leaveRequest.ApprovedBy = adminUser.Id;
                leaveRequest.ApprovedDate = DateTime.Now;
                leaveRequest.Comments = request.Comments;
                leaveRequest.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Duyệt đơn xin phép thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi duyệt đơn xin phép", error = ex.Message });
            }
        }

        // POST: api/leave-requests/{id}/reject
        [HttpPost("{id}/reject")]
        [AllowAnonymous]
        public async Task<IActionResult> RejectLeaveRequest(int id, [FromBody] RejectLeaveRequestDto request)
        {
            try
            {
                var leaveRequest = await _context.LeaveRequests.FindAsync(id);
                if (leaveRequest == null)
                {
                    return NotFound(new { success = false, message = "Không tìm thấy đơn xin phép" });
                }

                if (leaveRequest.Status != "pending")
                {
                    return BadRequest(new { success = false, message = "Đơn xin phép đã được xử lý" });
                }

                // Use first user as admin for testing
                var adminUser = await _context.Users.FirstOrDefaultAsync();
                if (adminUser == null)
                {
                    return BadRequest(new { success = false, message = "Không tìm thấy admin trong hệ thống" });
                }

                leaveRequest.Status = "rejected";
                leaveRequest.ApprovedBy = adminUser.Id;
                leaveRequest.ApprovedDate = DateTime.Now;
                leaveRequest.Comments = request.Comments;
                leaveRequest.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Từ chối đơn xin phép thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi từ chối đơn xin phép", error = ex.Message });
            }
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        private async Task<bool> IsAdminUser()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return false;

            var user = await _context.Users.FindAsync(userId.Value);
            return user?.Role == Models.Enums.UserRole.Admin;
        }
    }

    public class CreateLeaveRequestDto
    {
        [Required(ErrorMessage = "Loại nghỉ phép không được để trống")]
        [MaxLength(100, ErrorMessage = "Loại nghỉ phép không được vượt quá 100 ký tự")]
        public string Type { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime StartDate { get; set; }
        
        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        public DateTime EndDate { get; set; }
        
        [Range(1, 365, ErrorMessage = "Số ngày nghỉ phải từ 1 đến 365 ngày")]
        public int TotalDays { get; set; }
        
        [Required(ErrorMessage = "Lý do nghỉ phép không được để trống")]
        [MaxLength(500, ErrorMessage = "Lý do nghỉ phép không được vượt quá 500 ký tự")]
        public string Reason { get; set; } = string.Empty;
    }

    public class ApproveLeaveRequestDto
    {
        public string Comments { get; set; } = string.Empty;
    }

    public class RejectLeaveRequestDto
    {
        public string Comments { get; set; } = string.Empty;
    }

    public class UpdateLeaveRequestDto
    {
        public string Type { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
