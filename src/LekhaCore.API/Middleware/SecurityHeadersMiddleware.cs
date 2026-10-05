namespace LekhaCore.Api.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["Referrer-Policy"] = "no-referrer";
        headers["X-Permitted-Cross-Domain-Policies"] = "none";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
        headers["Content-Security-Policy"] = IsApiDocumentation(context.Request.Path)
            ? "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; font-src 'self' data:; connect-src 'self'; worker-src 'self' blob:; frame-ancestors 'none'"
            : "default-src 'none'; frame-ancestors 'none'";
        headers["X-Frame-Options"] = "DENY";
        headers["Cache-Control"] = "no-store";
        headers.Remove("Server");
        headers.Remove("X-Powered-By");

        await next(context);
    }

    private static bool IsApiDocumentation(PathString path) =>
        path.StartsWithSegments("/scalar") || path.StartsWithSegments("/openapi");
}
