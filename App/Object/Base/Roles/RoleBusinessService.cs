// App/Object/Base/Roles/RoleBusinessService.cs
using App.Contracts.Object.Base.Roles;
using App.utility;
using AutoMapper;
using ConfApp;
using Domain.Objects.Base;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Object.Base.Roles
{
    public class RoleBusinessService : BaseService<Role>
    {
        private readonly IMapper _mapper;

        public RoleBusinessService(MyContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<List<PermissionView>> GetAllPermissionsAsync()
        {
            return await _context.Permissions
                .Where(p => !p.IsDeleted)
                .Select(p => new PermissionView { Id = p.Id, PermissionName = p.Name, Category = p.Category })
                .ToListAsync();
        }

        public async Task<StatusResult> ValidateRoleUniqueAsync(string roleName, int? excludeId = null)
        {
            var query = GetActiveQuery();
            if (excludeId.HasValue)
                query = query.Where(r => r.Id != excludeId.Value);

            var exists = await query.AnyAsync(r => r.Name == roleName);
            if (exists)
                return ResultFactory.Status(ResultStatusEnum.Conflict, MessageApp.DuplicateField("نقش"));

            return ResultFactory.Status(ResultStatusEnum.Success);
        }
    }
}