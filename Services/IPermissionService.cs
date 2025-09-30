using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Services;

public interface IPermissionService
{
    bool HasPermission(string role, string permission);
    bool CanAccessRoute(string role, string route);
    bool IsButtonVisible(string role, string buttonId);
    List<string> GetRolePermissions(string role);
    List<string> GetRoutePermissions(string route);
}
