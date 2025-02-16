using App;
using App.Contracts.Object.Base;
using App.Contracts.Object.Shop.ProductCon;
using App.Object.Base;
using App.Object.Shop.ProductApp;
using AutoMapper;
using ConfApp;
using ConfApp.Rep;
using ConfApp.Rep.Inv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class CRMBootstraper
    {
        public static void AddCRMManagement(IServiceCollection service, string connectionstring)
        {
            //service.AddTransient<IProductApplication, ProductApplication>();
            service.AddScoped<IPersonApp, PersonApp>();
            service.AddScoped<IPersonRep, PersonRep>();
            service.AddScoped<IProductApp, ProductApp>();
            service.AddScoped<IProductRep, ProductRep>();



            // Register AutoMapper  
            service.AddAutoMapper(typeof(ClassMapping));




            service.AddDbContext<MyContext>(c =>
            {
                c.UseSqlServer(connectionstring, b => b.MigrationsAssembly("Infrastructure")
                ).EnableSensitiveDataLogging().LogTo(Console.WriteLine);
            }, ServiceLifetime.Scoped);

        }
    }
}
