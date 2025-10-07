using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models;
using FertilizerWarehouseAPI.Models.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/attendance/records
        [HttpGet("records")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAttendanceRecords([FromQuery] DateTime? date = null)
        {
            try
            {
                var query = _context.AttendanceRecords.AsQueryable();
                
                // Filter by date if provided
                if (date.HasValue)
                {
                    query = query.Where(a => a.Date.Date == date.Value.Date);
                }

                var records = await query
                    .Include(a => a.User)
                    .ThenInclude(u => u.Department)
                    .Select(a => new
                    {
                        a.Id,
                        a.UserId,
                        EmployeeId = a.User.Username,
                        EmployeeName = a.User.FullName,
                        Department = a.User.Department != null ? a.User.Department.Name : "N/A",
                        Position = a.User.Role.ToString(),
                        Date = a.Date.ToString("yyyy-MM-dd"),
                        CheckIn = a.CheckInTime.HasValue ? a.CheckInTime.Value.ToString("HH:mm") : null,
                        CheckOut = a.CheckOutTime.HasValue ? a.CheckOutTime.Value.ToString("HH:mm") : null,
                        Overtime = a.OvertimeHours ?? 0,
                        OvertimeStartTime = a.OvertimeStartTime.HasValue ? a.OvertimeStartTime.Value.ToString("HH:mm") : null,
                        OvertimeEndTime = a.OvertimeEndTime.HasValue ? a.OvertimeEndTime.Value.ToString("HH:mm") : null,
                        Status = a.Status ?? "present", // Use actual status from database
                        Notes = a.Notes ?? ""
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = records });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy dữ liệu chấm công", error = ex.Message });
            }
        }

        // POST: api/attendance/check-in
        [HttpPost("check-in")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            try
            {
                // Use userId from request
                var userId = request.UserId;
                
                // Use selected date or today
                var targetDate = !string.IsNullOrEmpty(request.Date) ? DateTime.Parse(request.Date) : DateTime.Today;
                var existingRecord = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.Date.Date == targetDate.Date);

                if (existingRecord != null && existingRecord.CheckInTime.HasValue)
                {
                    return BadRequest(new { success = false, message = $"Bạn đã chấm công vào ngày {targetDate:dd/MM/yyyy}" });
                }

                var checkInTime = DateTime.Parse(request.CheckInTime);
                var currentTime = DateTime.Now;

                if (existingRecord == null)
                {
                    var newRecord = new AttendanceRecord
                    {
                        UserId = userId,
                        Date = targetDate,
                        CheckInTime = checkInTime,
                        CreatedAt = currentTime
                    };
                    _context.AttendanceRecords.Add(newRecord);
                }
                else
                {
                    existingRecord.CheckInTime = checkInTime;
                    existingRecord.UpdatedAt = currentTime;
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = $"Chấm công vào thành công ngày {targetDate:dd/MM/yyyy}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi chấm công vào", error = ex.Message });
            }
        }

        // POST: api/attendance/records
        [HttpPost("records")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAttendanceRecord([FromBody] CreateAttendanceRecordRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                // Use UserId from request
                var userId = request.UserId;
                var targetDate = !string.IsNullOrEmpty(request.Date) ? DateTime.Parse(request.Date) : DateTime.Today;
                var existingRecord = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.Date.Date == targetDate.Date);

                if (existingRecord != null)
                {
                    // Update existing record
                    existingRecord.CheckInTime = !string.IsNullOrEmpty(request.CheckInTime) ? DateTime.Parse(request.CheckInTime) : null;
                    existingRecord.CheckOutTime = !string.IsNullOrEmpty(request.CheckOutTime) ? DateTime.Parse(request.CheckOutTime) : null;
                    existingRecord.OvertimeHours = request.OvertimeHours ?? 0;
                    existingRecord.OvertimeStartTime = !string.IsNullOrEmpty(request.OvertimeStartTime) ? DateTime.Parse(request.OvertimeStartTime) : null;
                    existingRecord.OvertimeEndTime = !string.IsNullOrEmpty(request.OvertimeEndTime) ? DateTime.Parse(request.OvertimeEndTime) : null;
                    existingRecord.Notes = request.Notes;
                    existingRecord.Status = request.Status;
                    existingRecord.UpdatedAt = DateTime.Now;
                }
                else
                {
                    // Create new record
                    var newRecord = new AttendanceRecord
                    {
                        UserId = userId,
                        Date = targetDate,
                        CheckInTime = !string.IsNullOrEmpty(request.CheckInTime) ? DateTime.Parse(request.CheckInTime) : null,
                        CheckOutTime = !string.IsNullOrEmpty(request.CheckOutTime) ? DateTime.Parse(request.CheckOutTime) : null,
                        OvertimeHours = request.OvertimeHours ?? 0,
                        OvertimeStartTime = !string.IsNullOrEmpty(request.OvertimeStartTime) ? DateTime.Parse(request.OvertimeStartTime) : null,
                        OvertimeEndTime = !string.IsNullOrEmpty(request.OvertimeEndTime) ? DateTime.Parse(request.OvertimeEndTime) : null,
                        Notes = request.Notes,
                        Status = request.Status,
                        CreatedAt = DateTime.Now
                    };
                    _context.AttendanceRecords.Add(newRecord);
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Lưu dữ liệu chấm công thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lưu dữ liệu chấm công", error = ex.Message });
            }
        }

        // POST: api/attendance/check-out
        [HttpPost("check-out")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
        {
            try
            {
                // Use userId from request
                var userId = request.UserId;
                
                // Use selected date or today
                var targetDate = !string.IsNullOrEmpty(request.Date) ? DateTime.Parse(request.Date) : DateTime.Today;
                var existingRecord = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.Date.Date == targetDate.Date);

                if (existingRecord == null)
                {
                    return BadRequest(new { success = false, message = $"Bạn chưa chấm công vào ngày {targetDate:dd/MM/yyyy}" });
                }

                if (existingRecord.CheckOutTime.HasValue)
                {
                    return BadRequest(new { success = false, message = $"Bạn đã chấm công ra ngày {targetDate:dd/MM/yyyy}" });
                }

                var checkOutTime = DateTime.Parse(request.CheckOutTime);
                var currentTime = DateTime.Now;

                existingRecord.CheckOutTime = checkOutTime;
                existingRecord.UpdatedAt = currentTime;

                // Calculate overtime if needed
                if (checkOutTime.Hour >= 17) // After 5 PM
                {
                    var overtimeStart = new DateTime(targetDate.Year, targetDate.Month, targetDate.Day, 17, 0, 0);
                    var overtimeHours = (checkOutTime - overtimeStart).TotalHours;
                    if (overtimeHours > 0)
                    {
                        existingRecord.OvertimeHours = (decimal)overtimeHours;
                        existingRecord.OvertimeStartTime = overtimeStart;
                        existingRecord.OvertimeEndTime = checkOutTime;
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = $"Chấm công ra thành công ngày {targetDate:dd/MM/yyyy}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi chấm công ra", error = ex.Message });
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

        private string GetAttendanceStatus(DateTime? checkIn, DateTime? checkOut)
        {
            if (!checkIn.HasValue)
                return "absent";

            if (checkIn.Value.Hour > 8 || (checkIn.Value.Hour == 8 && checkIn.Value.Minute > 0))
                return "late";

            if (!checkOut.HasValue)
                return "present";

            return "present";
        }

        // POST: api/attendance/overtime-status
        [HttpPost("overtime-status")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateOvertimeStatus([FromBody] UpdateOvertimeStatusRequest request)
        {
            try
            {
                var today = DateTime.Today;
                var existingRecord = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Date.Date == today);

                if (existingRecord == null)
                {
                    // Create new record if doesn't exist
                    var newRecord = new AttendanceRecord
                    {
                        UserId = request.UserId,
                        Date = today,
                        IsOvertimeRequired = request.IsOvertimeRequired,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.AttendanceRecords.Add(newRecord);
                }
                else
                {
                    // Update existing record
                    existingRecord.IsOvertimeRequired = request.IsOvertimeRequired;
                    existingRecord.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Cập nhật trạng thái tăng ca thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật trạng thái tăng ca", error = ex.Message });
            }
        }

        // GET: api/attendance/overtime-status
        [HttpGet("overtime-status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOvertimeStatus([FromQuery] DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Today;
                var records = await _context.AttendanceRecords
                    .Where(a => a.Date.Date == targetDate.Date && a.IsOvertimeRequired == true)
                    .Include(a => a.User)
                    .Select(a => new
                    {
                        a.UserId,
                        EmployeeName = a.User.FullName,
                        a.IsOvertimeRequired,
                        a.Date
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = records });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy trạng thái tăng ca", error = ex.Message });
            }
        }
    }

    public class CheckInRequest
    {
        public int UserId { get; set; }
        public string CheckInTime { get; set; } = string.Empty;
        public string? Date { get; set; }
    }

    public class CheckOutRequest
    {
        public int UserId { get; set; }
        public string CheckOutTime { get; set; } = string.Empty;
        public string? Date { get; set; }
    }

    public class CreateAttendanceRecordRequest
    {
        public int UserId { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public decimal? OvertimeHours { get; set; }
        public string? OvertimeStartTime { get; set; }
        public string? OvertimeEndTime { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public string? Date { get; set; } // Add date field
    }


    public class UpdateOvertimeStatusRequest
    {
        public int UserId { get; set; }
        public bool IsOvertimeRequired { get; set; }
    }
}