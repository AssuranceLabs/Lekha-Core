using LekhaCore.Api.Middleware;

namespace LekhaCore.Api.Extensions;

public static class HostExtensions
{
    public static WebApplication ConfigureLekhaCorePipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseSecurityHeaders();
        app.UseResponseCompression();
        app.UseCors("AllowSpecificOrigins");
        app.UseExceptionHandling();
        app.UseCorrelationId();
        app.UseRouting();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseActiveUser();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }
}
