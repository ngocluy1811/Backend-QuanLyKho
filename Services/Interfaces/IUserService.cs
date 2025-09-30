using FertilizerWarehouseAPI.DTOs;
using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Services.Interfaces
{
    public interface IUserService
    {
        // User CRUD operations
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto> CreateUserAsync(CreateUserDto createDto);
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);

        // User authentication
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(string email);
        Task<bool> SetPasswordAsync(int userId, string newPassword);

        // User roles and permissions
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);
        Task<bool> AssignRoleToUserAsync(int userId, string role);
        Task<bool> RemoveRoleFromUserAsync(int userId, string role);
        Task<IEnumerable<string>> GetUserPermissionsAsync(int userId);
        Task<bool> HasPermissionAsync(int userId, string permission);

        // User profile management
        Task<UserProfileDto?> GetUserProfileAsync(int userId);
        Task<UserProfileDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto);
        Task<bool> UpdateUserAvatarAsync(int userId, Stream avatarStream);
        Task<bool> DeleteUserAvatarAsync(int userId);

        // User sessions and activity
        Task<IEnumerable<UserSessionDto>> GetUserSessionsAsync(int userId);
        Task<bool> InvalidateUserSessionAsync(int userId, string sessionId);
        Task<bool> InvalidateAllUserSessionsAsync(int userId);
        Task<UserActivityDto> GetUserActivityAsync(int userId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> LogUserActivityAsync(int userId, string activity, string? details = null);

        // User search and filtering
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchQuery);
        Task<IEnumerable<UserDto>> FilterUsersAsync(string? role = null, string? department = null, string? status = null);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<UserDto>> GetUsersByDepartmentAsync(int departmentId);

        // User statistics
        Task<UserStatsDto> GetUserStatsAsync();
        Task<IEnumerable<UserLoginHistoryDto>> GetUserLoginHistoryAsync(int userId, int pageSize = 50);
        Task<IEnumerable<UserDto>> GetRecentlyActiveUsersAsync(int count = 10);

        // User notifications
        Task<IEnumerable<UserNotificationDto>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
        Task<bool> MarkNotificationAsReadAsync(int userId, int notificationId);
        Task<bool> MarkAllNotificationsAsReadAsync(int userId);
        Task<bool> SendNotificationToUserAsync(int userId, string title, string message, string type = "info");

        // User preferences
        Task<UserPreferencesDto?> GetUserPreferencesAsync(int userId);
        Task<UserPreferencesDto?> UpdateUserPreferencesAsync(int userId, UserPreferencesDto preferences);

        // Bulk operations
        Task<bool> BulkCreateUsersAsync(IEnumerable<CreateUserDto> users);
        Task<bool> BulkUpdateUsersAsync(IEnumerable<BulkUpdateUserDto> users);
        Task<bool> BulkDeactivateUsersAsync(IEnumerable<int> userIds);

        // Department management for users
        Task<bool> AssignUserToDepartmentAsync(int userId, int departmentId);
        Task<bool> RemoveUserFromDepartmentAsync(int userId);
        Task<IEnumerable<UserDto>> GetDepartmentManagersAsync();

        // User security
        Task<bool> LockUserAccountAsync(int userId, string reason);
        Task<bool> UnlockUserAccountAsync(int userId);
        Task<bool> RequirePasswordChangeAsync(int userId);
        Task<IEnumerable<UserSecurityEventDto>> GetUserSecurityEventsAsync(int userId);
    }
}
