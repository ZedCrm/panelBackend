// API\Controllers\bases\UserController.cs
using API.Attributes;
using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.Users;
using App.Contracts.Object.Shop.InvCon;
using App.Object.Base.auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace API.Controllers.bases
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : GenericController<UsersView, UsersCreat, UsersUpdate, int>
    {
        private readonly IUsersApp _usersApp;
        private readonly IPermissionService _permissionService;
        public UserController(IUsersApp usersApp , IPermissionService permissionService) : base(usersApp) 
        { 
            _usersApp = usersApp ;
         _permissionService = permissionService;
         }

     
        /*======================================================*/
        /*                  متدهای اختصاصی                      */
        /*======================================================*/
        [HttpPost("/api/User/keepalive")]
        public async Task<ActionResult<StatusResult>> KeepAlive()
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
                return Unauthorized(ResultFactory.Status(ResultStatusEnum.Unauthorized,"کاربر معتبر نیست."));

            return await _usersApp.KeepAlive(userId);
        }

        [RequirePermission("User.Create")]
        [HttpGet("/api/user/getcreateform")]
        public async Task<ActionResult<SingleDataResult<UserCreateFormData>>> GetCreateForm()
            => await _usersApp.CreateForm();




        [HttpGet("/api/user/permissions")]
        public async Task<ActionResult<ListDataResult<string>>> GetUserPermissions()
        {
            var userId = GetCurrentUserId();  // از BaseController می‌گیری
            if (userId <= 0) return Unauthorized(ResultFactory.Status(ResultStatusEnum.Unauthorized,"کاربر معتبر نیست."));

            var permissions = await _permissionService.GetUserPermissionsAsync(userId);  
            return ResultFactory.List<string>(ResultStatusEnum.Success,permissions);
        }

        [HttpGet("/api/user/getlist")]

        public async Task<ListDataResult<UserList>> GetList(){

            return await _usersApp.GetList();

        }
    }
}