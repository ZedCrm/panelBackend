using App;
using App.Contracts.Object.Base;
using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.Users;
using App.Contracts.Object.Shop.CountTypeCon;
using App.Contracts.Object.Shop.InvCon;
using App.Contracts.Object.Shop.ProductCon;
using App.Object.Base;
using App.Object.Base.auth;
using App.Object.Base.Auth;
using App.Object.Base.Users;
using App.Object.Shop.CountTypeApp;
using App.Object.Shop.invApp;
using App.Object.Shop.ProductApp;
using App.utility;
using AutoMapper;
using ConfApp;
using ConfApp.Rep;
using ConfApp.Rep.bases;
using ConfApp.Rep.Inv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class CRMBootstraper
    {
        public static void AddCRMManagement(IServiceCollection service, string connectionstring, DbProvider dbProvider)
        {
            //service.AddTransient<IProductApplication, ProductApplication>();
            service.AddScoped<IPersonApp, PersonApp>();
            service.AddScoped<IPersonRep, PersonRep>();
            service.AddScoped<IProductApp, ProductApp>();
            service.AddScoped<IProductRep, ProductRep>();
            service.AddScoped<ICountTypeApp, CountTypeApp>();
            service.AddScoped<ICountTypeRep, CountTypeRep>();
            service.AddScoped<IInvApp, InvApp>();
            service.AddScoped<IInvRep, InvRep>();

            service.AddScoped<IAuthApp, AuthApp>();
            service.AddScoped<ITokenApp, TokenApp>();
            service.AddScoped<IUserRepository, UserRep>();

            service.AddScoped<IPermissionRep, PermissionRepository>();
            service.AddScoped<IPermissionService, PermissionService>();


            service.AddScoped<IMyUserRepository, MyuserRepo>();
            service.AddScoped<IUsersApp, UsersApp>();
            service.AddScoped<IRoleRep , RoleRepo>();

            // Register AutoMapper  
            service.AddAutoMapper(typeof(ClassMapping));
            service.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());




            // DbContext با انتخاب نوع دیتابیس
            service.AddDbContext<MyContext>(c =>
            {
                if (dbProvider == DbProvider.SqlServer)
                {
                    c.UseSqlServer(connectionstring, b => b.MigrationsAssembly("Infrastructure"))
                     .EnableSensitiveDataLogging()
                     .LogTo(Console.WriteLine);
                }
                else if (dbProvider == DbProvider.Sqlite)
                {
                    c.UseSqlite(connectionstring, b => b.MigrationsAssembly("Infrastructure"))
                     .EnableSensitiveDataLogging()
                     .LogTo(Console.WriteLine);
                }
            }, ServiceLifetime.Scoped);

        }
    }
}

namespace Infrastructure
{
    public enum DbProvider
    {
        SqlServer,
        Sqlite
    }
}


