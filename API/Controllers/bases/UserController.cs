// API\Controllers\bases\UserController.cs
using API.Attributes;
using App.Contracts.Object.Base.Users;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;

namespace API.Controllers.bases
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        private readonly IUsersApp _usersApp;

        public UserController(IUsersApp usersApp)
        {
            _usersApp = usersApp;
        }

        private List<string> GetModelStateErrors()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }

        [RequirePermission("User.View")]
        [HttpPost("/api/User/GetAll")]
        public async Task<ActionResult<ApiResult>> GetAll([FromBody] Pagination pagination)
        {
            var result = await _usersApp.GetAll(pagination);
            return StatusCode(result.StatusCode, result);
        }

        [RequirePermission("User.View")]
        [HttpGet("/api/User/GetById")]
        public async Task<ActionResult<ApiResult>> GetById([FromQuery] int id)
        {
            var result = await _usersApp.GetById(id);
           return StatusCode(result.StatusCode, result);
        }

        [RequirePermission("User.Create")]
        [HttpPost("/api/User/create")]
        public async Task<ActionResult<ApiResult>> Create([FromForm] UsersCreat userCreate)
        {
            

            var result = await _usersApp.Create(userCreate);
            return StatusCode(result.StatusCode, result);
        }

        [RequirePermission("User.Edit")]
        [HttpPost("/api/User/update")]
        public async Task<ActionResult<ApiResult>> Update([FromForm] UsersUpdate userUpdate)
        {
            

            var result = await _usersApp.Update(userUpdate);
            return StatusCode(result.StatusCode, result);
        }

        [RequirePermission("User.Delete")]
        [HttpPost("/api/User/delete")]
        public async Task<ActionResult<ApiResult>> Delete([FromBody] List<int> ids)
        {


            var result = await _usersApp.DeleteBy(ids);
            return StatusCode(result.StatusCode, result);
        }

        // KeepAlive: معمولاً نیازی به permission نیست (کاربر خودش رو آنلاین می‌کنه)
        [HttpPost("/api/User/keepalive")]
        public async Task<ActionResult<ApiResult>> KeepAlive()
        {
            var userId = GetCurrentUserId();
            if (userId <= 0)
                return Unauthorized(ApiResult.Failed("کاربر معتبر نیست.", 401));

            var result = await _usersApp.KeepAlive(userId);
            return StatusCode(result.StatusCode, result);
        }

        [RequirePermission("User.Create")]
        [HttpGet("/api/user/getcreateform")]
        public async Task<ActionResult<ApiResult>> GetCreateForm()
        {
            var result = await _usersApp.CreateForm();
            return StatusCode(result.StatusCode, result);
        }
    }
}