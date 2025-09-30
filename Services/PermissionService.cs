using FertilizerWarehouseAPI.Models.Enums;

namespace FertilizerWarehouseAPI.Services;

public class PermissionService : IPermissionService
{
    // Permission mappings based on frontend PermissionContext
    private readonly Dictionary<string, List<string>> _rolePermissions = new()
    {
        ["admin"] = new List<string> { "all" },
        ["warehouse"] = new List<string>
        {
            "view_warehouse", "view_cell_details", "use_warehouse_filters", "view_products",
            "import_products", "export_products", "track_products", "manage_inventory",
            "view_dashboard", "perform_inventory_check", "view_inventory_check_history",
            "view_leave_requests", "create_leave_requests", "view_notifications"
        },
        ["team_leader"] = new List<string>
        {
            "view_warehouse", "edit_warehouse", "manage_locations", "view_products",
            "edit_products", "manage_products", "view_employees", "view_employee_performance",
            "view_reports", "view_dashboard", "view_production", "manage_production",
            "customize_warehouse", "warehouse_config", "view_maintenance_schedule",
            "create_maintenance_task", "initiate_inventory_check", "approve_inventory_adjustments",
            "view_alerts", "view_leave_requests", "create_leave_requests", "approve_leave_requests",
            "reject_leave_requests", "view_notifications", "create_notifications",
            "edit_notifications", "delete_notifications"
        },
        ["sales"] = new List<string>
        {
            "view_products", "view_warehouse", "export_products", "view_reports",
            "view_dashboard", "view_alerts", "view_leave_requests", "create_leave_requests",
            "view_notifications"
        }
    };

    private readonly Dictionary<string, List<string>> _routePermissions = new()
    {
        ["/dashboard"] = new List<string> { "view_dashboard" },
        ["/warehouse"] = new List<string> { "view_warehouse" },
        ["/production"] = new List<string> { "view_production" },
        ["/employees"] = new List<string> { "view_employees" },
        ["/reports"] = new List<string> { "view_reports" },
        ["/products"] = new List<string> { "view_products" },
        ["/inventory"] = new List<string> { "inventory_check" },
        ["/alerts"] = new List<string> { "view_alerts" },
        ["/settings"] = new List<string> { "manage_settings" },
        ["/leave-requests"] = new List<string> { "view_leave_requests" },
        ["/notifications"] = new List<string> { "view_notifications" },
        ["/attendance"] = new List<string> { "view_attendance" }
    };

    private readonly Dictionary<string, List<string>> _buttonPermissions = new()
    {
        // Warehouse management buttons
        ["btn_warehouse_config"] = new List<string> { "customize_warehouse", "warehouse_config" },
        ["btn_permissions"] = new List<string> { "manage_permissions" },
        ["btn_production"] = new List<string> { "manage_production" },
        ["btn_import"] = new List<string> { "import_products" },
        ["btn_export"] = new List<string> { "export_products" },
        ["btn_filter"] = new List<string> { "use_warehouse_filters" },
        ["btn_maintenance"] = new List<string> { "maintenance" },
        ["btn_inventory"] = new List<string> { "inventory_detail" },
        ["btn_refresh"] = new List<string> { "view_warehouse" },
        ["btn_drag_position"] = new List<string> { "drag_drop_position" },
        ["btn_transfer"] = new List<string> { "transfer_goods" },
        ["btn_manage_cluster"] = new List<string> { "manage_clusters" },
        ["btn_create_cluster"] = new List<string> { "create_cluster" },
        ["btn_empty_cell"] = new List<string> { "empty_cell" },
        
        // Menu items
        ["menu_dashboard"] = new List<string> { "view_dashboard" },
        ["menu_warehouse"] = new List<string> { "view_warehouse" },
        ["menu_production"] = new List<string> { "view_production" },
        ["menu_employees"] = new List<string> { "view_employees" },
        ["menu_reports"] = new List<string> { "view_reports" },
        ["menu_inventory"] = new List<string> { "inventory_check" },
        ["menu_alerts"] = new List<string> { "view_alerts" },
        ["menu_settings"] = new List<string> { "manage_settings" },
        ["menu_products"] = new List<string> { "view_products" },
        ["menu_leave_requests"] = new List<string> { "view_leave_requests" },
        ["menu_notifications"] = new List<string> { "view_notifications" },
        ["menu_attendance"] = new List<string> { "view_attendance" },
        
        // Leave Request buttons
        ["btn_create_leave_request"] = new List<string> { "create_leave_requests" },
        ["btn_approve_leave_request"] = new List<string> { "approve_leave_requests" },
        ["btn_reject_leave_request"] = new List<string> { "reject_leave_requests" },
        ["btn_delete_leave_request"] = new List<string> { "delete_leave_requests" },
        
        // Notification buttons
        ["btn_create_notification"] = new List<string> { "create_notifications" },
        ["btn_edit_notification"] = new List<string> { "edit_notifications" },
        ["btn_delete_notification"] = new List<string> { "delete_notifications" },
        ["btn_send_notification"] = new List<string> { "send_notifications" }
    };

    public bool HasPermission(string role, string permission)
    {
        if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(permission))
            return false;

        var roleKey = role.ToLower();
        if (!_rolePermissions.ContainsKey(roleKey))
            return false;

        var permissions = _rolePermissions[roleKey];
        
        // Admin has all permissions
        if (permissions.Contains("all"))
            return true;

        return permissions.Contains(permission);
    }

    public bool CanAccessRoute(string role, string route)
    {
        if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(route))
            return false;

        if (!_routePermissions.ContainsKey(route))
            return true; // Default to accessible if no specific permissions defined

        var requiredPermissions = _routePermissions[route];
        return requiredPermissions.Any(permission => HasPermission(role, permission));
    }

    public bool IsButtonVisible(string role, string buttonId)
    {
        if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(buttonId))
            return true; // Default to visible if no specific permissions defined

        if (!_buttonPermissions.ContainsKey(buttonId))
            return true; // Default to visible if no mapping exists

        var requiredPermissions = _buttonPermissions[buttonId];
        return requiredPermissions.Any(permission => HasPermission(role, permission));
    }

    public List<string> GetRolePermissions(string role)
    {
        if (string.IsNullOrEmpty(role))
            return new List<string>();

        var roleKey = role.ToLower();
        return _rolePermissions.ContainsKey(roleKey) ? _rolePermissions[roleKey] : new List<string>();
    }

    public List<string> GetRoutePermissions(string route)
    {
        if (string.IsNullOrEmpty(route))
            return new List<string>();

        return _routePermissions.ContainsKey(route) ? _routePermissions[route] : new List<string>();
    }
}
