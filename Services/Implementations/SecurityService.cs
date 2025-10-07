using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FertilizerWarehouseAPI.Data;
using FertilizerWarehouseAPI.Models.Entities;
using System.Text.Json;
using System.Net.Http;
using System.Text.RegularExpressions;
using Task = System.Threading.Tasks.Task;

namespace FertilizerWarehouseAPI.Services.Implementations
{
    public class SecurityService : ISecurityService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SecurityService> _logger;
        private readonly HttpClient _httpClient;

        public SecurityService(ApplicationDbContext context, ILogger<SecurityService> logger, HttpClient httpClient)
        {
            _context = context;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<LoginHistory> LogLoginAsync(int userId, string ipAddress, string userAgent, bool isSuccessful, string? failureReason = null)
        {
            try
            {
                var deviceInfo = ParseUserAgent(userAgent);
                var location = await GetLocationFromIpAsync(ipAddress);
                var isSuspicious = await IsSuspiciousLoginAsync(userId, ipAddress, userAgent, location);
                var riskScore = await CalculateRiskScoreAsync(userId, ipAddress, location, deviceInfo.DeviceFingerprint);

                var loginHistory = new LoginHistory
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Action = isSuccessful ? "Login" : "FailedLogin",
                    Timestamp = DateTime.UtcNow,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    DeviceType = deviceInfo.DeviceType,
                    OperatingSystem = deviceInfo.OperatingSystem,
                    Browser = deviceInfo.Browser,
                    DeviceName = deviceInfo.DeviceName,
                    Location = location,
                    IsSuccessful = isSuccessful,
                    FailureReason = failureReason,
                    IsSuspicious = isSuspicious,
                    SessionId = isSuccessful ? Guid.NewGuid().ToString() : null,
                    SessionExpiry = isSuccessful ? DateTime.UtcNow.AddHours(8) : null,
                    LoginMethod = "Password", // Can be enhanced with 2FA detection
                    IsActiveSession = isSuccessful,
                    IsFromNewDevice = await IsNewDeviceAsync(userId, deviceInfo.DeviceFingerprint),
                    IsFromNewLocation = await IsNewLocationAsync(userId, location),
                    IsFromNewIp = await IsNewIpAsync(userId, ipAddress),
                    IsOffHoursLogin = IsOffHoursLogin(),
                    IsRapidLogin = await IsRapidLoginAsync(userId),
                    RiskScore = riskScore,
                    RiskLevel = GetRiskLevel(riskScore),
                    AdditionalInfo = JsonSerializer.Serialize(deviceInfo)
                };

                _context.LoginHistories.Add(loginHistory);
                await _context.SaveChangesAsync();

                // Create security alert if suspicious
                if (isSuspicious)
                {
                    await CreateSecurityAlertAsync(userId, "SuspiciousLogin", 
                        "Suspicious Login Detected", 
                        $"Login from {deviceInfo.DeviceType} at {location} with risk score {riskScore}", 
                        GetSeverityFromRisk(riskScore), 
                        JsonSerializer.Serialize(loginHistory));
                }

                // Update trusted device if successful
                if (isSuccessful)
                {
                    await UpdateOrCreateTrustedDeviceAsync(userId, deviceInfo, ipAddress, location);
                }

                return loginHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging login for user {UserId}", userId);
                throw;
            }
        }

        public async Task LogLogoutAsync(int userId, string sessionId)
        {
            try
            {
                var loginHistory = new LoginHistory
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Action = "Logout",
                    Timestamp = DateTime.UtcNow,
                    SessionId = sessionId,
                    IsSuccessful = true,
                    IsActiveSession = false
                };

                _context.LoginHistories.Add(loginHistory);

                // Update session expiry
                var session = await _context.UserSessions.FirstOrDefaultAsync(s => s.Token == sessionId);
                if (session != null)
                {
                    session.ExpiresAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging logout for user {UserId}", userId);
                throw;
            }
        }

        public async Task LogFailedLoginAsync(int userId, string ipAddress, string userAgent, string failureReason)
        {
            await LogLoginAsync(userId, ipAddress, userAgent, false, failureReason);

            // Check for multiple failed attempts
            var recentFailures = await _context.LoginHistories
                .Where(lh => lh.UserId == userId && 
                           lh.Action == "FailedLogin" && 
                           lh.Timestamp > DateTime.UtcNow.AddMinutes(15))
                .CountAsync();

            if (recentFailures >= 5)
            {
                await CreateSecurityAlertAsync(userId, "MultipleFailedAttempts", 
                    "Multiple Failed Login Attempts", 
                    $"User has {recentFailures} failed login attempts in the last 15 minutes", 
                    "High");

                // Auto-lock account
                await LockAccountAsync(userId, "Multiple failed login attempts", 30);
            }
        }

