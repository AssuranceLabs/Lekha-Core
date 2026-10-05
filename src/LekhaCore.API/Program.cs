using LekhaCore.Api.Contracts;
using LekhaCore.Api.Extensions;
using LekhaCore.Application;
using LekhaCore.Domain.Common.Constants;
using LekhaCore.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail("Too many requests. Try again later.", "RATE_LIMIT"),
            token);
    };

    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

            return new BadRequestObjectResult(ApiResponse<object>.Fail(
                ValidationMessages.ValidationFailed,
                "VALIDATION_FAILED",
                errors));
        };
    });

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration
        .GetSection("AppCustomSettings:CorsOrigins")
        .Get<string[]>();

    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy
            .WithOrigins(allowedOrigins ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.MimeTypes = ["application/json"];
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Lekha Core API")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithClassicLayout()
            .ForceDarkMode()
            .SortTagsAlphabetically();
    });
}

app.ConfigureLekhaCorePipeline();
app.Run();

//if (!IsEfDesignTime())
//{
//    await app.SeedDatabaseAsync();
//    app.Run();
//}

//static bool IsEfDesignTime() =>
//    Environment.GetCommandLineArgs().Any(argument =>
//        argument.Contains("ef.dll", StringComparison.OrdinalIgnoreCase));
