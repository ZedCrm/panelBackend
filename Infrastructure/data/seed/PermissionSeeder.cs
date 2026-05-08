using System.Reflection;
using App.Contracts.Object.Base.auth;
using ConfApp;
using Domain.Objects.Base;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.data.seed
{
    public class PermissionSeeder
    {
        private readonly MyContext _context;
        private readonly IPermissionDiscoveryService _permissionDiscovery;

        public PermissionSeeder(MyContext context, IPermissionDiscoveryService permissionDiscovery)
        {
            _context = context;
            _permissionDiscovery = permissionDiscovery;
        }

        public async Task SeedAsync()
        {
            var permissionNames = _permissionDiscovery.GetAllPermissionNames(); // شامل مجوزهای ضمنی هم هست

            foreach (var name in permissionNames)
            {
                if (!await _context.Permissions.AnyAsync(p => p.Name == name && !p.IsDeleted))
                {
                    string controllerName = name.Contains('.') ? name.Split('.')[0] : "General";
                    _context.Permissions.Add(new Permission
                    {
                        Name = name,
                        Category = controllerName,
                        CreateDate = DateTime.Now
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

    }
}