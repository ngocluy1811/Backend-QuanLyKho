using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FertilizerWarehouseAPI.Services;
using System.Security.Claims;

namespace FertilizerWarehouseAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionController> _logger;

    public PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    /// <summary>
    /// Check if current user has a specific permission
    /// </summary>
    [HttpGet("has-permission/{permission}")]
    public IActionResult HasPermission(string permission)
    {
        try
        {
            var role = GetCurrentUserRole();
            if (string.IsNullOrEmpty(role))
            {
                return Unauthorized(new { message = "User role not found" });
            }

            var hasPermission = _permissionService.HasPermission(role, permission);
            
            return Ok(new
            {
                hasPermission,
                role,
                permission
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission {Permission}", permission);
            return StatusCode(500, new { message = "Error checking permission" });
        }
    }

    /// <summary>
    /// Check if current user can access a specific route
    /// </summary>
    [HttpGet("can-access-route/{route}")]
    public IActionResult CanAccessRoute(string route)
    {
        try
        {
            var role = GetCurrentUserRole();
            if (string.IsNullOrEmpty(role))
            {
                return Unauthorized(new { message = "User role not found" });
            }

            var canAccess = _permissionService.CanAccessRoute(role, route);
            
            return Ok(new
            {
                canAccess,
                role,
                route
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking route access {Route}", route);
            return StatusCode(500, new { message = "Error checking route access" });
        }
    }

    /// <summary>
    /// Check if a button should be visible for current user
    /// </summary>
    [HttpGet("is-button-visible/{buttonId}")]
    public IActionResult IsButtonVisible(string buttonId)
    {
        try
        {
            var role = GetCurrentUserRole();
            if (string.IsNullOrEmpty(role))
            {
                return Unauthorized(new { message = "User role not found" });
            }

            var isVisible = _permissionService.IsButtonVisible(role, buttonId);
            
            return Ok(new
            {
                isVisible,
                role,
                buttonId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking button visibility {ButtonId}", buttonId);
            return StatusCode(500, new { message = "Error checking button visibility" });
        }
    }

    /// <summary>
    /// Get all permissions for current user's role
    /// </summary>
    [HttpGet("my-permissions")]
    public IActionResult GetMyPermissions()
    {
        try
        {
            var role = GetCurrentUserRole();
            if (string.IsNullOrEmpty(role))
            {
                return Unauthorized(new { message = "User role not found" });
            }

            var permissions = _permissionService.GetRolePermissions(role);
            
            return Ok(new
            {
                role,
                permissions
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user permissions");
            return StatusCode(500, new { message = "Error getting user permissions" });
        }
    }

    /// <summary>
    /// Get all available roles and their permissions
    /// </summary>
    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        try
        {
            var roles = new[]
            {
                new
                {
                    id = "admin",
                    name = "Quản trị viên",
                    description = "Có tất cả quyền trong hệ thống",
                    color = "#ef4444",
                    permissions = _permissionService.GetRolePermissions("admin")
                },
                new
                {
                    id = "warehouse",
                    name = "Nhân viên kho",
                    description = "Nhân viên làm việc tại kho",
                    color = "#10b981",
                    permissions = _permissionService.GetRolePermissions("warehouse")
                },
                new
                {
                    id = "team_leader",
                    name = "Tổ trưởng",
                    description = "Quản lý nhóm nhân viên",
                    color = "#3b82f6",
                    permissions = _permissionService.GetRolePermissions("team_leader")
                },
                new
                {
                    id = "sales",
                    name = "Nhân viên kinh doanh",
                    description = "Nhân viên phụ trách kinh doanh và bán hàng",
                    color = "#8b5cf6",
                    permissions = _permissionService.GetRolePermissions("sales")
                }
            };

            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles");
            return StatusCode(500, new { message = "Error getting roles" });
        }
    }

    /// <summary>
    /// Check multiple permissions at once
    /// </summary>
    [HttpPost("check-permissions")]
    public IActionResult CheckPermissions([FromBody] CheckPermissionsRequest request)
    {
        try
        {
            var role = GetCurrentUserRole();
            if (string.IsNullOrEmpty(role))
            {
                return Unauthorized(new { message = "User role not found" });
            }

            var results = new Dictionary<string, bool>();
            foreach (var permission in request.Permissions)
            {
                results[permission] = _permissionService.HasPermission(role, permission);
            }

            return Ok(new
            {
                role,
                results
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking multiple permissions");
            return StatusCode(500, new { message = "Error checking permissions" });
        }
    }

    private string? GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        return roleClaim?.Value;
    }
}

public class CheckPermissionsRequest
{
    public List<string> Permissions { get; set; } = new();
}
