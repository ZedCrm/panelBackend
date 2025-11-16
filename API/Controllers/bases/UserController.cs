// API\Controllers\bases\UserController.cs
using API.Attributes;
using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.Users;
using App.Object.Base.auth;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;

namespace API.Controllers.bases
{
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
        public async Task<ActionResult<ApiResult<List<UsersView>>>> GetAll([FromBody] Pagination pagination)
            => await _usersApp.GetAll(pagination);

        [RequirePermission("User.View")]
        [HttpGet("/api/User/GetById")]
        public async Task<ActionResult<ApiResult<UsersUpdate>>> GetById([FromQuery] int id)
            => await _usersApp.GetById(id);

        [RequirePermission("User.Create")]
        [HttpPost("/api/User/create")]
        public async Task<ActionResult<ApiResult>> Create([FromForm] UsersCreat userCreate)
            => await _usersApp.CreateAsync(userCreate);

        [RequirePermission("User.Edit")]
        [HttpPost("/api/User/update")]
        public async Task<ActionResult<ApiResult>> Update([FromForm] UsersUpdate userUpdate)
            => await _usersApp.UpdateAsync(userUpdate);

        [RequirePermission("User.Delete")]
        [HttpPost("/api/User/delete")]
        public async Task<ActionResult<ApiResult>> Delete([FromBody] List<int> ids)
            => await _usersApp.DeleteBy(ids);

        /*======================================================*/
        /*                  متدهای اختصاصی                      */
        /*======================================================*/
        [HttpPost("/api/User/keepalive")]
        public async Task<ActionResult<ApiResult>> KeepAlive()
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
                return Unauthorized(ApiResult.Failed("کاربر معتبر نیست.", 401));

            return await _usersApp.KeepAlive(userId);
        }

        [RequirePermission("User.Create")]
        [HttpGet("/api/user/getcreateform")]
        public async Task<ActionResult<ApiResult<UserCreateFormData>>> GetCreateForm()
            => await _usersApp.CreateForm();




        [HttpGet("/api/user/permissions")]
        public async Task<ActionResult<ApiResult<List<string>>>> GetUserPermissions()
        {
            var userId = GetCurrentUserId();  // از BaseController می‌گیری
            if (userId <= 0) return Unauthorized(ApiResult.Failed("کاربر معتبر نیست.", 401));

            var permissions = await _permissionService.GetUserPermissionsAsync(userId);  
            return ApiResult<List<string>>.Success(permissions);
        }

        [HttpGet("/api/user/getlist")]

        public async Task<ApiResult<List<UserList>>> GetList(){

            return await _usersApp.GetList();

        }
    }
}