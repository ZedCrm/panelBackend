using API.Hubs;
using API.Middleware;
using API.Services;
using App.Contracts.Object.Base.auth;
using App.Object.Base.Users;
using App.utility;
using ConfApp;
using Infrastructure;
using Infrastructure.data.seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========================================================================
// REGION 1: Dependency Injection - Service Registration
// ========================================================================

#region Service Registration

// Add controllers and API exploration services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI for API documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "CRM API", 
        Version = "v1",
        Description = "API مدیریت مشتریان - برای تست ابتدا روی دکمه Authorize کلیک کنید"
    });

    // Change to HTTP Bearer authentication instead of OAuth2
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token. Example: Bearer abc123xyz"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer" 
                }
            },
            new[] { "read", "write" }
        }
    });

    c.TagActionsBy(api =>
    {
        if (api.ActionDescriptor.DisplayName.Contains("AuthController"))
            return new[] { "Authentication" };
        return new[] { api.ActionDescriptor.RouteValues["controller"] };
    });
});

// Add real-time communication support
builder.Services.AddSignalR();

// Enable access to HttpContext in services
builder.Services.AddHttpContextAccessor();

#endregion

// ========================================================================
// REGION 2: CORS Configuration
// ========================================================================

#region CORS Configuration

// Allow specific origin for Angular development server
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4200")  // Angular default dev port
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

#endregion

// ========================================================================
// REGION 3: JWT Authentication Setup
// ========================================================================

#region JWT Authentication

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,               // Ensure token comes from trusted issuer
            ValidateAudience = true,             // Ensure token is for this audience
            ValidateLifetime = true,             // Check token expiration
            ValidateIssuerSigningKey = true,     // Verify signature
            ValidIssuer = "yourapp",             // Issuer identifier
            ValidAudience = "yourapp",           // Audience identifier
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_at_least_16_chars"))
        };
    });

#endregion

// ========================================================================
// REGION 4: Database and Infrastructure Setup
// ========================================================================

#region Database & Infrastructure

// Get connection string from configuration or use SQLite as fallback
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=dev.db";

// Initialize CRM management with appropriate database provider
CRMBootstraper.AddCRMManagement(
    builder.Services,
    connectionString,
    connectionString.Contains("Data Source=dev.db") ? DbProvider.Sqlite : DbProvider.SqlServer
);

#endregion

// ========================================================================
// REGION 5: Custom Application Services
// ========================================================================

#region Custom Services

// File handling service for uploads/downloads
builder.Services.AddScoped<IFileService, API.utility.FileService>();

// Permission discovery service to find all permissions from controllers
builder.Services.AddScoped<IPermissionDiscoveryService, PermissionDiscoveryService>();

// Database seeder for permissions
builder.Services.AddScoped<PermissionSeeder>();

#endregion

var app = builder.Build();

// ========================================================================
// REGION 6: Development Environment Configuration
// ========================================================================

#region Development Configuration

if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI for API testing
    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM API V1");
    c.RoutePrefix = "swagger";
});

    // Seed permissions from controller attributes (development only)
    using var scope = app.Services.CreateScope();
    var permissionSeeder = scope.ServiceProvider.GetRequiredService<PermissionSeeder>();
    await permissionSeeder.SeedAsync();
}

#endregion

// ========================================================================
// REGION 7: Middleware Pipeline
// ========================================================================

#region Static Files & Routing

app.UseStaticFiles();           // Serve static files from wwwroot
app.UseRouting();               // Enable routing

#endregion

#region CORS Middleware

app.UseCors("AllowAll");        // Apply CORS policy

#endregion

#region Request Logging Middleware

// Log all incoming requests and outgoing responses
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

#endregion

#region Preflight Request Handling

// Handle CORS preflight (OPTIONS) requests
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});

#endregion

#region WebSocket Token Extraction

// Extract JWT token from query string for SignalR WebSocket connections
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/chatHub"))
    {
        var accessToken = context.Request.Query["access_token"].FirstOrDefault();
        if (!string.IsNullOrEmpty(accessToken))
        {
            context.Request.Headers.Authorization = $"Bearer {accessToken}";
        }
    }
    await next();
});

#endregion

#region Authentication & Authorization

app.UseAuthentication();        // Validate JWT tokens
app.UseTokenValidation();       // Custom token validation middleware
app.UseAuthorization();         // Authorize based on roles/policies

#endregion

// ========================================================================
// REGION 8: Real-time Communication Hubs
// ========================================================================

#region SignalR Hubs

// Map ChatHub for real-time messaging
app.MapHub<ChatHub>("/chatHub");

#endregion

// ========================================================================
// REGION 9: File System Setup
// ========================================================================

#region File System Setup

// Ensure uploads directory exists for file storage
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);

#endregion

// ========================================================================
// REGION 10: Background Services
// ========================================================================

#region Background Services

// Timer to check for inactive users every minute
var timer = new Timer(_ =>
{
    using var scope = app.Services.CreateScope();
    var statusService = scope.ServiceProvider.GetRequiredService<UserStatusService>();
    statusService.CheckInactive();  // Mark users as offline if inactive
}, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

#endregion

// ========================================================================
// REGION 11: Controller Mapping
// ========================================================================

#region Controller Mapping

app.MapControllers();   // Map all API controllers

#endregion

// ========================================================================
// REGION 12: Database Seeding
// ========================================================================

#region Database Seeding

// Seed initial data (Users, Roles, Permissions) on application startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MyContext>();
    var seeder = new DatabaseSeeder(context);
    seeder.SeedAll();   // Populate database with default data
}

#endregion

// ========================================================================
// REGION 13: Application Execution
// ========================================================================

app.Run();  // Start the application