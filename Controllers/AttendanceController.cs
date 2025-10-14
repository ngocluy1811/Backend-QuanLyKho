using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models;
using FertilizerWarehouseAPI.Models.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class CheckInRequest
{
    public int UserId { get; set; }
    public string? CheckInTime { get; set; }
    public string? Date { get; set; }
}

public class CheckOutRequest
{
    public int UserId { get; set; }
    public string? CheckOutTime { get; set; }
    public string? Date { get; set; }
}

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
                // Check if AttendanceRecords table exists and has data
                var totalRecords = await _context.AttendanceRecords.CountAsync();
                // Console.WriteLine($"Total attendance records: {totalRecords}"); // Commented out to reduce logs

                // If no records exist, return empty array
                if (totalRecords == 0)
                {
                    // Console.WriteLine("No attendance records found, returning empty array"); // Commented out to reduce logs
                    return Ok(new { success = true, data = new object[0], total = 0, message = "No attendance records found" });
                }

                var query = _context.AttendanceRecords.AsQueryable();
                
                // Filter by date if provided
                if (date.HasValue)
                {
                    // Ensure date is UTC to avoid PostgreSQL issues
                    var utcDate = DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc);
                    query = query.Where(a => a.Date.Date == utcDate.Date);
                }

                var records = await query
                    .Include(a => a.User)
                    .ThenInclude(u => u.Department)
                    .OrderBy(a => a.Date)
                    .ThenBy(a => a.UserId)
                    .Select(a => new
                    {
                        a.Id,
                        a.UserId,
                        EmployeeId = a.User != null ? a.User.Id.ToString() : "N/A",
                        EmployeeName = a.User != null ? (a.User.FullName ?? a.User.Username ?? "Nhân viên") : "Nhân viên",
                        Department = a.User != null && a.User.Department != null ? a.User.Department.Name : "Chưa phân công",
                        Position = a.User != null ? a.User.Role.ToString() : "Employee",
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

                // Console.WriteLine($"Found {records.Count} attendance records"); // Commented out to reduce logs
                return Ok(new { success = true, data = records, total = totalRecords });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy dữ liệu chấm công", error = ex.Message });
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

                Console.WriteLine($"Creating/updating attendance record for UserId: {request.UserId}");

                // Use UserId from request
                var userId = request.UserId;
                
                // Ensure date is UTC
                var targetDate = !string.IsNullOrEmpty(request.Date) 
                    ? DateTime.SpecifyKind(DateTime.Parse(request.Date).Date, DateTimeKind.Utc) 
                    : DateTime.UtcNow.Date;
                
                Console.WriteLine($"Target date: {targetDate:yyyy-MM-dd}");

                var existingRecord = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.Date.Date == targetDate.Date);

                if (existingRecord != null)
                {
                    Console.WriteLine("Updating existing record");
                    // Update existing record with UTC times
                    existingRecord.CheckInTime = !string.IsNullOrEmpty(request.CheckInTime) 
                        ? DateTime.SpecifyKind(DateTime.Parse(request.CheckInTime), DateTimeKind.Utc) 
                        : null;
                    existingRecord.CheckOutTime = !string.IsNullOrEmpty(request.CheckOutTime) 
                        ? DateTime.SpecifyKind(DateTime.Parse(request.CheckOutTime), DateTimeKind.Utc) 
                        : null;
                    existingRecord.OvertimeHours = request.OvertimeHours ?? 0;
                    existingRecord.OvertimeStartTime = !string.IsNullOrEmpty(request.OvertimeStartTime) 
                        ? DateTime.SpecifyKind(DateTime.Parse(request.OvertimeStartTime), DateTimeKind.Utc) 
                        : null;
                    existingRecord.OvertimeEndTime = !string.IsNullOrEmpty(request.OvertimeEndTime) 
                        ? DateTime.SpecifyKind(DateTime.Parse(request.OvertimeEndTime), DateTimeKind.Utc) 
                        : null;
                    existingRecord.Notes = request.Notes;
                    existingRecord.Status = request.Status;
                    existingRecord.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    Console.WriteLine("Creating new record");
                    // Create new record with UTC times
                    var newRecord = new AttendanceRecord
                    {
                        UserId = userId,
                        Date = targetDate,
                        CheckInTime = !string.IsNullOrEmpty(request.CheckInTime) 
                            ? DateTime.SpecifyKind(DateTime.Parse(request.CheckInTime), DateTimeKind.Utc) 
                            : null,
                        CheckOutTime = !string.IsNullOrEmpty(request.CheckOutTime) 
                            ? DateTime.SpecifyKind(DateTime.Parse(request.CheckOutTime), DateTimeKind.Utc) 
                            : null,
                        OvertimeHours = request.OvertimeHours ?? 0,
                        OvertimeStartTime = !string.IsNullOrEmpty(request.OvertimeStartTime) 
                            ? DateTime.SpecifyKind(DateTime.Parse(request.OvertimeStartTime), DateTimeKind.Utc) 
                            : null,
                        OvertimeEndTime = !string.IsNullOrEmpty(request.OvertimeEndTime) 
                            ? DateTime.SpecifyKind(DateTime.Parse(request.OvertimeEndTime), DateTimeKind.Utc) 
                            : null,
                        Notes = request.Notes,
                        Status = request.Status,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.AttendanceRecords.Add(newRecord);
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("Successfully saved attendance record");

                return Ok(new { success = true, message = "Lưu dữ liệu chấm công thành công" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAttendanceRecord: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Lỗi khi lưu dữ liệu chấm công", error = ex.Message });
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
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Request body cannot be null" });
                }

                Console.WriteLine($"Updating overtime status for UserId: {request.UserId}, IsOvertimeRequired: {request.IsOvertimeRequired}");
                
                var today = DateTime.UtcNow.Date;
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
                        Status = "present",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.AttendanceRecords.Add(newRecord);
                    Console.WriteLine($"Created new attendance record for UserId: {request.UserId}");
                }
                else
                {
                    // Update existing record
                    existingRecord.IsOvertimeRequired = request.IsOvertimeRequired;
                    existingRecord.UpdatedAt = DateTime.UtcNow;
                    Console.WriteLine($"Updated existing attendance record for UserId: {request.UserId}");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("Successfully saved attendance record");

                return Ok(new { success = true, message = "Cập nhật trạng thái tăng ca thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật trạng thái tăng ca", error = ex.Message });
            }
        }

        // POST: api/attendance/check-in
        [HttpPost("check-in")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Request body cannot be null" });
                }

                Console.WriteLine($"Check-in request for UserId: {request.UserId}");
                
                // Use provided date or default to today
                var targetDate = !string.IsNullOrEmpty(request.Date) 
                    ? DateTime.Parse(request.Date).Date 
                    : DateTime.UtcNow.Date;
                
                // Use provided time or default to current time
                var checkInTime = !string.IsNullOrEmpty(request.CheckInTime) 
                    ? DateTime.Parse($"{targetDate:yyyy-MM-dd} {request.CheckInTime}")
                    : DateTime.UtcNow;
                
                var existingRecord = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Date.Date == targetDate);

                if (existingRecord == null)
                {
                    // Create new record
                    var newRecord = new AttendanceRecord
                    {
                        UserId = request.UserId,
                        Date = targetDate,
                        CheckInTime = checkInTime,
                        Status = "present",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.AttendanceRecords.Add(newRecord);
                    Console.WriteLine($"Created new attendance record for UserId: {request.UserId} on {targetDate:yyyy-MM-dd}");
                }
                else
                {
                    // Update existing record with new time
                    existingRecord.CheckInTime = checkInTime;
                    existingRecord.UpdatedAt = DateTime.UtcNow;
                    Console.WriteLine($"Updated check-in time for UserId: {request.UserId} on {targetDate:yyyy-MM-dd} to {checkInTime:HH:mm}");
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("Successfully saved check-in record");

                return Ok(new { success = true, message = "Chấm công vào thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi chấm công vào", error = ex.Message });
            }
        }

        // POST: api/attendance/check-out
        [HttpPost("check-out")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Request body cannot be null" });
                }

                Console.WriteLine($"Check-out request for UserId: {request.UserId}");
                
                // Use provided date or default to today
                var targetDate = !string.IsNullOrEmpty(request.Date) 
                    ? DateTime.Parse(request.Date).Date 
                    : DateTime.UtcNow.Date;
                
                // Use provided time or default to current time
                var checkOutTime = !string.IsNullOrEmpty(request.CheckOutTime) 
                    ? DateTime.Parse($"{targetDate:yyyy-MM-dd} {request.CheckOutTime}")
                    : DateTime.UtcNow;
                
                var existingRecord = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Date.Date == targetDate);

                if (existingRecord == null)
                {
                    return BadRequest(new { success = false, message = $"Chưa chấm công vào ngày {targetDate:yyyy-MM-dd}, không thể chấm công ra" });
                }

                // Update existing record with new time
                existingRecord.CheckOutTime = checkOutTime;
                existingRecord.UpdatedAt = DateTime.UtcNow;
                Console.WriteLine($"Updated check-out time for UserId: {request.UserId} on {targetDate:yyyy-MM-dd} to {checkOutTime:HH:mm}");

                await _context.SaveChangesAsync();
                Console.WriteLine("Successfully saved check-out record");

                return Ok(new { success = true, message = "Chấm công ra thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi chấm công ra", error = ex.Message });
            }
        }

        // GET: api/attendance/overtime-status
        [HttpGet("overtime-status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOvertimeStatus([FromQuery] DateTime? date = null)
        {
            try
            {
                // Ensure date is UTC to avoid PostgreSQL issues
                var targetDate = date.HasValue ? DateTime.SpecifyKind(date.Value.Date, DateTimeKind.Utc) : DateTime.UtcNow.Date;
                // Console.WriteLine($"Getting overtime status for date: {targetDate:yyyy-MM-dd}"); // Commented out to reduce logs
                
                var records = await _context.AttendanceRecords
                    .Where(a => a.Date.Date == targetDate.Date && a.IsOvertimeRequired == true)
                    .OrderBy(a => a.UserId)
                    .Select(a => new
                    {
                        a.UserId,
                        EmployeeName = "Nhân viên", // Simplified to avoid null reference
                        a.IsOvertimeRequired,
                        Date = a.Date.ToString("yyyy-MM-dd")
                    })
                    .ToListAsync();

                // Console.WriteLine($"Found {records.Count} overtime records for {targetDate:yyyy-MM-dd}"); // Commented out to reduce logs
                return Ok(new { success = true, data = records, date = targetDate.ToString("yyyy-MM-dd") });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi lấy trạng thái tăng ca", error = ex.Message });
            }
        }

        // POST: api/attendance/create-sample
        [HttpPost("create-sample")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateSampleAttendance()
        {
            try
            {
                // Get first user for testing
                var user = await _context.Users.FirstOrDefaultAsync();
                if (user == null)
                {
                    return BadRequest(new { success = false, message = "No users found in database" });
                }

                var today = DateTime.UtcNow.Date;
                
                // Check if record already exists
                var existingRecord = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.UserId == user.Id && a.Date.Date == today);

                if (existingRecord != null)
                {
                    return Ok(new { success = true, message = "Sample attendance record already exists", data = new { UserId = user.Id, Date = today.ToString("yyyy-MM-dd") } });
                }

                // Create sample attendance record
                var sampleRecord = new AttendanceRecord
                {
                    UserId = user.Id,
                    Date = today,
                    CheckInTime = DateTime.SpecifyKind(today.AddHours(8), DateTimeKind.Utc),
                    CheckOutTime = DateTime.SpecifyKind(today.AddHours(17), DateTimeKind.Utc),
                    OvertimeHours = 2.0m,
                    OvertimeStartTime = DateTime.SpecifyKind(today.AddHours(17), DateTimeKind.Utc),
                    OvertimeEndTime = DateTime.SpecifyKind(today.AddHours(19), DateTimeKind.Utc),
                    IsOvertimeRequired = true,
                    Status = "present",
                    Notes = "Sample attendance record",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.AttendanceRecords.Add(sampleRecord);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Created sample attendance record for user {user.Id} on {today:yyyy-MM-dd}");
                return Ok(new { success = true, message = "Sample attendance record created successfully", data = new { UserId = user.Id, Date = today.ToString("yyyy-MM-dd") } });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi tạo dữ liệu mẫu", error = ex.Message });
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