using App.Contracts.Object.Base.auth;
using Microsoft.EntityFrameworkCore;
using ConfApp;

namespace App.Object.Base.auth
{
    public class PermissionService : IPermissionService
    {
        private readonly MyContext _context;

        public PermissionService(MyContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(int userId, string permission)
        {
            // ابتدا بررسی کنیم کاربر ادمین است؟ (Admin نقش ویژه دارد)
            var isAdmin = await _context.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Include(ur => ur.Role)
                .AnyAsync(ur => ur.Role.Name == "Admin");

            if (isAdmin) return true;

            // بررسی دسترسی معمولی
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .AnyAsync(ur => ur.Role.RolePermissions.Any(rp => rp.Permission.Name == permission));
        }

        public async Task<List<string>> GetUserPermissionsAsync(int userId)
        {
            // بررسی ادمین
            var isAdmin = await _context.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Include(ur => ur.Role)
                .AnyAsync(ur => ur.Role.Name == "Admin");

            if (isAdmin)
            {
                // ادمین به همه دسترسی‌ها دارد
                return await _context.Permissions
                    .Where(p => !p.IsDeleted)
                    .Select(p => p.Name)
                    .ToListAsync();
            }

            // برگرداندن دسترسی‌های معمولی
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Name))
                .Distinct()
                .ToListAsync();
        }
    }
}