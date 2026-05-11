using App.Contracts.Object.Base;
using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.auth.UserContext;
using App.Contracts.Object.Base.Roles;
using App.Contracts.Object.Base.Users;
using App.Contracts.Object.Chat;
using App.Contracts.Object.Shop.CountTypeCon;
using App.Contracts.Object.Shop.InvCon;
using App.Contracts.Object.Shop.ProductCon;
using App.Object.Base;
using App.Object.Base.auth;
using App.Object.Base.auth.UserContext;
using App.Object.Base.Auth;
using App.Object.Base.Roles;
using App.Object.Base.Users;
using App.Object.Chat;
using App.Object.Shop.CountTypeApp;
using App.Object.Shop.InvApp;
using App.Object.Shop.ProductApp;
using ConfApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class CRMBootstraper
    {
        public static void AddCRMManagement(IServiceCollection services, string connectionString, DbProvider dbProvider)
        {
            // === App Service ها ===
            services.AddScoped<IPersonApp, PersonApp>();
            services.AddScoped<IProductApp, ProductApp>();
            services.AddScoped<ICountTypeApp, CountTypeApp>();
            services.AddScoped<IInvApp, InvApp>();
            services.AddScoped<IAuthApp, AuthApp>();
            services.AddScoped<ITokenApp, TokenApp>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUsersApp, UsersApp>();
            services.AddScoped<IChatApp, ChatApp>();
            services.AddScoped<IRoleApp, RoleApp>();



            // Business Services
            services.AddScoped<UserBusinessService>();
            services.AddScoped<RoleBusinessService>();
            services.AddScoped<ProductBusinessService>();
            services.AddScoped<CountTypeBusinessService>();
            services.AddScoped<PersonBusinessService>();

            // === سرویس‌های مشترک ===
            services.AddSingleton<UserStatusService>();

            // === HttpContextAccessor ===
            services.AddHttpContextAccessor();

            // === UserContext ===
            services.AddScoped<IUserContext, UserContext>();

            // === AutoMapper (فقط یکبار) ===
            services.AddAutoMapper(typeof(ClassMapping));

            // === DbContext ===
            services.AddDbContext<MyContext>(options =>
            {
                if (dbProvider == DbProvider.SqlServer)
                {
                    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Infrastructure"))
                           .EnableSensitiveDataLogging()
                           .LogTo(Console.WriteLine);
                }
                else if (dbProvider == DbProvider.Sqlite)
                {
                    options.UseSqlite(connectionString, b => b.MigrationsAssembly("Infrastructure"))
                           .EnableSensitiveDataLogging()
                           .LogTo(Console.WriteLine);
                }
            }, ServiceLifetime.Scoped);
        }
    }

    public enum DbProvider
    {
        SqlServer,
        Sqlite
    }
}