using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.Roles;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace API.Controllers.bases
{
    public class RoleController : GenericController<RoleView ,RoleCreate, RoleUpdate, int>
    {
        private readonly IRoleApp roleApp;
        private readonly IPermissionService permissionService;

        public RoleController(IRoleApp roleApp, IPermissionService permissionService) : base(roleApp)
        {
            this.roleApp = roleApp;
            this.permissionService = permissionService;
        }




       
    }
}
