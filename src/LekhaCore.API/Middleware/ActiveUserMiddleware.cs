using LekhaCore.Api.Contracts;
using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Application.Interfaces.IService;

namespace LekhaCore.Api.Middleware;

public class ActiveUserMiddleware(RequestDelegate next, ILogger<ActiveUserMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IUserRepository users, ICurrentUserService currentUser)
    {
        if (currentUser.IsAuthenticated)
        {
            if (currentUser.UserId is not Guid userId)
            {
                await WriteUnauthorizedAsync(context);
                return;
            }

            var user = await users.GetByPublicIdReadOnlyAsync(userId, context.RequestAborted);
            if (user is null || !user.IsActive)
            {
                logger.LogWarning("Rejected an authenticated request for inactive or unknown user {UserId}.", userId);
                await WriteUnauthorizedAsync(context);
                return;
            }
        }

        await next(context);
    }

    private static async Task WriteUnauthorizedAsync(HttpContext context)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Unauthorized.", "AUTH_401"));
    }
}