        public async Task<TrustedDevice?> GetTrustedDeviceAsync(int userId, string deviceFingerprint)
        {
            return await _context.TrustedDevices
                .FirstOrDefaultAsync(td => td.UserId == userId && td.DeviceFingerprint == deviceFingerprint);
        }

        public async Task<TrustedDevice> AddTrustedDeviceAsync(int userId, string deviceName, string deviceType, string operatingSystem, string browser, string userAgent, string? ipAddress, string? location)
        {
            var deviceFingerprint = GenerateDeviceFingerprint(userAgent, deviceType, operatingSystem, browser);
            
            var trustedDevice = new TrustedDevice
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                DeviceName = deviceName,
                DeviceType = deviceType,
                OperatingSystem = operatingSystem,
                Browser = browser,
                UserAgent = userAgent,
                IpAddress = ipAddress,
                Location = location,
                DeviceFingerprint = deviceFingerprint,
                FirstSeen = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow,
                LoginCount = 1,
                IsActive = true,
                TrustLevel = "Medium"
            };

            _context.TrustedDevices.Add(trustedDevice);
            await _context.SaveChangesAsync();

            return trustedDevice;
        }

        public async Task UpdateDeviceLastSeenAsync(Guid deviceId)
        {
            var device = await _context.TrustedDevices.FindAsync(deviceId);
            if (device != null)
            {
                device.LastSeen = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task BlockDeviceAsync(Guid deviceId, string reason)
        {
            var device = await _context.TrustedDevices.FindAsync(deviceId);
            if (device != null)
            {
                device.IsBlocked = true;
                device.BlockedAt = DateTime.UtcNow;
                device.BlockReason = reason;
                device.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsSuspiciousLoginAsync(int userId, string ipAddress, string userAgent, string? location)
        {
            var riskScore = await CalculateRiskScoreAsync(userId, ipAddress, location, GenerateDeviceFingerprint(userAgent, "", "", ""));
            return riskScore >= 70; // Threshold for suspicious activity
        }

        public async Task<int> CalculateRiskScoreAsync(int userId, string ipAddress, string? location, string? deviceFingerprint)
        {
            int score = 0;

            // Check for new device (30 points)
            if (!string.IsNullOrEmpty(deviceFingerprint))
            {
                var isNewDevice = await IsNewDeviceAsync(userId, deviceFingerprint);
                if (isNewDevice) score += 30;
            }

            // Check for new location (25 points)
            if (!string.IsNullOrEmpty(location))
            {
                var isNewLocation = await IsNewLocationAsync(userId, location);
                if (isNewLocation) score += 25;
            }

            // Check for new IP (20 points)
            var isNewIp = await IsNewIpAsync(userId, ipAddress);
            if (isNewIp) score += 20;

            // Check for off-hours login (15 points)
            if (IsOffHoursLogin()) score += 15;

            // Check for rapid logins (10 points)
            var isRapidLogin = await IsRapidLoginAsync(userId);
            if (isRapidLogin) score += 10;

            // Check for failed attempts in last hour (20 points)
            var recentFailures = await _context.LoginHistories
                .Where(lh => lh.UserId == userId && 
                           lh.Action == "FailedLogin" && 
                           lh.Timestamp > DateTime.UtcNow.AddHours(1))
                .CountAsync();
            if (recentFailures > 0) score += Math.Min(recentFailures * 5, 20);

            return Math.Min(score, 100);
        }

        public async Task<List<SecurityAlert>> GetActiveAlertsAsync(int userId)
        {
            return await _context.SecurityAlerts
                .Where(sa => sa.UserId == userId && !sa.IsResolved)
                .OrderByDescending(sa => sa.AlertTime)
                .ToListAsync();
        }

        public async Task CreateSecurityAlertAsync(int userId, string alertType, string title, string description, string severity, string? additionalData = null)
        {
            var alert = new SecurityAlert
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AlertType = alertType,
                Title = title,
                Description = description,
                Severity = severity,
                AlertTime = DateTime.UtcNow,
                IsResolved = false
            };

            if (additionalData != null)
            {
                alert.AdditionalData = additionalData;
            }

            _context.SecurityAlerts.Add(alert);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Security alert created for user {UserId}: {AlertType} - {Title}", userId, alertType, title);
        }

        public async Task<string?> GetLocationFromIpAsync(string ipAddress)
        {
            try
            {
                // For demo purposes, return a mock location
                // In production, use a real IP geolocation service like ipapi.co, ipgeolocation.io, etc.
                if (ipAddress == "127.0.0.1" || ipAddress == "::1")
                {
                    return "Local Development, Vietnam";
                }

                // Mock different locations for testing
                var mockLocations = new[]
                {
                    "Ho Chi Minh City, Vietnam",
                    "Hanoi, Vietnam",
                    "Da Nang, Vietnam",
                    "Bangkok, Thailand",
                    "Singapore, Singapore"
                };

                var random = new Random(ipAddress.GetHashCode());
                return mockLocations[random.Next(mockLocations.Length)];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting location for IP {IpAddress}", ipAddress);
                return null;
            }
        }

        public async Task<bool> IsNewLocationAsync(int userId, string? location)
        {
            if (string.IsNullOrEmpty(location)) return false;

            var hasPreviousLocation = await _context.LoginHistories
                .Where(lh => lh.UserId == userId && lh.Location == location)
                .AnyAsync();

            return !hasPreviousLocation;
        }

        public async Task<bool> IsNewIpAsync(int userId, string ipAddress)
        {
            var hasPreviousIp = await _context.LoginHistories
                .Where(lh => lh.UserId == userId && lh.IpAddress == ipAddress)
                .AnyAsync();

            return !hasPreviousIp;
        }

        public async Task<bool> IsValidSessionAsync(string sessionId)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.Token == sessionId && s.ExpiresAt > DateTime.UtcNow);

            return session != null;
        }

        public async Task InvalidateSessionAsync(string sessionId)
        {
            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.Token == sessionId);

            if (session != null)
            {
                session.ExpiresAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task InvalidateAllUserSessionsAsync(int userId)
        {
            var sessions = await _context.UserSessions
                .Where(s => s.UserId == userId && s.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var session in sessions)
            {
                session.ExpiresAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<LoginHistory>> GetLoginHistoryAsync(int userId, int page = 1, int pageSize = 20)
        {
            return await _context.LoginHistories
                .Where(lh => lh.UserId == userId)
                .OrderByDescending(lh => lh.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<LoginHistory>> GetSuspiciousActivitiesAsync(int userId)
        {
            return await _context.LoginHistories
                .Where(lh => lh.UserId == userId && !lh.IsSuccessful)
                .OrderByDescending(lh => lh.Timestamp)
                .ToListAsync();
        }

        public async Task<object> GetSecurityDashboardAsync(int userId)
        {
            var totalLogins = await _context.LoginHistories
                .Where(lh => lh.UserId == userId && lh.Action == "Login")
                .CountAsync();

            var suspiciousLogins = await _context.LoginHistories
                .Where(lh => lh.UserId == userId && !lh.IsSuccessful)
                .CountAsync();

            var activeAlerts = await _context.SecurityAlerts
                .Where(sa => sa.UserId == userId && !sa.IsResolved)
                .CountAsync();

            var trustedDevices = await _context.TrustedDevices
                .Where(td => td.UserId == userId && td.IsActive && !td.IsBlocked)
                .CountAsync();

            var recentLogins = await _context.LoginHistories
                .Where(lh => lh.UserId == userId)
                .OrderByDescending(lh => lh.Timestamp)
                .Take(10)
                .Select(lh => new
                {
                    lh.Timestamp,
                    lh.Action,
                    lh.IpAddress,
                    lh.Location,
                    lh.DeviceType,
                    lh.IsSuccessful,
                    lh.RiskLevel
                })
                .ToListAsync();

            return new
            {
                TotalLogins = totalLogins,
                SuspiciousLogins = suspiciousLogins,
                ActiveAlerts = activeAlerts,
                TrustedDevices = trustedDevices,
                RecentLogins = recentLogins
            };
        }

        public async Task<bool> ShouldLockAccountAsync(int userId)
        {
            var recentFailures = await _context.LoginHistories
                .Where(lh => lh.UserId == userId && 
                           lh.Action == "FailedLogin" && 
                           lh.Timestamp > DateTime.UtcNow.AddMinutes(15))
                .CountAsync();

            return recentFailures >= 5;
        }

        public async Task LockAccountAsync(int userId, string reason, int lockoutMinutes = 30)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
                await _context.SaveChangesAsync();

                await CreateSecurityAlertAsync(userId, "AccountLocked", 
                    "Account Temporarily Locked", 
                    $"Account locked due to: {reason}. Will be unlocked at {user.LockedUntil:yyyy-MM-dd HH:mm:ss} UTC", 
                    "High");
            }
        }

        public async Task UnlockAccountAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LockedUntil = null;
                await _context.SaveChangesAsync();
            }
        }

        // Helper methods
        private (string DeviceType, string OperatingSystem, string Browser, string DeviceName, string DeviceFingerprint) ParseUserAgent(string userAgent)
        {
            var deviceType = "Unknown";
            var operatingSystem = "Unknown";
            var browser = "Unknown";
            var deviceName = "Unknown";

            // Simple user agent parsing (in production, use a proper library)
            if (userAgent.Contains("Mobile") || userAgent.Contains("Android"))
            {
                deviceType = "Mobile";
                operatingSystem = "Android";
            }
            else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
            {
                deviceType = userAgent.Contains("iPad") ? "Tablet" : "Mobile";
                operatingSystem = "iOS";
            }
            else if (userAgent.Contains("Windows"))
            {
                deviceType = "Desktop";
                operatingSystem = "Windows";
            }
            else if (userAgent.Contains("Mac"))
            {
                deviceType = "Desktop";
                operatingSystem = "macOS";
            }

            if (userAgent.Contains("Chrome"))
                browser = "Chrome";
            else if (userAgent.Contains("Firefox"))
                browser = "Firefox";
            else if (userAgent.Contains("Safari"))
                browser = "Safari";
            else if (userAgent.Contains("Edge"))
                browser = "Edge";

            var deviceFingerprint = GenerateDeviceFingerprint(userAgent, deviceType, operatingSystem, browser);

            return (deviceType, operatingSystem, browser, deviceName, deviceFingerprint);
        }

        private string GenerateDeviceFingerprint(string userAgent, string deviceType, string operatingSystem, string browser)
        {
            var fingerprint = $"{deviceType}_{operatingSystem}_{browser}_{userAgent.GetHashCode()}";
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(fingerprint));
        }

        private async Task<bool> IsNewDeviceAsync(int userId, string deviceFingerprint)
        {
            var hasDevice = await _context.TrustedDevices
                .Where(td => td.UserId == userId && td.DeviceFingerprint == deviceFingerprint)
                .AnyAsync();

            return !hasDevice;
        }

        private bool IsOffHoursLogin()
        {
            var hour = DateTime.UtcNow.Hour;
            return hour < 6 || hour > 22; // Outside 6 AM - 10 PM UTC
        }

        private async Task<bool> IsRapidLoginAsync(int userId)
        {
            var recentLogins = await _context.LoginHistories
                .Where(lh => lh.UserId == userId && 
                           lh.Action == "Login" && 
                           lh.Timestamp > DateTime.UtcNow.AddMinutes(5))
                .CountAsync();

            return recentLogins > 3;
        }

        private string GetRiskLevel(int riskScore)
        {
            return riskScore switch
            {
                < 30 => "Low",
                < 50 => "Medium",
                < 70 => "High",
                _ => "Critical"
            };
        }

        private string GetSeverityFromRisk(int riskScore)
        {
            return riskScore switch
            {
                < 30 => "Low",
                < 50 => "Medium",
                < 70 => "High",
                _ => "Critical"
            };
        }

        private async Task UpdateOrCreateTrustedDeviceAsync(int userId, (string DeviceType, string OperatingSystem, string Browser, string DeviceName, string DeviceFingerprint) deviceInfo, string ipAddress, string? location)
        {
            var existingDevice = await _context.TrustedDevices
                .FirstOrDefaultAsync(td => td.UserId == userId && td.DeviceFingerprint == deviceInfo.DeviceFingerprint);

            if (existingDevice != null)
            {
                await UpdateDeviceLastSeenAsync(existingDevice.Id);
            }
            else
            {
                await AddTrustedDeviceAsync(userId, deviceInfo.DeviceName, deviceInfo.DeviceType, 
                    deviceInfo.OperatingSystem, deviceInfo.Browser, "", ipAddress, location);
            }
        }
    }
}
