using FertilizerWarehouseAPI.DTOs;

namespace FertilizerWarehouseAPI.Services;

public interface IAuthService
{
    System.Threading.Tasks.Task<AuthResponseDto> LoginAsync(LoginDto loginDto, string ipAddress, string userAgent);
    System.Threading.Tasks.Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgent);
    System.Threading.Tasks.Task<bool> LogoutAsync(string token, int userId);
    System.Threading.Tasks.Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    System.Threading.Tasks.Task<bool> ForgotPasswordAsync(string email);
    System.Threading.Tasks.Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    System.Threading.Tasks.Task<UserDto> GetUserProfileAsync(int userId);
    System.Threading.Tasks.Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto);
}
