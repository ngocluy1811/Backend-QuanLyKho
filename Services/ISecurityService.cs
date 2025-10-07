using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FertilizerWarehouseAPI.Models.Entities;
using Task = System.Threading.Tasks.Task;

namespace FertilizerWarehouseAPI.Services
{
    public interface ISecurityService
    {
        // Login/Logout tracking
        Task<LoginHistory> LogLoginAsync(int userId, string ipAddress, string userAgent, bool isSuccessful, string? failureReason = null);
        Task LogLogoutAsync(int userId, string sessionId);
        Task LogFailedLoginAsync(int userId, string ipAddress, string userAgent, string failureReason);

        // Device management
        Task<TrustedDevice?> GetTrustedDeviceAsync(int userId, string deviceFingerprint);
        Task<TrustedDevice> AddTrustedDeviceAsync(int userId, string deviceName, string deviceType, string operatingSystem, string browser, string userAgent, string? ipAddress, string? location);
        Task UpdateDeviceLastSeenAsync(Guid deviceId);
        Task BlockDeviceAsync(Guid deviceId, string reason);

        // Security analysis
        Task<bool> IsSuspiciousLoginAsync(int userId, string ipAddress, string userAgent, string? location);
        Task<int> CalculateRiskScoreAsync(int userId, string ipAddress, string? location, string? deviceFingerprint);
        Task<List<SecurityAlert>> GetActiveAlertsAsync(int userId);
        Task CreateSecurityAlertAsync(int userId, string alertType, string title, string description, string severity, string? additionalData = null);

        // Location and IP analysis
        Task<string?> GetLocationFromIpAsync(string ipAddress);
        Task<bool> IsNewLocationAsync(int userId, string? location);
        Task<bool> IsNewIpAsync(int userId, string ipAddress);

        // Session management
        Task<bool> IsValidSessionAsync(string sessionId);
        Task InvalidateSessionAsync(string sessionId);
        Task InvalidateAllUserSessionsAsync(int userId);

        // Security reports
        Task<List<LoginHistory>> GetLoginHistoryAsync(int userId, int page = 1, int pageSize = 20);
        Task<List<LoginHistory>> GetSuspiciousActivitiesAsync(int userId);
        Task<object> GetSecurityDashboardAsync(int userId);

        // Auto-lock features
        Task<bool> ShouldLockAccountAsync(int userId);
        Task LockAccountAsync(int userId, string reason, int lockoutMinutes = 30);
        Task UnlockAccountAsync(int userId);
    }
}
