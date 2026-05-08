// API/Controllers/bases/RoleController.cs
using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.Roles;
using App.Object.Base.Roles;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace API.Controllers.bases
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : GenericController<RoleView, RoleCreate, RoleUpdate, int>
    {
        private readonly RoleBusinessService _roleBusiness;

        public RoleController(IRoleApp roleApp, RoleBusinessService roleBusiness) : base(roleApp)
        {
            _roleBusiness = roleBusiness;
        }

        [HttpGet("permissions")]
        public async Task<ActionResult<ListDataResult<PermissionView>>> GetAllPermissions()
        {
            var permissions = await _roleBusiness.GetAllPermissionsAsync();
            return ResultFactory.List(ResultStatusEnum.Success, permissions);
        }
    }
}