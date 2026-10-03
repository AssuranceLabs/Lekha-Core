using Microsoft.AspNetCore.ResponseCompression;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CORS
builder.Services.AddCors(options =>
{
    var allowedOrigins =
        builder.Configuration
            .GetSection("AppCustomSettings:CorsOrigins")
            .Get<string[]>();

    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy
            .WithOrigins(allowedOrigins ?? Array.Empty<string>())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;

    options.MimeTypes = new[]
    {
        "application/json"
    };

    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});


var app = builder.Build();

// HTTPS in ALL environments
app.UseHttpsRedirection();

// HSTS in non-development environments
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Response compression
app.UseResponseCompression();

// CORS
app.UseCors("AllowSpecificOrigins");

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

app.UseAuthorization();

app.MapControllers();

app.Run();
