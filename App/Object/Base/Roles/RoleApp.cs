using App.Contracts.Object.Base.Roles;
using App.Object.Base.auth;
using AutoMapper;
using Domain.Objects.Base;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Object.Base.Roles
{
    // App\Object\Base\Roles\RoleApp.cs
    public class RoleApp : CrudService<Role, RoleView, RoleCreate, RoleUpdate, int>, IRoleApp
    {
        private readonly IRoleRep _roleRep;
        private readonly IPermissionRep _permissionRep; // دسترسی به جدول مجوزها
        private readonly IMapper _mapper;

        public RoleApp(IRoleRep roleRep, IMapper mapper, IPermissionRep permissionRep)
            : base(roleRep, mapper)
        {
            _roleRep = roleRep;
            _mapper = mapper;
            _permissionRep = permissionRep;
        }

        // دریافت تمام مجوزها برای استفاده در فرم
        public async Task<List<PermissionView>> GetAllPermissionsAsync()
        {
            var permissions = new List<PermissionView>();
            return _mapper.Map<List<PermissionView>>(permissions);
        }

        // override CreateAsync برای ذخیره RolePermissions
        public override async Task<StatusResult> CreateAsync(RoleCreate dto)
        {
            // اعتبارسنجی اولیه توسط ModelValidator در CrudService انجام می‌شود
            var role = _mapper.Map<Role>(dto);
            role.RolePermissions = dto.PermissionIds.Select(pid => new RolePermission
            {
                PermissionId = pid,
                RoleId = role.Id // بعد از ذخیره role مقدار Id مشخص می‌شود
            }).ToList();

            await _roleRep.CreateAsync(role);
            await _roleRep.SaveChangesAsync();
            return ResultFactory.Status(ResultStatusEnum.Success, "نقش با موفقیت ایجاد شد.");
        }

        // override UpdateAsync برای بروزرسانی RolePermissions
        public override async Task<StatusResult> UpdateAsync(RoleUpdate dto)
        {
            var role = await _roleRep.GetAsync(dto.Id);
            if (role == null)
                return ResultFactory.Status(ResultStatusEnum.NotFound, "نقش یافت نشد.");

            _mapper.Map(dto, role); 

            var existingPermissions = role.RolePermissions.ToList();
            foreach (var rp in existingPermissions)
                
            role.RolePermissions = dto.PermissionIds.Select(pid => new RolePermission
            {
                PermissionId = pid,
                RoleId = role.Id
            }).ToList();

            await _roleRep.UpdateAsync(role);
            await _roleRep.SaveChangesAsync();
            return ResultFactory.Status(ResultStatusEnum.Success, "نقش با موفقیت بروزرسانی شد.");
        }

       
        public override async Task<SingleDataResult<RoleUpdate>> GetByIdAsync(int id)
        {
            var role = await _roleRep.GetAsync(id);
            if (role == null)
                return ResultFactory.Single<RoleUpdate>(ResultStatusEnum.NotFound, null, "نقش یافت نشد.");

            var roleUpdate = _mapper.Map<RoleUpdate>(role);
            roleUpdate.PermissionIds = role.RolePermissions?.Select(rp => rp.PermissionId).ToList();
            return ResultFactory.Single(ResultStatusEnum.Success, roleUpdate);
        }

        
        public override async Task<ListDataResult<RoleView>> GetAllAsync(Pagination pagination)
        {
            var roles = await _roleRep.GetAsync(pagination);
            var roleViews = _mapper.Map<List<RoleView>>(roles);
            // پر کردن لیست مجوزها برای هر نقش (بهتر است با Include در ریپازیتوری انجام شود)
            foreach (var roleView in roleViews)
            {
                var role = roles.First(r => r.Id == roleView.Id);
                roleView.Permissions = _mapper.Map<List<PermissionView>>(role.RolePermissions?.Select(rp => rp.Permission));
            }
            var total = await _roleRep.CountAsync();
            return ResultFactory.List(ResultStatusEnum.Success, roleViews, total, pagination);
        }
    }



    public interface IRoleRep : IBaseRep<Role, int>
    {
    }

    
}
