using LekhaCore.Api.Contracts;

namespace LekhaCore.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = ExceptionMapper.Map(exception);

        var traceId = context.TraceIdentifier;
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? traceId;

        LogException(exception, error.StatusCode, traceId, correlationId);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = error.StatusCode;

        var details = new Dictionary<string, object>
        {
            ["traceId"] = traceId,
            ["correlationId"] = correlationId
        };

        if (error.Errors is not null)
            details["errors"] = error.Errors;

        var response = ApiResponse<object>.Fail(
            error.Message,
            error.ErrorCode,
            details
        );

        await context.Response.WriteAsJsonAsync(response);
    }

    private void LogException(
        Exception exception,
        int statusCode,
        string traceId,
        string correlationId)
    {
        if (statusCode >= 500)
            _logger.LogError(exception,
                "[{TraceId}] [{CorrelationId}] Unhandled exception",
                traceId, correlationId);
        else
            _logger.LogWarning(exception,
                "[{TraceId}] [{CorrelationId}] Handled exception",
                traceId, correlationId);
    }
}