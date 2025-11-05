using Infrastructure;
using Infrastructure.data.seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Middleware;
using App.Object.Base.Users;
using ConfApp;

var builder = WebApplication.CreateBuilder(args);

// === CORS ===
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// === سرویس‌های اصلی ===
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// === احراز هویت JWT ===
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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

// === ثبت تمام سرویس‌های CRM (شامل FileService, UserStatusService, DbContext و ...) ===
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Data Source=dev.db";

CRMBootstraper.AddCRMManagement(
    builder.Services,
    connectionString,
    connectionString.Contains("Data Source=dev.db") ? DbProvider.Sqlite : DbProvider.SqlServer
);
builder.Services.AddScoped<App.utility.IFileService, API.utility.FileService>();
var app = builder.Build();

// === Middleware Pipeline ===
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// لاگ درخواست
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Received {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

// Preflight
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

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseTokenValidation();
app.UseAuthorization();

// === ایجاد فولدر uploads ===
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
    Directory.CreateDirectory(uploadsPath);

// === تایمر چک وضعیت کاربران (هر 1 دقیقه) ===
var timer = new Timer(_ =>
{
    var statusService = app.Services.GetRequiredService<UserStatusService>();
    statusService.CheckInactive();
}, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

app.MapControllers();

// === Seed دیتابیس ===
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MyContext>();
    var seeder = new DatabaseSeeder(context);
    seeder.SeedAll();
}

app.Run();