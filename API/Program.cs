using API.Hubs;
using API.Middleware;
using API.Services;
using API.utility;
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
// REGION 1: Configuration
// ========================================================================

// خواندن تنظیمات JWT از appsettings.json (بعداً اضافه می‌کنیم)
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "yourapp";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "yourapp";
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "your_secret_key_at_least_16_chars";

// ========================================================================
// REGION 2: Service Registration
// ========================================================================

builder.Services.AddControllers(options =>
{
    // اضافه کردن ResultFilter به صورت Global
    options.Filters.Add<ResultFilter>();
});
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CRM API",
        Version = "v1",
        Description = "API مدیریت مشتریان"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "توکن خود را وارد کنید: Bearer {your_token}"
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
            Array.Empty<string>()
        }
    });
});

// SignalR for Real-time
builder.Services.AddSignalR();

// HttpContext Accessor
builder.Services.AddHttpContextAccessor();

// ========================================================================
// REGION 3: CORS Configuration
// ========================================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// ========================================================================
// REGION 4: JWT Authentication
// ========================================================================

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
        };

        // برای SignalR - دریافت توکن از QueryString
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// ========================================================================
// REGION 5: Database and Infrastructure (یکبار و درست)
// ========================================================================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=dev.db";

var dbProvider = connectionString.Contains("Data Source=dev.db")
    ? DbProvider.Sqlite
    : DbProvider.SqlServer;

CRMBootstraper.AddCRMManagement(builder.Services, connectionString, dbProvider);

// ثبت سرویس‌های اضافی که در CRMBootstraper نیستند
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IPermissionDiscoveryService, PermissionDiscoveryService>();
builder.Services.AddScoped<PermissionSeeder>();

// ========================================================================
// REGION 6: Build App
// ========================================================================

var app = builder.Build();

// ========================================================================
// REGION 7: Development Environment
// ========================================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM API V1");
        c.RoutePrefix = "swagger";
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.DefaultModelsExpandDepth(0);
        c.EnableTryItOutByDefault();
    });

    // Seed permissions فقط در محیط توسعه
    using (var scope = app.Services.CreateScope())
    {
        var permissionSeeder = scope.ServiceProvider.GetRequiredService<PermissionSeeder>();
        await permissionSeeder.SeedAsync();
    }
}

// ========================================================================
// REGION 8: Middleware Pipeline
// ========================================================================

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAngular");

// Custom middleware
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

// WebSocket Token Extraction (توسط JWT Events مدیریت شد، این middleware اضافی است)
// اما برای اطمینان بیشتر نگه می‌داریم
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/chatHub"))
    {
        var accessToken = context.Request.Query["access_token"].FirstOrDefault();
        if (!string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(context.Request.Headers.Authorization))
        {
            context.Request.Headers.Authorization = $"Bearer {accessToken}";
        }
    }
    await next();
});

app.UseAuthentication();
app.UseTokenValidation();
app.UseAuthorization();

// ========================================================================
// REGION 9: Hubs & Controllers
// ========================================================================

app.MapHub<ChatHub>("/chatHub");
app.MapControllers();

// ========================================================================
// REGION 10: File System Setup
// ========================================================================

var uploadsPath = Path.Combine(app.Environment.WebRootPath ?? Directory.GetCurrentDirectory(), "uploads");
Directory.CreateDirectory(uploadsPath);

// ========================================================================
// REGION 11: Database Seeding (یکبار در انتها)
// ========================================================================

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MyContext>();

    // اعمال خودکار Migrationها
    await context.Database.MigrateAsync();

    // Seed کردن داده‌های اولیه (کاربر، نقش‌ها)
    var seeder = new DatabaseSeeder(context);
    seeder.SeedAll();
}

// ========================================================================
// REGION 12: Run Application
// ========================================================================

app.Run();