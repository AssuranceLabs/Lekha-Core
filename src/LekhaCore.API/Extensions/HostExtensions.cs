using LekhaCore.Api.Middleware;

namespace LekhaCore.Api.Extensions;

public static class HostExtensions
{
    public static WebApplication ConfigureLekhaCorePipeline(this WebApplication app)
    {
        app.UseHttpsRedirection();

        app.UseCorrelationId();

        app.UseExceptionHandling();


        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();


        return app;
    }
}