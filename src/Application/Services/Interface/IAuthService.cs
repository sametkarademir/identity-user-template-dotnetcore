using Application.Dtos.Auths;

namespace Application.Services.Interface;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto> RefreshTokenAsync(string userId, string refreshToken);
    Task<LoginResponseDto> RefreshPageAsync(string userId);
    Task RegisterAsync(RegisterRequestDto request);
    Task ResendEmailConfirmationLinkAsync(string email);
    Task ConfirmEmailAsync(ConfirmEmailRequestDto request);
    Task ForgotUsernameOrPasswordAsync(string email);
    Task ResetPasswordAsync(ResetPasswordRequestDto request);
}