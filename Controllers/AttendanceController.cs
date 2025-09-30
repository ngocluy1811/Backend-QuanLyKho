using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get attendance records with filtering and pagination
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<IEnumerable<AttendanceDto>>> GetAttendance(
            [FromQuery] DateTime? date = null,
            [FromQuery] string? department = null,
            [FromQuery] string? status = null,
            [FromQuery] int? employeeId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                // Apply filters
                if (date.HasValue)
                {
                    // Filter by date logic would go here
                    // This is a simplified example
                }

                if (!string.IsNullOrEmpty(department))
                {
                    query = query.Where(u => u.Department!.Name == department);
                }

                if (employeeId.HasValue)
                {
                    query = query.Where(u => u.Id == employeeId.Value);
                }

                // For now, return mock data based on users
                var users = await query
                    .Include(u => u.Department)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var attendanceRecords = users.Select(user => new AttendanceDto
                {
                    Id = user.Id,
                    EmployeeId = user.Id,
                    EmployeeName = user.FullName ?? user.Username,
                    Department = user.Department?.Name ?? "N/A",
                    Position = user.Role.ToString(),
                    Date = date ?? DateTime.Today,
                    CheckIn = DateTime.Today.Add(TimeSpan.FromHours(8)),
                    CheckOut = DateTime.Today.Add(TimeSpan.FromHours(17)),
                    Status = "present",
                    OvertimeHours = TimeSpan.Zero,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                return Ok(attendanceRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get attendance statistics for a specific date
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<AttendanceStatsDto>> GetAttendanceStats([FromQuery] DateTime? date = null)
        {
            try
            {
                var targetDate = date ?? DateTime.Today;
                var totalEmployees = await _context.Users.Where(u => u.IsActive).CountAsync();

                // Mock statistics
                var stats = new AttendanceStatsDto
                {
                    Date = targetDate,
                    TotalEmployees = totalEmployees,
                    PresentCount = (int)(totalEmployees * 0.9),
                    AbsentCount = (int)(totalEmployees * 0.05),
                    LateCount = (int)(totalEmployees * 0.03),
                    LeaveCount = (int)(totalEmployees * 0.02),
                    AttendanceRate = 90.0m
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new attendance record
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<AttendanceDto>> CreateAttendance([FromBody] CreateAttendanceDto createDto)
        {
            try
            {
                // Verify employee exists
                var employee = await _context.Users
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Id == createDto.EmployeeId);

                if (employee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                // For now, return mock data
                var attendanceDto = new AttendanceDto
                {
                    Id = new Random().Next(1000, 9999),
                    EmployeeId = employee.Id,
                    EmployeeName = employee.FullName ?? employee.Username,
                    Department = employee.Department?.Name ?? "N/A",
                    Position = employee.Role.ToString(),
                    Date = createDto.Date,
                    CheckIn = createDto.CheckIn,
                    CheckOut = createDto.CheckOut,
                    Status = createDto.Status,
                    OvertimeHours = createDto.OvertimeHours,
                    OvertimeStart = createDto.OvertimeStart,
                    OvertimeEnd = createDto.OvertimeEnd,
                    Notes = createDto.Notes,
                    CreatedAt = DateTime.UtcNow
                };

                return CreatedAtAction(nameof(GetAttendanceById), new { id = attendanceDto.Id }, attendanceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get attendance record by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<AttendanceDto>> GetAttendanceById(int id)
        {
            try
            {
                // For now, return mock data
                var attendanceDto = new AttendanceDto
                {
                    Id = id,
                    EmployeeId = 1,
                    EmployeeName = "Sample Employee",
                    Department = "Sample Department",
                    Position = "Sample Position",
                    Date = DateTime.Today,
                    CheckIn = DateTime.Today.Add(TimeSpan.FromHours(8)),
                    CheckOut = DateTime.Today.Add(TimeSpan.FromHours(17)),
                    Status = "present",
                    OvertimeHours = TimeSpan.Zero,
                    CreatedAt = DateTime.UtcNow
                };

                return Ok(attendanceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update attendance record
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<AttendanceDto>> UpdateAttendance(int id, [FromBody] UpdateAttendanceDto updateDto)
        {
            try
            {
                // For now, return mock updated data
                var attendanceDto = new AttendanceDto
                {
                    Id = id,
                    EmployeeId = 1,
                    EmployeeName = "Sample Employee",
                    Department = "Sample Department",
                    Position = "Sample Position",
                    Date = updateDto.Date ?? DateTime.Today,
                    CheckIn = updateDto.CheckIn ?? DateTime.Today.Add(TimeSpan.FromHours(8)),
                    CheckOut = updateDto.CheckOut ?? DateTime.Today.Add(TimeSpan.FromHours(17)),
                    Status = updateDto.Status ?? "present",
                    OvertimeHours = updateDto.OvertimeHours ?? TimeSpan.Zero,
                    OvertimeStart = updateDto.OvertimeStart,
                    OvertimeEnd = updateDto.OvertimeEnd,
                    Notes = updateDto.Notes,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(attendanceDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete attendance record
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteAttendance(int id)
        {
            try
            {
                // Mock deletion
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Bulk create/update attendance records
        /// </summary>
        [HttpPost("bulk")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult> BulkCreateAttendance([FromBody] BulkAttendanceDto bulkDto)
        {
            try
            {
                // Mock bulk operation
                return Ok(new { message = $"Successfully processed {bulkDto.Records.Count} attendance records" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Export attendance records to Excel
        /// </summary>
        [HttpGet("export")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult> ExportAttendance(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? department = null,
            [FromQuery] string format = "xlsx")
        {
            try
            {
                // Mock file content
                var fileName = $"attendance_report_{DateTime.Now:yyyyMMdd}.{format}";
                var content = System.Text.Encoding.UTF8.GetBytes("Mock attendance export data");

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}
