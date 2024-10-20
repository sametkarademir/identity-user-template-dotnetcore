using System.Security.Authentication;
using System.Text;
using Application.Dtos.Auths;
using Application.Services.Interface;
using Core.Exceptions.Enums;
using Core.Exceptions.Types;
using Domain.Models;
using Domain.Shared.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Persistence.Repositories.Interface;

namespace Application.Services.Concrete;

public class AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IUow uow, IJwtService jwtService, IEventBus eventBus, IConfiguration config) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
    {
        var user = await userManager.FindByNameAsync(loginRequest.UserName);
        
        if (user == null)
            throw new AppBusinessException("User not found", 404, AppLogLevel.Warning);
        
        if (user.EmailConfirmed == false) 
            throw new AppBusinessException("User email not confirmed", 400, AppLogLevel.Warning);
        
        var result = await signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
        
        if (result.IsLockedOut)
            throw new AppBusinessException("User locked out", 400, AppLogLevel.Warning);
        
        if (!result.Succeeded)
        {
            // User has input an invalid password
            if (!user.UserName!.Equals("admin"))
            {
                // Increamenting AccessFailedCount of the AspNetUser by 1
                await userManager.AccessFailedAsync(user);
            }

            if (user.AccessFailedCount >= Convert.ToUInt16(config["Identity:AccessFailedCount"]))
            {
                // Lock the user for one day
                await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(1));
                throw new AppBusinessException("User locked out", 400, AppLogLevel.Warning);
            }

            throw new AppBusinessException("UserName or Password is incorrect", 400, AppLogLevel.Warning);
        }
        
        await userManager.ResetAccessFailedCountAsync(user);
        await userManager.SetLockoutEndDateAsync(user, null);
        
        return await CreateApplicationUserDto(user);
    }
    
    public async Task<LoginResponseDto> RefreshTokenAsync(string userId, string refreshToken)
    {
        if (IsValidRefreshTokenAsync(userId, refreshToken).GetAwaiter().GetResult())
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                throw new AuthenticationException();
            
            return await CreateApplicationUserDto(user);
        }
        
        throw new AuthenticationException();
    }
    
    public async Task<LoginResponseDto> RefreshPageAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        
        if (user == null)
            throw new AuthenticationException();

        if (await userManager.IsLockedOutAsync(user))
            throw new AuthenticationException();
        
        return await CreateApplicationUserDto(user);
    }

    public async Task RegisterAsync(RegisterRequestDto registerRequest)
    {
        if (await CheckUserNameExistsAsync(registerRequest.UserName))
            throw new AppBusinessException("UserName already exists", 400, AppLogLevel.Warning);

        if (await CheckEmailExistsAsync(registerRequest.Email))
            throw new AppBusinessException("Email already exists", 400, AppLogLevel.Warning);

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = registerRequest.UserName,
            Email = registerRequest.Email
        };
        
        var result = await userManager.CreateAsync(user, registerRequest.Password);
        
        if (!result.Succeeded)
            throw new AppBusinessException("User creation failed", 400, AppLogLevel.Warning);
        
        try
        {
            await SendConfirmEMailAsync(user);
        }
        catch(Exception)
        {
            throw new ArgumentException();
        }
    }

    public async Task ResendEmailConfirmationLinkAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        
        if (user == null)
            throw new AppBusinessException("User not found", 404, AppLogLevel.Warning);
        
        if (user.EmailConfirmed)
            throw new AppBusinessException("User email already confirmed", 400, AppLogLevel.Warning);
        
        try
        {
            await SendConfirmEMailAsync(user);
        }
        catch(Exception)
        {
            throw new ArgumentException();
        }
    }
    
    public async Task ConfirmEmailAsync(ConfirmEmailRequestDto confirmEmailRequest)
    {
        var user = await userManager.FindByEmailAsync(confirmEmailRequest.Email);
        
        if (user == null)
            throw new AppBusinessException("User not found", 404, AppLogLevel.Warning);
        
        if (user.EmailConfirmed)
            throw new AppBusinessException("User email already confirmed", 400, AppLogLevel.Warning);
        
        try
        {
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(confirmEmailRequest.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await userManager.ConfirmEmailAsync(user, decodedToken);
            
            if (!result.Succeeded)
                throw new ArgumentException();
            
        }
        catch(Exception)
        {
            throw new ArgumentException();
        }
    }

    public async Task ForgotUsernameOrPasswordAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        
        if (user == null)
            throw new AppBusinessException("User not found", 404, AppLogLevel.Warning);
        
        if (!user.EmailConfirmed)
            throw new AppBusinessException("User email not confirmed", 400, AppLogLevel.Warning);

        try
        {
            await SendForgotUsernameOrPasswordEmail(user);
        }
        catch (Exception)
        {
            throw new ArgumentException();
        }
    }

    public async Task ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequest)
    {
        var user = await userManager.FindByEmailAsync(resetPasswordRequest.Email);
        
        if (user == null)
            throw new AppBusinessException("User not found", 404, AppLogLevel.Warning);
        
        if (!user.EmailConfirmed)
            throw new AppBusinessException("User email not confirmed", 400, AppLogLevel.Warning);
        
        try
        {
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(resetPasswordRequest.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await userManager.ResetPasswordAsync(user, decodedToken, resetPasswordRequest.NewPassword);
            if (!result.Succeeded)
            {
                throw new ArgumentException();
            }
        }
        catch(Exception)
        {
            throw new ArgumentException();
        }
    }
    
    private async Task<LoginResponseDto> CreateApplicationUserDto(AppUser user)
    {
        var result = await SaveRefreshTokenAsync(user);
        return new LoginResponseDto
        {
            AccessToken = await jwtService.CreateJwtAsync(user),
            RefreshToken = result.Token,
            DateExpiresUtc = result.DateExpiresUtc
        };
    }
    private async Task<RefreshToken> SaveRefreshTokenAsync(AppUser user)
    {
        var refreshToken = jwtService.CreateRefreshToken(user);

        var existingRefreshToken = await uow.RefreshTokenRepository.GetAsync(x => x.UserId == user.Id);
        if (existingRefreshToken != null)
        {
            existingRefreshToken.Token = refreshToken.Token;
            existingRefreshToken.DateCreatedUtc = refreshToken.DateCreatedUtc;
            existingRefreshToken.DateExpiresUtc = refreshToken.DateExpiresUtc;
            await uow.RefreshTokenRepository.UpdateAsync(existingRefreshToken);
        }
        else
        {
            existingRefreshToken = await uow.RefreshTokenRepository.AddAsync(new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshToken.Token,
                DateCreatedUtc = refreshToken.DateCreatedUtc,
                DateExpiresUtc = refreshToken.DateExpiresUtc,
                UserId = user.Id,
            });
        }
        
        return existingRefreshToken;
    }
    private async Task<bool> IsValidRefreshTokenAsync(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return false;

        var fetchedRefreshToken = await uow.RefreshTokenRepository.GetAsync(x => x.UserId == userId && x.Token == token);
        if (fetchedRefreshToken == null) return false;
        if (fetchedRefreshToken.IsExpired) return false;

        return true;
    }
    private async Task<bool> CheckUserNameExistsAsync(string username)
    {
        var appUser = await userManager.FindByNameAsync(username);
        return appUser != null;
    }
    private async Task<bool> CheckEmailExistsAsync(string email)
    {
        var appUser =  await userManager.FindByEmailAsync(email);
        return appUser != null;
    }
    private async Task SendConfirmEMailAsync(AppUser appUser)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(appUser);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var url = $"{config["Identity:ClientUrl"]}/{config["Identity:ConfirmEmailPath"]}?token={token}&email={appUser.Email}";

        var body = $"<p>Hello: {appUser.UserName}</p>" +
                   "<p>Please confirm your email address by clicking on the following link.</p>" +
                   $"<p><a href=\"{url}\">Click here</a></p>" +
                   "<p>Thank you,</p>" +
                   $"<br>{config["Identity:ApplicationName"]}";

        eventBus.Publish(new MailSendRequestEvent(appUser.Email!, "Confirm your email address", body, appUser.UserName ?? string.Empty));
    }
    private async Task SendForgotUsernameOrPasswordEmail(AppUser appUser)
    {
        var token = await userManager.GeneratePasswordResetTokenAsync(appUser);
        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var url = $"{config["Identity:ClientUrl"]}/{config["Identity:ResetPasswordPath"]}?token={token}&email={appUser.Email}";

        var body = $"<p>Hello: {appUser.UserName}</p>" +
                   $"<p>Username: {appUser.UserName}.</p>" +
                   "<p>In order to reset your password, please click on the following link.</p>" +
                   $"<p><a href=\"{url}\">Click here</a></p>" +
                   "<p>Thank you,</p>" +
                   $"<br>{config["Identity:ApplicationName"]}";

        eventBus.Publish(new MailSendRequestEvent(appUser.Email!, "Forgot username or password", body, appUser.UserName ?? string.Empty));
    }
}