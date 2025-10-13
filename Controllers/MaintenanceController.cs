using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/maintenance
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _context.Users
                    .Where(u => u.IsActive)
                    .Select(u => new
                    {
                        id = u.Id,
                        username = u.Username,
                        fullName = u.FullName,
                        email = u.Email,
                        role = u.Role
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = employees });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách nhân viên", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMaintenanceRequests()
        {
            try
            {
                var requests = await _context.MaintenanceRequests
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => new {
                        id = m.Id,
                        title = m.Title,
                        description = m.Description,
                        location = m.Location,
                        maintenanceType = m.MaintenanceType,
                        priority = m.Priority,
                        assignedTo = m.AssignedTo,
                        scheduledDate = m.ScheduledDate,
                        estimatedDuration = m.EstimatedDuration,
                        status = m.Status,
                        progress = m.Progress,
                        notes = m.Notes,
                        warehouseId = m.WarehouseId,
                        warehouseCellId = m.WarehouseCellId,
                        createdBy = m.CreatedBy,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        completedAt = m.CompletedAt
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    data = requests 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy danh sách yêu cầu bảo trì", 
                    error = ex.Message 
                });
            }
        }

        // PUT: api/maintenance/{id}/progress
        [HttpPut("{id}/progress")]
        public async Task<IActionResult> UpdateMaintenanceProgress(int id, [FromBody] UpdateMaintenanceProgressDto dto)
        {
            try
            {
                var request = await _context.MaintenanceRequests.FindAsync(id);
                if (request == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy yêu cầu bảo trì" 
                    });
                }

                request.Progress = dto.Progress;
                request.UpdatedAt = DateTime.UtcNow;
                request.UpdatedBy = dto.UpdatedBy ?? "System";

                if (dto.Progress >= 100)
                {
                    request.Status = "completed";
                    request.CompletedAt = DateTime.UtcNow;
                }
                else if (dto.Progress > 0)
                {
                    request.Status = "in_progress";
                }

                await _context.SaveChangesAsync();

                // Add to history
                var history = new Models.Entities.MaintenanceHistory
                {
                    MaintenanceRequestId = request.Id,
                    Action = "progress_updated",
                    Description = $"Cập nhật tiến độ: {dto.Progress}%",
                    Progress = dto.Progress,
                    Notes = dto.Notes,
                    CreatedBy = dto.UpdatedBy ?? "System",
                    CreatedAt = DateTime.UtcNow
                };

                _context.MaintenanceHistories.Add(history);
                await _context.SaveChangesAsync();

                // Add to warehouse activity log
                var activity = new Models.Entities.WarehouseActivity
                {
                    WarehouseId = request.WarehouseId,
                    CellId = request.WarehouseCellId,
                    ActivityType = "Maintenance",
                    Description = $"Cập nhật tiến độ bảo trì: {request.Title}",
                    ProductName = "",
                    BatchNumber = "",
                    Quantity = 0,
                    Unit = "",
                    UserName = dto.UpdatedBy ?? "System",
                    UserId = null,
                    Timestamp = DateTime.UtcNow,
                    Notes = $"Tiến độ: {dto.Progress}%, Ghi chú: {dto.Notes ?? "Không có"}",
                    Status = "Completed",
                    IsActive = true
                };

                _context.WarehouseActivities.Add(activity);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Cập nhật tiến độ thành công" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi cập nhật tiến độ", 
                    error = ex.Message 
                });
            }
        }

        // PUT: api/maintenance/{id}/complete
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> CompleteMaintenance(int id, [FromBody] CompleteMaintenanceDto dto)
        {
            try
            {
                var request = await _context.MaintenanceRequests.FindAsync(id);
                if (request == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy yêu cầu bảo trì" 
                    });
                }

                request.Status = "completed";
                request.Progress = 100;
                request.CompletedAt = DateTime.UtcNow;
                request.UpdatedAt = DateTime.UtcNow;
                request.UpdatedBy = dto.CompletedBy ?? "System";
                request.Notes = dto.Notes ?? request.Notes;

                await _context.SaveChangesAsync();

                // Add to history
                var history = new Models.Entities.MaintenanceHistory
                {
                    MaintenanceRequestId = request.Id,
                    Action = "completed",
                    Description = "Hoàn thành bảo trì",
                    Progress = 100,
                    Notes = dto.Notes,
                    CreatedBy = dto.CompletedBy ?? "System",
                    CreatedAt = DateTime.UtcNow
                };

                _context.MaintenanceHistories.Add(history);
                await _context.SaveChangesAsync();

                // Add to warehouse activity log
                var activity = new Models.Entities.WarehouseActivity
                {
                    WarehouseId = request.WarehouseId,
                    CellId = request.WarehouseCellId,
                    ActivityType = "Maintenance",
                    Description = $"Hoàn thành bảo trì: {request.Title}",
                    ProductName = "",
                    BatchNumber = "",
                    Quantity = 0,
                    Unit = "",
                    UserName = dto.CompletedBy ?? "System",
                    UserId = null,
                    Timestamp = DateTime.UtcNow,
                    Notes = $"Tiến độ: 100%, Ghi chú: {dto.Notes ?? "Không có"}",
                    Status = "Completed",
                    IsActive = true
                };

                _context.WarehouseActivities.Add(activity);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Hoàn thành bảo trì thành công" 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi hoàn thành bảo trì", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/maintenance/cells/bulk
        [HttpGet("cells/bulk")]
        public async Task<IActionResult> GetMaintenanceByCells([FromQuery] string cellIds)
        {
            try
            {
                if (string.IsNullOrEmpty(cellIds))
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Danh sách cell IDs không được để trống" 
                    });
                }

                var cellIdList = cellIds.Split(',').Select(id => int.TryParse(id.Trim(), out var cellId) ? cellId : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (!cellIdList.Any())
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Không có cell ID hợp lệ" 
                    });
                }

                var requests = await _context.MaintenanceRequests
                    .Where(m => m.WarehouseCellId.HasValue && cellIdList.Contains(m.WarehouseCellId.Value))
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => new {
                        id = m.Id,
                        title = m.Title,
                        description = m.Description,
                        location = m.Location,
                        maintenanceType = m.MaintenanceType,
                        priority = m.Priority,
                        assignedTo = m.AssignedTo,
                        scheduledDate = m.ScheduledDate,
                        estimatedDuration = m.EstimatedDuration,
                        status = m.Status,
                        progress = m.Progress,
                        notes = m.Notes,
                        warehouseId = m.WarehouseId,
                        warehouseCellId = m.WarehouseCellId,
                        createdBy = m.CreatedBy,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        completedAt = m.CompletedAt
                    })
                    .ToListAsync();

                // Group by cellId and find active maintenance for each cell
                var result = new Dictionary<int, object>();
                foreach (var cellId in cellIdList)
                {
                    var cellRequests = requests.Where(r => r.warehouseCellId == cellId).ToList();
                    var activeMaintenance = cellRequests.FirstOrDefault(r => 
                        r.status == "in_progress" || r.status == "pending"
                    );
                    
                    result[cellId] = activeMaintenance ?? null;
                }

                return Ok(new { 
                    success = true, 
                    data = result 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy yêu cầu bảo trì theo danh sách ô", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/maintenance/cell/{cellId}
        [HttpGet("cell/{cellId}")]
        public async Task<IActionResult> GetMaintenanceByCell(int cellId)
        {
            try
            {
                var requests = await _context.MaintenanceRequests
                    .Where(m => m.WarehouseCellId == cellId)
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => new {
                        id = m.Id,
                        title = m.Title,
                        description = m.Description,
                        location = m.Location,
                        maintenanceType = m.MaintenanceType,
                        priority = m.Priority,
                        assignedTo = m.AssignedTo,
                        scheduledDate = m.ScheduledDate,
                        estimatedDuration = m.EstimatedDuration,
                        status = m.Status,
                        progress = m.Progress,
                        notes = m.Notes,
                        warehouseId = m.WarehouseId,
                        warehouseCellId = m.WarehouseCellId,
                        createdBy = m.CreatedBy,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        completedAt = m.CompletedAt
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    data = requests 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy danh sách bảo trì theo cell", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/maintenance/{id}/history
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetMaintenanceHistory(int id)
        {
            try
            {
                var histories = await _context.MaintenanceHistories
                    .Where(h => h.MaintenanceRequestId == id)
                    .OrderBy(h => h.CreatedAt)
                    .Select(h => new {
                        id = h.Id,
                        action = h.Action,
                        description = h.Description,
                        progress = h.Progress,
                        notes = h.Notes,
                        createdBy = h.CreatedBy,
                        createdAt = h.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    data = histories 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy lịch sử bảo trì", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/maintenance/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceRequest(int id)
        {
            try
            {
                var request = await _context.MaintenanceRequests
                    .Where(m => m.Id == id)
                    .Select(m => new {
                        id = m.Id,
                        title = m.Title,
                        description = m.Description,
                        location = m.Location,
                        maintenanceType = m.MaintenanceType,
                        priority = m.Priority,
                        assignedTo = m.AssignedTo,
                        scheduledDate = m.ScheduledDate,
                        estimatedDuration = m.EstimatedDuration,
                        status = m.Status,
                        progress = m.Progress,
                        notes = m.Notes,
                        warehouseId = m.WarehouseId,
                        warehouseCellId = m.WarehouseCellId,
                        createdBy = m.CreatedBy,
                        createdAt = m.CreatedAt,
                        updatedAt = m.UpdatedAt,
                        completedAt = m.CompletedAt
                    })
                    .FirstOrDefaultAsync();

                if (request == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy yêu cầu bảo trì" 
                    });
                }

                return Ok(new { 
                    success = true, 
                    data = request 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi lấy thông tin yêu cầu bảo trì", 
                    error = ex.Message 
                });
            }
        }

        // POST: api/maintenance
        [HttpPost]
        public async Task<IActionResult> CreateMaintenanceRequest([FromBody] CreateMaintenanceRequestDto dto)
        {
            try
            {
                Console.WriteLine($"🔧 Creating maintenance request with data: {System.Text.Json.JsonSerializer.Serialize(dto)}");

                // Validate required fields
                if (string.IsNullOrEmpty(dto.Title))
                {
                    Console.WriteLine("❌ Validation failed: Title is required");
                    return BadRequest(new { 
                        success = false, 
                        message = "Tiêu đề là bắt buộc" 
                    });
                }

                if (string.IsNullOrEmpty(dto.Location))
                {
                    Console.WriteLine("❌ Validation failed: Location is required");
                    return BadRequest(new { 
                        success = false, 
                        message = "Vị trí là bắt buộc" 
                    });
                }

                if (string.IsNullOrEmpty(dto.MaintenanceType))
                {
                    Console.WriteLine("❌ Validation failed: MaintenanceType is required");
                    return BadRequest(new { 
                        success = false, 
                        message = "Loại bảo trì là bắt buộc" 
                    });
                }

                if (string.IsNullOrEmpty(dto.Priority))
                {
                    Console.WriteLine("❌ Validation failed: Priority is required");
                    return BadRequest(new { 
                        success = false, 
                        message = "Mức độ ưu tiên là bắt buộc" 
                    });
                }

                if (string.IsNullOrEmpty(dto.AssignedTo))
                {
                    Console.WriteLine("❌ Validation failed: AssignedTo is required");
                    return BadRequest(new { 
                        success = false, 
                        message = "Người phụ trách là bắt buộc" 
                    });
                }

                // Check if warehouse exists
                Console.WriteLine($"🔍 Checking warehouse with ID: {dto.WarehouseId}");
                var warehouse = await _context.Warehouses.FindAsync(dto.WarehouseId);
                if (warehouse == null)
                {
                    Console.WriteLine($"❌ Warehouse not found: {dto.WarehouseId}");
                    return BadRequest(new { 
                        success = false, 
                        message = "Kho không tồn tại" 
                    });
                }
                Console.WriteLine($"✅ Warehouse found: {warehouse.Name}");

                // Check if warehouse cell exists (if provided)
                if (dto.WarehouseCellId.HasValue)
                {
                    Console.WriteLine($"🔍 Checking warehouse cell with ID: {dto.WarehouseCellId.Value}");
                    var cell = await _context.WarehouseCells.FindAsync(dto.WarehouseCellId.Value);
                    if (cell == null)
                    {
                        Console.WriteLine($"❌ Warehouse cell not found: {dto.WarehouseCellId.Value}");
                        return BadRequest(new { 
                            success = false, 
                            message = "Vị trí kho không tồn tại" 
                        });
                    }
                    Console.WriteLine($"✅ Warehouse cell found: {cell.CellCode}");
                }

                Console.WriteLine("🔧 Creating maintenance request entity...");
                var request = new Models.Entities.MaintenanceRequest
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Location = dto.Location,
                    MaintenanceType = dto.MaintenanceType,
                    Priority = dto.Priority,
                    AssignedTo = dto.AssignedTo,
                    ScheduledDate = dto.ScheduledDate,
                    EstimatedDuration = dto.EstimatedDuration,
                    Status = "pending",
                    Progress = 0,
                    Notes = dto.Notes,
                    WarehouseId = dto.WarehouseId,
                    WarehouseCellId = dto.WarehouseCellId,
                    CreatedBy = dto.CreatedBy ?? "System",
                    CreatedAt = DateTime.UtcNow
                };

                Console.WriteLine("💾 Adding maintenance request to context...");
                _context.MaintenanceRequests.Add(request);
                await _context.SaveChangesAsync();
                Console.WriteLine($"✅ Maintenance request created with ID: {request.Id}");

                // Add to history
                var history = new Models.Entities.MaintenanceHistory
                {
                    MaintenanceRequestId = request.Id,
                    Action = "created",
                    Description = "Yêu cầu bảo trì đã được tạo",
                    Progress = 0,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                _context.MaintenanceHistories.Add(history);
                await _context.SaveChangesAsync();

                // Add to warehouse activity log (optional, don't fail if this fails)
                try
                {
                    var activity = new Models.Entities.WarehouseActivity
                    {
                        WarehouseId = request.WarehouseId,
                        CellId = request.WarehouseCellId,
                        ActivityType = "Maintenance",
                        Description = $"Tạo yêu cầu bảo trì: {request.Title}",
                        ProductName = "",
                        BatchNumber = "",
                        Quantity = 0,
                        Unit = "",
                        UserName = request.CreatedBy,
                        UserId = null,
                        Timestamp = DateTime.UtcNow,
                        Notes = $"Loại: {request.MaintenanceType}, Ưu tiên: {request.Priority}, Người phụ trách: {request.AssignedTo}",
                        Status = "Completed",
                        IsActive = true
                    };

                    _context.WarehouseActivities.Add(activity);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"✅ Warehouse activity logged for maintenance: {request.Title} in cell {request.WarehouseCellId}");
                }
                catch (Exception activityEx)
                {
                    // Log activity error but don't fail the main operation
                    Console.WriteLine($"❌ Warning: Failed to add warehouse activity: {activityEx.Message}");
                    Console.WriteLine($"❌ Stack trace: {activityEx.StackTrace}");
                }

                return Ok(new { 
                    success = true, 
                    message = "Tạo yêu cầu bảo trì thành công", 
                    data = new {
                        id = request.Id,
                        title = request.Title,
                        description = request.Description,
                        location = request.Location,
                        maintenanceType = request.MaintenanceType,
                        priority = request.Priority,
                        assignedTo = request.AssignedTo,
                        scheduledDate = request.ScheduledDate,
                        estimatedDuration = request.EstimatedDuration,
                        status = request.Status,
                        progress = request.Progress,
                        notes = request.Notes,
                        warehouseId = request.WarehouseId,
                        warehouseCellId = request.WarehouseCellId,
                        createdBy = request.CreatedBy,
                        createdAt = request.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi tạo yêu cầu bảo trì", 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // PUT: api/maintenance/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateMaintenanceStatus(int id, [FromBody] UpdateMaintenanceStatusDto dto)
        {
            try
            {
                var request = await _context.MaintenanceRequests.FindAsync(id);
                if (request == null)
                {
                    return NotFound(new { 
                        success = false, 
                        message = "Không tìm thấy yêu cầu bảo trì" 
                    });
                }

                var oldStatus = request.Status;
                request.Status = dto.Status;
                request.Progress = dto.Progress;
                request.Notes = dto.Notes;
                request.UpdatedAt = DateTime.UtcNow;
                request.UpdatedBy = dto.UpdatedBy ?? "System";

                if (dto.Status == "completed")
                {
                    request.CompletedAt = DateTime.UtcNow;
                }

                // Add to history
                var history = new Models.Entities.MaintenanceHistory
                {
                    MaintenanceRequestId = request.Id,
                    Action = dto.Status,
                    Description = dto.Description ?? $"Trạng thái đã thay đổi từ {oldStatus} thành {dto.Status}",
                    Progress = dto.Progress,
                    Notes = dto.Notes,
                    CreatedBy = request.UpdatedBy
                };

                _context.MaintenanceHistories.Add(history);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = "Cập nhật trạng thái bảo trì thành công", 
                    data = request 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Lỗi khi cập nhật trạng thái bảo trì", 
                    error = ex.Message 
                });
            }
        }

    }

    // DTOs
    public class CreateMaintenanceRequestDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Location { get; set; } = string.Empty;
        public string MaintenanceType { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string? EstimatedDuration { get; set; }
        public string? Notes { get; set; }
        public int WarehouseId { get; set; }
        public int? WarehouseCellId { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class UpdateMaintenanceStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public int? Progress { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class UpdateMaintenanceProgressDto
    {
        public int Progress { get; set; }
        public string? Notes { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class CompleteMaintenanceDto
    {
        public string? Notes { get; set; }
        public string? CompletedBy { get; set; }
    }
}
