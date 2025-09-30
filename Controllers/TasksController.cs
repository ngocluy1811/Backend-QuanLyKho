using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Models.Enums;
using TaskStatus = FertilizerWarehouseAPI.Models.Enums.TaskStatus;

namespace FertilizerWarehouseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all tasks
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<object>>> GetTasks(
            [FromQuery] string? status = null,
            [FromQuery] string? priority = null,
            [FromQuery] int? assignedTo = null,
            [FromQuery] int? warehouseId = null)
        {
            try
            {
                var query = _context.Tasks
                    .Include(t => t.CreatedByUser)
                    .Include(t => t.AssignedToUser)
                    .Include(t => t.Warehouse)
                    .Where(t => t.IsActive);

                // Apply filters
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskStatus>(status, out var taskStatus))
                {
                    query = query.Where(t => t.Status == taskStatus);
                }

                if (!string.IsNullOrEmpty(priority))
                {
                    query = query.Where(t => t.Priority == priority);
                }

                if (assignedTo.HasValue)
                {
                    query = query.Where(t => t.AssignedTo == assignedTo.Value);
                }

                if (warehouseId.HasValue)
                {
                    query = query.Where(t => t.WarehouseId == warehouseId.Value);
                }

                var tasks = await query
                    .Select(t => new
                    {
                        t.Id,
                        t.Title,
                        t.Description,
                        t.Status,
                        StatusName = t.Status.ToString(),
                        t.Priority,
                        t.TaskType,
                        t.CreatedAt,
                        t.DueDate,
                        t.StartedAt,
                        t.CompletedAt,
                        CreatedBy = t.CreatedBy,
                        CreatedByName = t.CreatedByUser.FullName,
                        AssignedTo = t.AssignedTo,
                        AssignedToName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                        WarehouseId = t.WarehouseId,
                        WarehouseName = t.Warehouse != null ? t.Warehouse.Name : null,
                        t.EstimatedHours,
                        t.ActualHours,
                        t.ProgressPercentage,
                        IsOverdue = t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed,
                        // Calculate days remaining
                        DaysRemaining = t.DueDate != null ? (int?)(t.DueDate.Value - DateTime.UtcNow).TotalDays : null
                    })
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving tasks", error = ex.Message });
            }
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetTask(int id)
        {
            try
            {
                var task = await _context.Tasks
                    .Include(t => t.CreatedByUser)
                    .Include(t => t.AssignedToUser)
                    .Include(t => t.Warehouse)
                    .Include(t => t.Comments)
                        .ThenInclude(c => c.CommentByUser)
                    .Where(t => t.Id == id && t.IsActive)
                    .Select(t => new
                    {
                        t.Id,
                        t.Title,
                        t.Description,
                        t.Status,
                        StatusName = t.Status.ToString(),
                        t.Priority,
                        t.TaskType,
                        t.CreatedAt,
                        t.DueDate,
                        t.StartedAt,
                        t.CompletedAt,
                        CreatedBy = t.CreatedBy,
                        CreatedByName = t.CreatedByUser.FullName,
                        AssignedTo = t.AssignedTo,
                        AssignedToName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                        WarehouseId = t.WarehouseId,
                        WarehouseName = t.Warehouse != null ? t.Warehouse.Name : null,
                        t.EntityType,
                        t.EntityId,
                        t.EstimatedHours,
                        t.ActualHours,
                        t.ProgressPercentage,
                        IsOverdue = t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed,
                        TimeSpent = t.ActualHours,
                        TimeRemaining = t.EstimatedHours - t.ActualHours,
                        // Comments
                        Comments = t.Comments.Where(c => c.IsActive).Select(c => new
                        {
                            c.Id,
                            c.Comment,
                            c.CommentAt,
                            CommentBy = c.CommentBy,
                            CommentByName = c.CommentByUser.FullName,
                            TimeAgo = GetTimeAgo(c.CommentAt)
                        }).OrderByDescending(c => c.CommentAt)
                    })
                    .FirstOrDefaultAsync();

                if (task == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving task", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new task
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "TeamLeader")]
        public async Task<ActionResult<object>> CreateTask([FromBody] CreateTaskBasicDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var task = new Models.Entities.Task
                {
                    Title = createDto.Title,
                    Description = createDto.Description,
                    Priority = createDto.Priority,
                    TaskType = createDto.TaskType,
                    DueDate = createDto.DueDate,
                    AssignedTo = createDto.AssignedTo,
                    WarehouseId = createDto.WarehouseId,
                    EntityType = createDto.EntityType,
                    EntityId = createDto.EntityId,
                    EstimatedHours = createDto.EstimatedHours,
                    Status = TaskStatus.Pending,
                    ProgressPercentage = 0,
                    ActualHours = 0,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetCurrentUserId(),
                    IsActive = true
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, new
                {
                    task.Id,
                    task.Title,
                    task.Status,
                    task.Priority,
                    task.CreatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating task", error = ex.Message });
            }
        }

        /// <summary>
        /// Update task
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> UpdateTask(int id, [FromBody] UpdateTaskBasicDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var task = await _context.Tasks.FindAsync(id);
                if (task == null || !task.IsActive)
                    return NotFound(new { message = "Task not found" });

                // Check if user can update this task
                var currentUserId = GetCurrentUserId();
                if (task.AssignedTo != currentUserId && task.CreatedBy != currentUserId && !IsManagement())
                {
                    return Forbid("You can only update tasks assigned to you or created by you");
                }

                task.Title = updateDto.Title;
                task.Description = updateDto.Description;
                task.Status = updateDto.Status;
                task.Priority = updateDto.Priority;
                task.TaskType = updateDto.TaskType;
                task.DueDate = updateDto.DueDate;
                task.AssignedTo = updateDto.AssignedTo;
                task.WarehouseId = updateDto.WarehouseId;
                task.EstimatedHours = updateDto.EstimatedHours;
                task.ActualHours = updateDto.ActualHours;
                task.ProgressPercentage = updateDto.ProgressPercentage;
                task.IsActive = updateDto.IsActive;
                task.UpdatedAt = DateTime.UtcNow;

                // Update status timestamps
                if (updateDto.Status == TaskStatus.InProgress && task.StartedAt == null)
                {
                    task.StartedAt = DateTime.UtcNow;
                }
                else if (updateDto.Status == TaskStatus.Completed && task.CompletedAt == null)
                {
                    task.CompletedAt = DateTime.UtcNow;
                    task.ProgressPercentage = 100;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    task.Id,
                    task.Title,
                    task.Status,
                    task.ProgressPercentage,
                    task.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating task", error = ex.Message });
            }
        }

        /// <summary>
        /// Update task status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult> UpdateTaskStatus(int id, [FromBody] UpdateTaskStatusBasicDto statusDto)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null || !task.IsActive)
                    return NotFound(new { message = "Task not found" });

                // Check if user can update this task
                var currentUserId = GetCurrentUserId();
                if (task.AssignedTo != currentUserId && task.CreatedBy != currentUserId && !IsManagement())
                {
                    return Forbid("You can only update tasks assigned to you or created by you");
                }

                task.Status = statusDto.Status;
                task.ProgressPercentage = statusDto.ProgressPercentage;
                task.UpdatedAt = DateTime.UtcNow;

                if (statusDto.ActualHours.HasValue)
                {
                    task.ActualHours = statusDto.ActualHours.Value;
                }

                // Update timestamps
                if (statusDto.Status == TaskStatus.InProgress && task.StartedAt == null)
                {
                    task.StartedAt = DateTime.UtcNow;
                }
                else if (statusDto.Status == TaskStatus.Completed && task.CompletedAt == null)
                {
                    task.CompletedAt = DateTime.UtcNow;
                    task.ProgressPercentage = 100;
                }

                await _context.SaveChangesAsync();

                // Add status update comment
                if (!string.IsNullOrEmpty(statusDto.StatusNote))
                {
                    var comment = new TaskComment
                    {
                        TaskId = id,
                        Comment = $"Status updated to {statusDto.Status}: {statusDto.StatusNote}",
                        CommentBy = currentUserId,
                        CommentAt = DateTime.UtcNow,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.TaskComments.Add(comment);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Task status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating task status", error = ex.Message });
            }
        }

        /// <summary>
        /// Add comment to task
        /// </summary>
        [HttpPost("{id}/comments")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> AddTaskComment(int id, [FromBody] AddTaskCommentBasicDto commentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var task = await _context.Tasks.FindAsync(id);
                if (task == null || !task.IsActive)
                    return NotFound(new { message = "Task not found" });

                var comment = new TaskComment
                {
                    TaskId = id,
                    Comment = commentDto.Comment,
                    CommentBy = GetCurrentUserId(),
                    CommentAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TaskComments.Add(comment);
                await _context.SaveChangesAsync();

                // Get user info for response
                var user = await _context.Users.FindAsync(comment.CommentBy);

                return Ok(new
                {
                    comment.Id,
                    comment.Comment,
                    comment.CommentAt,
                    CommentBy = comment.CommentBy,
                    CommentByName = user?.FullName,
                    TimeAgo = GetTimeAgo(comment.CommentAt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding comment", error = ex.Message });
            }
        }

        /// <summary>
        /// Get tasks summary
        /// </summary>
        [HttpGet("summary")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<object>> GetTasksSummary()
        {
            try
            {
                var today = DateTime.Today;
                var currentUserId = GetCurrentUserId();

                var summary = new
                {
                    TotalTasks = await _context.Tasks.CountAsync(t => t.IsActive),
                    PendingTasks = await _context.Tasks.CountAsync(t => t.IsActive && t.Status == TaskStatus.Pending),
                    InProgressTasks = await _context.Tasks.CountAsync(t => t.IsActive && t.Status == TaskStatus.InProgress),
                    CompletedTasks = await _context.Tasks.CountAsync(t => t.IsActive && t.Status == TaskStatus.Completed),
                    OverdueTasks = await _context.Tasks.CountAsync(t => t.IsActive && t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed),
                    MyTasks = await _context.Tasks.CountAsync(t => t.IsActive && t.AssignedTo == currentUserId),
                    MyPendingTasks = await _context.Tasks.CountAsync(t => t.IsActive && t.AssignedTo == currentUserId && t.Status == TaskStatus.Pending),
                    MyOverdueTasks = await _context.Tasks.CountAsync(t => t.IsActive && t.AssignedTo == currentUserId && t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed),
                    TasksCompletedToday = await _context.Tasks.CountAsync(t => t.IsActive && t.CompletedAt.HasValue && t.CompletedAt.Value.Date == today),
                    AverageCompletionTime = await _context.Tasks
                        .Where(t => t.IsActive && t.Status == TaskStatus.Completed && t.ActualHours > 0)
                        .AverageAsync(t => (double?)t.ActualHours) ?? 0
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving summary", error = ex.Message });
            }
        }

        /// <summary>
        /// Get my tasks (assigned to current user)
        /// </summary>
        [HttpGet("my-tasks")]
        [Authorize(Policy = "Warehouse")]
        public async Task<ActionResult<IEnumerable<object>>> GetMyTasks([FromQuery] string? status = null)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var query = _context.Tasks
                    .Include(t => t.CreatedByUser)
                    .Include(t => t.Warehouse)
                    .Where(t => t.IsActive && t.AssignedTo == currentUserId);

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskStatus>(status, out var taskStatus))
                {
                    query = query.Where(t => t.Status == taskStatus);
                }

                var tasks = await query
                    .Select(t => new
                    {
                        t.Id,
                        t.Title,
                        t.Description,
                        t.Status,
                        StatusName = t.Status.ToString(),
                        t.Priority,
                        t.TaskType,
                        t.CreatedAt,
                        t.DueDate,
                        t.ProgressPercentage,
                        CreatedByName = t.CreatedByUser.FullName,
                        WarehouseName = t.Warehouse != null ? t.Warehouse.Name : null,
                        IsOverdue = t.DueDate < DateTime.UtcNow && t.Status != TaskStatus.Completed,
                        DaysRemaining = t.DueDate != null ? (int?)(t.DueDate.Value - DateTime.UtcNow).TotalDays : null
                    })
                    .OrderBy(t => t.DueDate)
                    .ToListAsync();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving your tasks", error = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 1; // Default to admin
        }

        private bool IsManagement()
        {
            return User.IsInRole("Admin") || User.IsInRole("TeamLeader");
        }

        private static string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;
            if (timeSpan.Days > 0) return $"{timeSpan.Days} ngày trước";
            if (timeSpan.Hours > 0) return $"{timeSpan.Hours} giờ trước";
            if (timeSpan.Minutes > 0) return $"{timeSpan.Minutes} phút trước";
            return "Vừa xong";
        }
    }

    // Basic DTOs for Tasks
    public class CreateTaskBasicDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = "Normal";
        public string TaskType { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public int? AssignedTo { get; set; }
        public int? WarehouseId { get; set; }
        public string? EntityType { get; set; }
        public int? EntityId { get; set; }
        public int EstimatedHours { get; set; }
    }

    public class UpdateTaskBasicDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string TaskType { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public int? AssignedTo { get; set; }
        public int? WarehouseId { get; set; }
        public int EstimatedHours { get; set; }
        public int ActualHours { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateTaskStatusBasicDto
    {
        public TaskStatus Status { get; set; }
        public int ProgressPercentage { get; set; }
        public string? StatusNote { get; set; }
        public int? ActualHours { get; set; }
    }

    public class AddTaskCommentBasicDto
    {
        public string Comment { get; set; } = string.Empty;
    }
}
