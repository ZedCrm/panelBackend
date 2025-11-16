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

// === Add services to DI ===

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CRM API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});



builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();


// === CORS: Allow all origins (dev only) ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();;
    });
});

// === JWT Authentication ===
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourapp",
            ValidAudience = "yourapp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_at_least_16_chars"))
        };
    });

// === Register CRM Services (DbContext, Repos, Apps, etc.) ===
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=dev.db";

CRMBootstraper.AddCRMManagement(
    builder.Services,
    connectionString,
    connectionString.Contains("Data Source=dev.db") ? DbProvider.Sqlite : DbProvider.SqlServer
);

// === Custom Services ===
builder.Services.AddScoped<IFileService, API.utility.FileService>();
builder.Services.AddScoped<IPermissionDiscoveryService, PermissionDiscoveryService>();
builder.Services.AddScoped<PermissionSeeder>(); // Register seeder

var app = builder.Build();

// === Development-only middleware ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM API v1");
    c.RoutePrefix = "swagger";
});

    // Seed permissions from controllers (only in dev)
    using var scope = app.Services.CreateScope();
    var permissionSeeder = scope.ServiceProvider.GetRequiredService<PermissionSeeder>();
    await permissionSeeder.SeedAsync();
}
app.UseStaticFiles();
// === Global Middleware Pipeline ===
app.UseRouting();
app.UseCors("AllowAll");

// Log incoming requests
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

// Handle preflight (OPTIONS) requests
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

app.UseAuthentication();
app.UseTokenValidation(); // Custom middleware
app.UseAuthorization();

app.MapHub<ChatHub>("/chatHub");

// === Ensure uploads folder exists ===
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);

// === Background timer: Check inactive users every minute ===
var timer = new Timer(_ =>
{
    using var scope = app.Services.CreateScope();
    var statusService = scope.ServiceProvider.GetRequiredService<UserStatusService>();
    statusService.CheckInactive();
}, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

// === Map controllers ===
app.MapControllers();

// === Seed initial data (Users, Roles, etc.) ===
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MyContext>();
    var seeder = new DatabaseSeeder(context);
    seeder.SeedAll();
}

app.Run();