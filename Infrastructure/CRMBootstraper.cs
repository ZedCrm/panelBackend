using App.Contracts.Object.Base;
using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.auth.UserContext;
using App.Contracts.Object.Base.Users;
using App.Contracts.Object.Chat;
using App.Contracts.Object.Shop.CountTypeCon;
using App.Contracts.Object.Shop.InvCon;
using App.Contracts.Object.Shop.ProductCon;
using App.Object.Base;
using App.Object.Base.auth;
using App.Object.Base.auth.UserContext;
using App.Object.Base.Auth;
using App.Object.Base.Users;
using App.Object.Chat;
using App.Object.Chat.MessageApp;
using App.Object.Shop.CountTypeApp;
using App.Object.Shop.InvApp;
using App.Object.Shop.ProductApp;
using App.utility; // فقط IFileService
using ConfApp;
using ConfApp.Rep;
using ConfApp.Rep.bases;
using ConfApp.Rep.Chat;
using ConfApp.Rep.Inv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class CRMBootstraper
    {
        public static void AddCRMManagement(IServiceCollection services, string connectionString, DbProvider dbProvider)
        {


        
            // === Repository ها ===
            services.AddScoped<IPersonRep, PersonRep>();
            services.AddScoped<IProductRep, ProductRep>();
            services.AddScoped<ICountTypeRep, CountTypeRep>();
            services.AddScoped<IInvRep, InvRep>();
            services.AddScoped<IUserRepository, UserRep>();
            services.AddScoped<IPermissionRep, PermissionRepository>();
            services.AddScoped<IMyUserRepository, MyuserRepo>();
            services.AddScoped<IRoleRep, RoleRepo>();

            // === App Service ها ===
            services.AddScoped<IPersonApp, PersonApp>();
            services.AddScoped<IProductApp, ProductApp>();
            services.AddScoped<ICountTypeApp, CountTypeApp>();
            services.AddScoped<IInvApp, InvApp>();
            services.AddScoped<IAuthApp, AuthApp>();
            services.AddScoped<ITokenApp, TokenApp>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IUsersApp, UsersApp>();

            services.AddScoped<IMessageRep, MessageRep>();
            services.AddScoped<IChatApp, ChatApp>();

            // === سرویس‌های مشترک ===
            services.AddSingleton<UserStatusService>();
            // IFileService فقط در Program.cs ثبت می‌شه

            // === HttpContextAccessor ===
            services.AddHttpContextAccessor();

            // === UserContext ===
            services.AddScoped<IUserContext, UserContext>();

            // === AutoMapper ===
            services.AddAutoMapper(typeof(ClassMapping));
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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