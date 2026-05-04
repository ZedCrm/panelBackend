// API\Controllers\bases\UserController.cs
using API.Attributes;
using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.Users;
using App.Object.Base.auth;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace API.Controllers.bases
{
    [ApiExplorerSettings(IgnoreApi = true)] 
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUsersApp _usersApp;
        private readonly IPermissionService _permissionService;
        public UserController(IUsersApp usersApp , IPermissionService permissionService) 
        { 
            _usersApp = usersApp ;
         _permissionService = permissionService;
         }

        /*======================================================*/
        /*                     متدهای CRUD                      */
        /*======================================================*/
        [RequirePermission("User.View")]
        [HttpPost("/api/User/GetAll")]
        public async Task<ActionResult<ListDataResult<UsersView>>> GetAll([FromBody] Pagination pagination)
            => await _usersApp.GetAll(pagination);

        [RequirePermission("User.View")]
        [HttpGet("/api/User/GetById")]
        public async Task<ActionResult<SingleDataResult<UsersUpdate>>> GetById([FromQuery] int id)
            => await _usersApp.GetById(id);

        [RequirePermission("User.Create")]
        [HttpPost("/api/User/create")]
        public async Task<ActionResult<StatusResult>> Create([FromForm] UsersCreat userCreate)
            => await _usersApp.CreateAsync(userCreate);

        [RequirePermission("User.Edit")]
        [HttpPost("/api/User/update")]
        public async Task<ActionResult<StatusResult>> Update([FromForm] UsersUpdate userUpdate)
            => await _usersApp.UpdateAsync(userUpdate);

        [RequirePermission("User.Delete")]
        [HttpPost("/api/User/delete")]
        public async Task<ActionResult<StatusResult>> Delete([FromBody] List<int> ids)
            => await _usersApp.DeleteBy(ids);

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