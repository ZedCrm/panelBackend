using App.Object.Base.Roles;
using ConfApp;
using Domain.Objects.Base;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool; 

public class RoleRepo : BaseRep<Role, int>, IRoleRep
{
    private readonly MyContext ctx;

    public RoleRepo(MyContext ctx) : base(ctx)
    {
        this.ctx = ctx;
    }

    public override async Task<Role> GetAsync(int id)
    {
        return await ctx.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public override async Task<List<Role>> GetAsync(Pagination pagination)
    {
        var query = ctx.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => !r.IsDeleted);

        if (!string.IsNullOrEmpty(pagination.SortBy))
        {
          
        }

        return await query.Skip(pagination.CalculateSkip())
                          .Take(pagination.PageSize)
                          .ToListAsync();
    }

  
    public void DeleteRolePermission(RolePermission entity)
    {
        entity.IsDeleted = true;
        entity.UpdateDate = DateTime.Now;
        ctx.RolePermissions.Update(entity);
    }
}