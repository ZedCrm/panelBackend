
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
                .Where(ur => ur.UserId == userId && !ur.IsDeleted) 
        .Include(ur => ur.Role) // لود Role
        .ThenInclude(r => r.RolePermissions) // لود RolePermissions
        .ThenInclude(rp => rp.Permission) // لود Permission
        .AnyAsync(ur => ur.Role.RolePermissions.Any(rp => rp.Permission.Name == permission));
        }
    }
}