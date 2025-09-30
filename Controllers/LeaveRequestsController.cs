using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FertilizerWarehouseAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using System.Security.Claims;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get leave requests with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestDto>>> GetLeaveRequests(
            [FromQuery] string? status = null,
            [FromQuery] string? type = null,
            [FromQuery] int? employeeId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var isManager = User.IsInRole("Admin") || User.IsInRole("Manager");

                // Mock data for demonstration
                var mockRequests = new List<LeaveRequestDto>
                {
                    new LeaveRequestDto
                    {
                        Id = 1,
                        EmployeeId = currentUserId,
                        EmployeeName = "Current User",
                        Department = "IT",
                        Position = "Developer",
                        Type = "annual",
                        StartDate = DateTime.Today.AddDays(7),
                        EndDate = DateTime.Today.AddDays(9),
                        TotalDays = 3,
                        Reason = "Personal vacation",
                        Status = "pending",
                        RequestDate = DateTime.Today,
                        CreatedAt = DateTime.UtcNow
                    },
                    new LeaveRequestDto
                    {
                        Id = 2,
                        EmployeeId = currentUserId,
                        EmployeeName = "Current User",
                        Department = "IT",
                        Position = "Developer",
                        Type = "sick",
                        StartDate = DateTime.Today.AddDays(-5),
                        EndDate = DateTime.Today.AddDays(-3),
                        TotalDays = 3,
                        Reason = "Medical treatment",
                        Status = "approved",
                        RequestDate = DateTime.Today.AddDays(-7),
                        ApprovedBy = 1,
                        ApprovedByName = "Manager",
                        ApprovedDate = DateTime.Today.AddDays(-6),
                        Comments = "Approved with medical certificate",
                        CreatedAt = DateTime.UtcNow.AddDays(-7)
                    }
                };

                // If not manager, only show current user's requests
                if (!isManager)
                {
                    mockRequests = mockRequests.Where(r => r.EmployeeId == currentUserId).ToList();
                }

                // Apply filters
                if (!string.IsNullOrEmpty(status))
                {
                    mockRequests = mockRequests.Where(r => r.Status == status).ToList();
                }

                if (!string.IsNullOrEmpty(type))
                {
                    mockRequests = mockRequests.Where(r => r.Type == type).ToList();
                }

                if (employeeId.HasValue)
                {
                    mockRequests = mockRequests.Where(r => r.EmployeeId == employeeId.Value).ToList();
                }

                // Apply pagination
                var pagedRequests = mockRequests
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(pagedRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get leave request by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequestById(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var isManager = User.IsInRole("Admin") || User.IsInRole("Manager");

                // Mock data
                var request = new LeaveRequestDto
                {
                    Id = id,
                    EmployeeId = currentUserId,
                    EmployeeName = "Current User",
                    Department = "IT",
                    Position = "Developer",
                    Type = "annual",
                    StartDate = DateTime.Today.AddDays(7),
                    EndDate = DateTime.Today.AddDays(9),
                    TotalDays = 3,
                    Reason = "Personal vacation",
                    Status = "pending",
                    RequestDate = DateTime.Today,
                    CreatedAt = DateTime.UtcNow
                };

                // Check permission
                if (!isManager && request.EmployeeId != currentUserId)
                {
                    return Forbid("You can only view your own leave requests");
                }

                return Ok(request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new leave request
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest([FromBody] CreateLeaveRequestDto createDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                // Verify employee exists
                var employee = await _context.Users
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Id == createDto.EmployeeId);

                if (employee == null)
                {
                    return NotFound(new { message = "Employee not found" });
                }

                // Check permission - users can only create requests for themselves unless they're managers
                var isManager = User.IsInRole("Admin") || User.IsInRole("Manager");
                if (!isManager && createDto.EmployeeId != currentUserId)
                {
                    return Forbid("You can only create leave requests for yourself");
                }

                // Validate dates
                if (createDto.StartDate > createDto.EndDate)
                {
                    return BadRequest(new { message = "Start date cannot be after end date" });
                }

                if (createDto.StartDate < DateTime.Today)
                {
                    return BadRequest(new { message = "Start date cannot be in the past" });
                }

                // Mock creation
                var leaveRequestDto = new LeaveRequestDto
                {
                    Id = new Random().Next(1000, 9999),
                    EmployeeId = employee.Id,
                    EmployeeName = employee.FullName ?? employee.Username,
                    Department = employee.Department?.Name ?? "N/A",
                    Position = employee.Role.ToString(),
                    Type = createDto.Type,
                    StartDate = createDto.StartDate,
                    EndDate = createDto.EndDate,
                    TotalDays = createDto.TotalDays,
                    Reason = createDto.Reason,
                    Status = "pending",
                    RequestDate = DateTime.Today,
                    Comments = createDto.Comments,
                    CreatedAt = DateTime.UtcNow
                };

                return CreatedAtAction(nameof(GetLeaveRequestById), new { id = leaveRequestDto.Id }, leaveRequestDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update leave request (only for pending requests)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<LeaveRequestDto>> UpdateLeaveRequest(int id, [FromBody] UpdateLeaveRequestDto updateDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var isManager = User.IsInRole("Admin") || User.IsInRole("Manager");

                // Mock validation - in real implementation, fetch from database
                // For now, assume user can edit their own pending requests
                
                var updatedRequest = new LeaveRequestDto
                {
                    Id = id,
                    EmployeeId = currentUserId,
                    EmployeeName = "Current User",
                    Department = "IT",
                    Position = "Developer",
                    Type = updateDto.Type ?? "annual",
                    StartDate = updateDto.StartDate ?? DateTime.Today.AddDays(7),
                    EndDate = updateDto.EndDate ?? DateTime.Today.AddDays(9),
                    TotalDays = updateDto.TotalDays ?? 3,
                    Reason = updateDto.Reason ?? "Updated reason",
                    Status = "pending",
                    RequestDate = DateTime.Today,
                    Comments = updateDto.Comments,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(updatedRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Approve or reject leave request
        /// </summary>
        [HttpPost("{id}/approve")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<LeaveRequestDto>> ApproveLeaveRequest(int id, [FromBody] ApproveLeaveRequestDto approvalDto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

                if (approvalDto.Status != "approved" && approvalDto.Status != "rejected")
                {
                    return BadRequest(new { message = "Status must be 'approved' or 'rejected'" });
                }

                // Mock approval
                var approvedRequest = new LeaveRequestDto
                {
                    Id = id,
                    EmployeeId = 1,
                    EmployeeName = "Employee Name",
                    Department = "IT",
                    Position = "Developer",
                    Type = "annual",
                    StartDate = DateTime.Today.AddDays(7),
                    EndDate = DateTime.Today.AddDays(9),
                    TotalDays = 3,
                    Reason = "Personal vacation",
                    Status = approvalDto.Status,
                    RequestDate = DateTime.Today.AddDays(-1),
                    ApprovedBy = currentUserId,
                    ApprovedByName = "Current Manager",
                    ApprovedDate = DateTime.Today,
                    Comments = approvalDto.Comments,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(approvedRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete leave request (only for pending requests)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLeaveRequest(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var isManager = User.IsInRole("Admin") || User.IsInRole("Manager");

                // Mock validation and deletion
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get employee leave balance
        /// </summary>
        [HttpGet("balance/{employeeId}")]
        public async Task<ActionResult<LeaveBalanceDto>> GetLeaveBalance(int employeeId, [FromQuery] int year = 0)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var isManager = User.IsInRole("Admin") || User.IsInRole("Manager");

                // Check permission
                if (!isManager && employeeId != currentUserId)
                {
                    return Forbid("You can only view your own leave balance");
                }

                if (year == 0)
                    year = DateTime.Now.Year;

                // Mock balance data
                var balance = new LeaveBalanceDto
                {
                    EmployeeId = employeeId,
                    EmployeeName = "Employee Name",
                    TotalAnnualLeave = 12,
                    UsedAnnualLeave = 5,
                    RemainingAnnualLeave = 7,
                    SickLeaveUsed = 2,
                    PersonalLeaveUsed = 1,
                    Year = year
                };

                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get leave statistics
        /// </summary>
        [HttpGet("stats")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult<LeaveStatsDto>> GetLeaveStats(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.Today.AddMonths(-1);
                var to = toDate ?? DateTime.Today;

                // Mock statistics
                var stats = new LeaveStatsDto
                {
                    TotalRequests = 25,
                    PendingRequests = 3,
                    ApprovedRequests = 20,
                    RejectedRequests = 2,
                    TotalDaysRequested = 150,
                    TotalDaysApproved = 120,
                    FromDate = from,
                    ToDate = to
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Export leave requests to Excel
        /// </summary>
        [HttpGet("export")]
        [Authorize(Policy = "Management")]
        public async Task<ActionResult> ExportLeaveRequests(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? status = null,
            [FromQuery] string format = "xlsx")
        {
            try
            {
                // Mock file content
                var fileName = $"leave_requests_{DateTime.Now:yyyyMMdd}.{format}";
                var content = System.Text.Encoding.UTF8.GetBytes("Mock leave requests export data");

                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }
}
