using Infrastructure;
using Microsoft.Extensions.Logging;

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
                   //.AllowCredentials(); // اگر احراز هویت (مثل JWT) ندارید، این خط را حذف کنید
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
    // Log Request Method و URL
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Received {Method} request at {Url}", context.Request.Method, context.Request.Path);

    // ادامه پردازش درخواست
    await next();

    // Log Response Status Code
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

// فعال‌سازی CORS بعد از UseRouting
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.Run();
