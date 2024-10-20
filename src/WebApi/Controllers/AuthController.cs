using Application;
using Application.Dtos.Auths;
using Application.Services.Interface;
using Core.Exceptions.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    
    [ProducesResponseType<LoginResponseDto>(200)]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        var response = await authService.LoginAsync(loginRequest);
        
        // var cookieOptions = new CookieOptions
        // {
        //     Expires = response.DateExpiresUtc,
        //     IsEssential = true,
        //     HttpOnly = true,
        // };

        //Response.Cookies.Append("Personel_Assistant", JsonSerializer.Serialize(response), cookieOptions);
        return Ok(response);
    }
    
    [ProducesResponseType<LoginResponseDto>(200)]
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> RefreshPageAsync()
    {
        var userId = User.GetUserId();
        
        if (userId == null)
            throw new AppAuthenticationException();
        
        var response = await authService.RefreshPageAsync(userId);

        return Ok(response);
    }
    
    [ProducesResponseType<LoginResponseDto>(200)]
    [HttpGet("{refreshToken}")]
    [Authorize]
    public async Task<IActionResult> RefreshTokenAsync(string refreshToken)
    {
        var userId = User.GetUserId();
        
        if (userId == null)
            throw new AppAuthenticationException();
        
        return Ok(await authService.RefreshTokenAsync(userId, refreshToken));
    }
    
    [FeatureToggle("FeatureToggles:IsRegisterActionEnabled")]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto registerRequest)
    {
        await authService.RegisterAsync(registerRequest);
        return NoContent();
    }
    
    [FeatureToggle("FeatureToggles:IsConfirmEmailActionEnabled")]
    [HttpGet("resend-email-confirmation-link/{email}")]
    public async Task<IActionResult> ResendEmailConfirmationLinkAsync(string email)
    {
        await authService.ResendEmailConfirmationLinkAsync(email);
        return NoContent();
    }
    
    [FeatureToggle("FeatureToggles:IsConfirmEmailActionEnabled")]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmailAsync(ConfirmEmailRequestDto confirmEmailRequest)
    {
        await authService.ConfirmEmailAsync(confirmEmailRequest);
        return NoContent();
    }
    
    [FeatureToggle("FeatureToggles:IsForgotUsernameOrPasswordActionEnabled")]
    [HttpGet("forgot-password/{email}")]
    public async Task<IActionResult> ForgotUsernameOrPasswordAsync(string email)
    {
        await authService.ForgotUsernameOrPasswordAsync(email);
        return NoContent();
    }
    
    [FeatureToggle("FeatureToggles:IsResetPasswordActionEnabled")]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest)
    {
        await authService.ResetPasswordAsync(resetPasswordRequest);
        return NoContent();
    }
}