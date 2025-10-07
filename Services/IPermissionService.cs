using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Services;

public interface IPermissionService
{
    bool HasPermission(string role, string permission);
    bool CanAccessRoute(string role, string route);
    bool IsButtonVisible(string role, string buttonId);
    List<string> GetRolePermissions(string role);
    List<string> GetRoutePermissions(string route);
    
    // Async methods for API compatibility
    Task<List<string>> GetUserPermissionsAsync(string userId, string role);
    Task<bool> HasPermissionAsync(string userId, string role, string permission);
    Task<List<string>> GetRolePermissionsAsync(string role);
    Task<List<object>> GetAllRolesAsync();
}
