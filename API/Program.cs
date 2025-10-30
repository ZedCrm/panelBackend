using ConfApp;
using Infrastructure;
using Infrastructure.data.seed;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer; // اضافه شده
using Microsoft.IdentityModel.Tokens; // اضافه شده
using System.Text;
using API.Middleware; // اضافه شده

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// تنظیم احراز هویت JWT
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
        ValidIssuer = "yourapp", // باید با "iss" توکن مطابقت داشته باشد
        ValidAudience = "yourapp", // باید با "aud" توکن مطابقت داشته باشد
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_at_least_16_chars")) // کلید امضا
    };
});

if (false)
{
    CRMBootstraper.AddCRMManagement(builder.Services, "Data Source=dev.db", DbProvider.Sqlite);
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    CRMBootstraper.AddCRMManagement(builder.Services, connectionString, DbProvider.SqlServer);
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Middleware برای لاگ کردن درخواست‌ها
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Received {Method} request at {Url}", context.Request.Method, context.Request.Path);
    await next();
    logger.LogInformation("Response Status Code: {StatusCode}", context.Response.StatusCode);
});

// Middleware برای هندل کردن درخواست‌های Preflight (OPTIONS)
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

builder.Services.AddHttpContextAccessor();
app.UseHttpsRedirection();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MyContext>();
    var seeder = new DatabaseSeeder(context);
    seeder.SeedAll();
}

app.Run();