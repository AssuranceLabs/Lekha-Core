using LekhaCore.Application.Interfaces;
using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Application.Options;
using LekhaCore.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace LekhaCore.Infrastructure.Extensions;

public static class AuthenticationDependencyInjection
{
    public static IServiceCollection AddLekhaCoreAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
        if (jwtSettings is null ||
            string.IsNullOrWhiteSpace(jwtSettings.Key) ||
            Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
        {
            throw new InvalidOperationException(
                "JwtSettings:Key is required and must be at least 32 bytes. Set it with the JwtSettings__Key environment variable outside Development.");
        }

        if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) || string.IsNullOrWhiteSpace(jwtSettings.Audience))
            throw new InvalidOperationException("JwtSettings:Issuer and JwtSettings:Audience are required.");

        if (jwtSettings.ExpiryMinutes <= 0 || jwtSettings.RefreshTokenDays <= 0)
            throw new InvalidOperationException("JwtSettings token lifetimes must be greater than zero.");

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IClientInfoProvider, ClientInfoProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddScoped<ITokenHasher, TokenHasher>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = System.Security.Claims.ClaimTypes.Name,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            isSuccess = false,
                            message = "Unauthorized.",
                            errorCode = "AUTH_401"
                        }));
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            isSuccess = false,
                            message = "Forbidden.",
                            errorCode = "AUTH_403"
                        }));
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}
