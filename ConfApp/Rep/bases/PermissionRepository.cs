using App.Object.Base.auth;
using Microsoft.EntityFrameworkCore;

namespace ConfApp.Rep.bases
{
    public class PermissionRepository : IPermissionRep
    {
        private readonly MyContext _context;

        public PermissionRepository(MyContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(int userId, string permission)
        {
            // چک اگر کاربر Admin باشه، همه دسترسی‌ها رو بده
            var isAdmin = await _context.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Include(ur => ur.Role)
                .AnyAsync(ur => ur.Role.Name == "Admin");

            if (isAdmin) return true;  // Admin همه پرمیشن‌ها رو داره

            // چک پرمیشن خاص
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .AnyAsync(ur => ur.Role.RolePermissions.Any(rp => rp.Permission.Name == permission));
        }
    }
}