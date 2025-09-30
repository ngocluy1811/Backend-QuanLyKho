using FertilizerWarehouseAPI.Models.Entities;

namespace FertilizerWarehouseAPI.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    int? GetUserIdFromToken(string token);
    System.Threading.Tasks.Task<bool> IsTokenBlacklistedAsync(string token);
    System.Threading.Tasks.Task BlacklistTokenAsync(string token, DateTime expiry);
}
