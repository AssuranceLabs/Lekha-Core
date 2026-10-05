using LekhaCore.Application.DTOs;
using LekhaCore.Application.DTOs.Auth;
using LekhaCore.Application.Interfaces;
using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Application.Options;
using LekhaCore.Domain.Common;
using LekhaCore.Domain.Common.Constants;
using LekhaCore.Domain.Entities;
using Microsoft.Extensions.Options;

namespace LekhaCore.Application.Services;

public class AuthService(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IUnitOfWork unitOfWork,
    IWorkContext workContext,
    IClientInfoProvider clientInfo,
    ITokenHasher tokenHasher,
    IPermissionService permissions,
    IOptions<JwtSettings> jwtOptions) : IAuthService
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private static readonly Error InvalidCredentials =
        Error.Unauthorized("Auth.InvalidCredentials", AuthMessages.InvalidCredentials);

    private static readonly Error AccountLocked =
        Error.Unauthorized("Auth.Locked", AuthMessages.AccountLocked);

    private static readonly Error AccountDisabled =
        Error.Forbidden("Auth.Disabled", AuthMessages.AccountDisabled);

    private static readonly Error InvalidRefreshToken =
        Error.Unauthorized("Auth.RefreshInvalid", ValidationMessages.RefreshTokenInvalid);

    public async Task<Result<TokenResponseDto>> LoginAsync(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await users.GetByEmailTrackedAsync(email);

        if (user is null)
        {
            passwordHasher.VerifyUnknownUser(request.Password);
            return Result<TokenResponseDto>.Failure(InvalidCredentials);
        }

        if (user.LockoutEndUtc is DateTime lockedUntil && lockedUntil > DateTime.UtcNow)
            return Result<TokenResponseDto>.Failure(AccountLocked);

        workContext.SetCurrentUser(user.GUID);

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= MaxFailedAttempts)
            {
                user.LockoutEndUtc = DateTime.UtcNow.Add(LockoutDuration);
                user.FailedLoginAttempts = 0;
            }

            await unitOfWork.CommitAsync();
            return Result<TokenResponseDto>.Failure(InvalidCredentials);
        }

        if (!user.IsActive)
            return Result<TokenResponseDto>.Failure(AccountDisabled);

        user.FailedLoginAttempts = 0;
        user.LockoutEndUtc = null;
        user.LastLoginAt = DateTime.UtcNow;

        var response = await IssueTokensAsync(user);
        return Result<TokenResponseDto>.Success(response);
    }

    public async Task<Result<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var stored = await refreshTokens.GetByHashAsync(tokenHasher.Hash(request.RefreshToken));
        if (stored is null || stored.Expires <= DateTime.UtcNow)
            return Result<TokenResponseDto>.Failure(InvalidRefreshToken);

        workContext.SetCurrentUser(stored.UserId);

        if (stored.IsRevoked)
        {
            await refreshTokens.RevokeAllForUserAsync(stored.UserId, DateTime.UtcNow);
            await unitOfWork.CommitAsync();
            return Result<TokenResponseDto>.Failure(InvalidRefreshToken);
        }

        var user = await users.GetByPublicIdReadOnlyAsync(stored.UserId);
        if (user is null || !user.IsActive)
        {
            stored.IsRevoked = true;
            stored.RevokedAt = DateTime.UtcNow;
            await unitOfWork.CommitAsync();
            return Result<TokenResponseDto>.Failure(InvalidRefreshToken);
        }

        var response = await RotateAsync(stored, user);
        return Result<TokenResponseDto>.Success(response);
    }

    public async Task<Result> LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Success();

        var stored = await refreshTokens.GetByHashAsync(tokenHasher.Hash(refreshToken));
        if (stored is null || stored.IsRevoked)
            return Result.Success();

        stored.IsRevoked = true;
        stored.RevokedAt = DateTime.UtcNow;
        await unitOfWork.CommitAsync();
        return Result.Success();
    }

    private async Task<TokenResponseDto> IssueTokensAsync(User user)
    {
        var (accessToken, expiresAt) = jwtTokenGenerator.GenerateToken(user);
        var rawRefreshToken = refreshTokenGenerator.GenerateRefreshToken();
        var now = DateTime.UtcNow;

        await refreshTokens.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.GUID,
            TokenHash = tokenHasher.Hash(rawRefreshToken),
            Created = now,
            Expires = now.AddDays(jwtOptions.Value.RefreshTokenDays),
            CreatedByIp = ClientIp()
        });

        await unitOfWork.CommitAsync();
        return await CreateTokenResponseAsync(user, accessToken, rawRefreshToken, expiresAt);
    }

    private async Task<TokenResponseDto> RotateAsync(RefreshToken stored, User user)
    {
        var (accessToken, expiresAt) = jwtTokenGenerator.GenerateToken(user);
        var rawRefreshToken = refreshTokenGenerator.GenerateRefreshToken();
        var hash = tokenHasher.Hash(rawRefreshToken);
        var now = DateTime.UtcNow;

        stored.IsRevoked = true;
        stored.RevokedAt = now;
        stored.ReplacedByTokenHash = hash;

        await refreshTokens.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.GUID,
            TokenHash = hash,
            Created = now,
            Expires = now.AddDays(jwtOptions.Value.RefreshTokenDays),
            CreatedByIp = ClientIp()
        });

        await unitOfWork.CommitAsync();
        return await CreateTokenResponseAsync(user, accessToken, rawRefreshToken, expiresAt);
    }

    private async Task<TokenResponseDto> CreateTokenResponseAsync(
        User user,
        string accessToken,
        string refreshToken,
        DateTime expiresAt)
    {
        var roles = await permissions.GetRoleNamesAsync(user.GUID);
        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAtUtc = expiresAt,
            Roles = roles,
            FullName = user.FullName
        };
    }

    private string? ClientIp()
    {
        var ipAddress = clientInfo.ClientIpAddress;
        return string.IsNullOrWhiteSpace(ipAddress) ? null : ipAddress;
    }
}
