using System.Reflection;
using System.Text;
using Application.Services.Concrete;
using Application.Services.Interface;
using Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence.Contexts;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationExtensions(this IServiceCollection services, IConfiguration config)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddScoped<IJwtService, JwtService>();
        services.AddTransient<IMailKitService, MailKitService>();
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAppRoleService, AppRoleService>();
        services.AddScoped<IAppUserRoleService, AppUserRoleService>();
        services.AddScoped<IAppUserService, AppUserService>();
        
        // defining our IdentityCore Service
        services.AddIdentityCore<AppUser>(options =>
            {
                // password configuration
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;

                // for email confirmation
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<AppRole>() // be able to add roles
            .AddRoleManager<RoleManager<AppRole>>() // be able to make use of RoleManager
            .AddEntityFrameworkStores<BaseDbContext>() // providing our context
            .AddSignInManager<SignInManager<AppUser>>() // make use of Signin manager
            .AddUserManager<UserManager<AppUser>>() // make use of UserManager to create users
            .AddDefaultTokenProviders(); // be able to create tokens for email confirmation

        // be able to authenticate users using JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // validate the token based on the key we have provided inside appsettings.development.json JWT:Key
                    ValidateIssuerSigningKey = true,
                    // the issuer singning key based on JWT:Key
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"] ?? string.Empty)),
                    // the issuer which in here is the api project url we are using
                    ValidIssuer = config["JWT:Issuer"],
                    // validate the issuer (who ever is issuing the JWT)
                    ValidateIssuer = true,
                    // don't validate audience (angular side)
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        return services;
    }
}