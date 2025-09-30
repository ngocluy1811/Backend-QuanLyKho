using Microsoft.EntityFrameworkCore;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.DTOs;
using FertilizerWarehouseAPI.Models.Entities;
using FertilizerWarehouseAPI.Services.Interfaces;
using AutoMapper;
using BCrypt.Net;
using Task = System.Threading.Tasks.Task;

namespace FertilizerWarehouseAPI.Services.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, IMapper mapper, ILogger<UserService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .Where(u => u.IsActive)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createDto)
    {
        var user = _mapper.Map<User>(createDto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password);
        user.CreatedAt = DateTime.UtcNow;
        user.IsActive = true;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User created: {Username} by admin", user.Username);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateDto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null || !user.IsActive) return null;

        _mapper.Map(updateDto, user);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("User updated: {Username}", user.Username);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User deleted: {Username}", user.Username);
        return true;
    }

    public async Task<bool> DeactivateUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ActivateUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null) return false;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.IsActive) return false;

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Password changed for user: {Username}", user.Username);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null) return false;

        // Generate reset token and send email
        // Implementation for password reset email
        _logger.LogInformation("Password reset requested for user: {Email}", email);
        return true;
    }

    public async Task<bool> SetPasswordAsync(int userId, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.IsActive) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return new List<string>();

        return new List<string> { user.Role.ToString() };
    }

    public async Task<bool> AssignRoleToUserAsync(int userId, string role)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (Enum.TryParse<Models.Enums.UserRole>(role, true, out var userRole))
        {
            user.Role = userRole;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> RemoveRoleFromUserAsync(int userId, string role)
    {
        // In this simple implementation, users have only one role
        // This could be extended to support multiple roles
        return false;
    }

    public async Task<IEnumerable<string>> GetUserPermissionsAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return new List<string>();

        // Return permissions based on role
        return user.Role switch
        {
            Models.Enums.UserRole.Admin => new List<string> { "all" },
            Models.Enums.UserRole.TeamLeader => new List<string> { "warehouse.read", "warehouse.write", "product.read", "product.write", "team.read", "team.write" },
            Models.Enums.UserRole.Warehouse => new List<string> { "warehouse.read", "product.read" },
            Models.Enums.UserRole.Sales => new List<string> { "sales.read", "sales.write", "customer.read", "customer.write" },
            _ => new List<string>()
        };
    }

    public async Task<bool> HasPermissionAsync(int userId, string permission)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        return permissions.Contains(permission) || permissions.Contains("all");
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        return user != null ? _mapper.Map<UserProfileDto>(user) : null;
    }

    public async Task<UserProfileDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.IsActive) return null;

        _mapper.Map(updateDto, user);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task<bool> UpdateUserAvatarAsync(int userId, Stream avatarStream)
    {
        // Implementation for avatar update
        return true;
    }

    public async Task<bool> DeleteUserAvatarAsync(int userId)
    {
        // Implementation for avatar deletion
        return true;
    }

    // Placeholder implementations for complex methods
    public async Task<IEnumerable<UserActivityDto>> GetUserActivityLogsAsync(int userId, int pageNumber, int pageSize)
    {
        // Implementation for user activity logs
        return new List<UserActivityDto>();
    }

    // Additional methods required by interface
    public async Task<IEnumerable<UserSessionDto>> GetUserSessionsAsync(int userId)
    {
        return new List<UserSessionDto>();
    }

    public async Task<bool> InvalidateUserSessionAsync(int userId, string sessionId)
    {
        return true;
    }

    public async Task<bool> InvalidateAllUserSessionsAsync(int userId)
    {
        return true;
    }

    public async Task<UserActivityDto> GetUserActivityAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        return new UserActivityDto();
    }

    public async Task<bool> LogUserActivityAsync(int userId, string action, string? description = null)
    {
        // Implementation for logging user activity
        return true;
    }

    public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchQuery)
    {
        var users = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .Where(u => u.IsActive && 
                       (u.Username.Contains(searchQuery) || 
                        u.FullName.Contains(searchQuery) ||
                        u.Email.Contains(searchQuery)))
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<IEnumerable<UserDto>> FilterUsersAsync(string? role = null, string? department = null, string? status = null)
    {
        var query = _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .Where(u => u.IsActive);

        if (!string.IsNullOrEmpty(role))
            query = query.Where(u => u.Role.ToString() == role);

        if (!string.IsNullOrEmpty(department))
            query = query.Where(u => u.Department!.Name == department);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(u => u.IsActive.ToString() == status);

        var users = await query.ToListAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role)
    {
        var users = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .Where(u => u.IsActive && u.Role.ToString() == role)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<IEnumerable<UserDto>> GetUsersByDepartmentAsync(int departmentId)
    {
        var users = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .Where(u => u.IsActive && u.DepartmentId == departmentId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserStatsDto> GetUserStatsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var activeUsers = await _context.Users.CountAsync(u => u.IsActive);
        var inactiveUsers = totalUsers - activeUsers;
        var newUsersThisMonth = await _context.Users.CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddMonths(-1));

        return new UserStatsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = inactiveUsers,
            NewUsersThisMonth = newUsersThisMonth,
            UsersByRole = 0,
            UsersByDepartment = 0,
            LastUpdated = DateTime.UtcNow
        };
    }

    public async Task<IEnumerable<UserLoginHistoryDto>> GetUserLoginHistoryAsync(int userId, int pageNumber)
    {
        return new List<UserLoginHistoryDto>();
    }

    public async Task<IEnumerable<UserDto>> GetRecentlyActiveUsersAsync(int count)
    {
        var users = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .Where(u => u.IsActive)
            .OrderByDescending(u => u.LastLogin)
            .Take(count)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<IEnumerable<UserNotificationDto>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
    {
        return new List<UserNotificationDto>();
    }

    public async Task<bool> MarkNotificationAsReadAsync(int userId, int notificationId)
    {
        // Implementation for marking notification as read
        return true;
    }

    public async Task<bool> MarkAllNotificationsAsReadAsync(int userId)
    {
        // Implementation for marking all notifications as read
        return true;
    }

    public async Task<bool> SendNotificationToUserAsync(int userId, string title, string message, string type)
    {
        // Implementation for sending notification
        return true;
    }

    public async Task<bool> BulkDeactivateUsersAsync(IEnumerable<int> userIds)
    {
        var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
        foreach (var user in users)
        {
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignUserToDepartmentAsync(int userId, int departmentId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.DepartmentId = departmentId;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveUserFromDepartmentAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.DepartmentId = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<UserDto>> GetDepartmentManagersAsync()
    {
        var users = await _context.Users
            .Include(u => u.Department)
            .Include(u => u.Company)
            .Where(u => u.IsActive && u.Role == Models.Enums.UserRole.TeamLeader)
            .ToListAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<bool> LockUserAccountAsync(int userId, string reason)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnlockUserAccountAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RequirePasswordChangeAsync(int userId)
    {
        // Implementation for requiring password change
        return true;
    }

    public async Task<IEnumerable<UserSecurityEventDto>> GetUserSecurityEventsAsync(int userId)
    {
        return new List<UserSecurityEventDto>();
    }

    public async Task<IEnumerable<UserDto>> GetUsersByDepartmentAsync(int departmentId, int pageNumber, int pageSize)
    {
        // Implementation for users by department
        return new List<UserDto>();
    }

    public async Task<UserStatsDto> GetUserStatisticsAsync()
    {
        // Implementation for user statistics
        return new UserStatsDto();
    }

    public async Task<IEnumerable<UserLoginHistoryDto>> GetUserLoginHistoryAsync(int userId, int pageNumber, int pageSize)
    {
        // Implementation for login history
        return new List<UserLoginHistoryDto>();
    }

    public async Task<IEnumerable<UserNotificationDto>> GetUserNotificationsAsync(int userId, int pageNumber, int pageSize)
    {
        // Implementation for user notifications
        return new List<UserNotificationDto>();
    }

    public async Task MarkNotificationAsReadAsync(int notificationId)
    {
        // Implementation for marking notification as read
    }

    public async Task<UserPreferencesDto?> GetUserPreferencesAsync(int userId)
    {
        // Implementation for user preferences
        return new UserPreferencesDto();
    }

    public async Task<UserPreferencesDto?> UpdateUserPreferencesAsync(int userId, UserPreferencesDto userPreferencesDto)
    {
        // Implementation for updating user preferences
        return new UserPreferencesDto();
    }

    public async Task<bool> BulkCreateUsersAsync(IEnumerable<CreateUserDto> createUsersDto)
    {
        // Implementation for bulk user creation
        return true;
    }

    public async Task<bool> BulkUpdateUsersAsync(IEnumerable<BulkUpdateUserDto> bulkUpdateUserDto)
    {
        // Implementation for bulk user update
        return true;
    }

    public async Task<bool> BulkDeleteUsersAsync(IEnumerable<int> userIds)
    {
        // Implementation for bulk user deletion
        return true;
    }

    public async Task<IEnumerable<UserSecurityEventDto>> GetUserSecurityEventsAsync(int userId, int pageNumber, int pageSize)
    {
        // Implementation for user security events
        return new List<UserSecurityEventDto>();
    }
}
