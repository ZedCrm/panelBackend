// App/Object/Base/Roles/RoleApp.cs
using App.Contracts.Object.Base.Roles;
using AutoMapper;
using Domain.Objects.Base;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using ConfApp;

namespace App.Object.Base.Roles
{
    public class RoleApp : CrudService<Role, RoleView, RoleCreate, RoleUpdate, int>, IRoleApp
    {
        private readonly RoleBusinessService _roleBusiness;

        public RoleApp(MyContext context, IMapper mapper, RoleBusinessService roleBusiness)
            : base(context, mapper)
        {
            _roleBusiness = roleBusiness;
        }

        public async Task<List<PermissionView>> GetAllPermissionsAsync()
            => await _roleBusiness.GetAllPermissionsAsync();

        public override async Task<StatusResult> CreateAsync(RoleCreate dto)
        {
            // validation با استفاده از BusinessService
            var uniqueCheck = await _roleBusiness.ValidateRoleUniqueAsync(dto.Rolename);
            if (!uniqueCheck.IsSuccess) return uniqueCheck;

            var role = _mapper.Map<Role>(dto);
            role.RolePermissions = dto.PermissionIds.Select(pid => new RolePermission { PermissionId = pid }).ToList();
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return ResultFactory.Status(ResultStatusEnum.Success, MessageApp.CreatedMsg("نقش"));
        }

        public override async Task<StatusResult> UpdateAsync(RoleUpdate dto)
        {
            var uniqueCheck = await _roleBusiness.ValidateRoleUniqueAsync(dto.Rolename, dto.Id);
            if (!uniqueCheck.IsSuccess) return uniqueCheck;

            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == dto.Id && !r.IsDeleted);

            if (role == null)
                return ResultFactory.Status(ResultStatusEnum.NotFound, MessageApp.NotFoundItem("نقش"));

            _mapper.Map(dto, role);
            _context.RolePermissions.RemoveRange(role.RolePermissions);
            role.RolePermissions = dto.PermissionIds.Select(pid => new RolePermission { PermissionId = pid }).ToList();
            await _context.SaveChangesAsync();

            return ResultFactory.Status(ResultStatusEnum.Success, MessageApp.UpdatedMsg("نقش"));
        }

        public override async Task<SingleDataResult<RoleUpdate>> GetByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (role == null)
                return ResultFactory.Single<RoleUpdate>(ResultStatusEnum.NotFound, null, MessageApp.NotFoundItem("نقش"));

            var dto = _mapper.Map<RoleUpdate>(role);
            dto.PermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();
            return ResultFactory.Single(ResultStatusEnum.Success, dto);
        }
    }
}