
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
            return await _context.UserRoles
                .Where(ur => ur.Id == userId)
                .Join(_context.RolePermissions,
                    ur => ur.Id,
                    rp => rp.Id,
                    (ur, rp) => rp.Permission.Name)
                .AnyAsync(p => p == permission);
        }
    }
}