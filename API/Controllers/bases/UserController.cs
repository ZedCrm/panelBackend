using System.Runtime.Intrinsics.Arm;
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

        [HttpPost("/api/User/GetAll")]
        public async Task<ActionResult<ApiResult<UsersView>>> GetAll([FromBody] Pagination pagination)
        {
            var result = await _usersApp.GetAll(pagination);
            return Ok(result);
        }

        [HttpGet("/api/User/GetById")]
        public async Task<ActionResult<ApiResult<UsersUpdate>>> GetById([FromQuery] int id)
        {
            var result = await _usersApp.GetById(id);
            return Ok(result);
        }

        [HttpPost("/api/User/create")]
        public async Task<ActionResult<ApiResult<object>>> Create([FromForm] UsersCreat userCreate)  // FromForm برای فایل
        {
            var result = await _usersApp.Create(userCreate);
            return Ok(result);
        }

        [HttpPost("/api/User/update")]
        public async Task<ActionResult<ApiResult<object>>> Update([FromForm] UsersUpdate userUpdate)  // FromForm
        {
            var result = await _usersApp.Update(userUpdate);
            return Ok(result);
        }

        [HttpPost("/api/User/delete")]
        public async Task<ActionResult<ApiResult<object>>> Delete([FromBody] List<int> ids)
        {
            var result = await _usersApp.DeleteBy(ids);
            return Ok(result);
        }

        // جدید: برای keep alive
        [HttpPost("/api/User/keepalive")]
        public async Task<ActionResult<ApiResult<object>>> KeepAlive()
        {
            var userId = GetCurrentUserId();
            var result = await _usersApp.KeepAlive(userId);
            return Ok(result);
        }

        [HttpGet("/api/user/getcreateform")]
        public async Task<ActionResult<ApiResult<object>>> Getcreateform()
        {
            var result = await _usersApp.CreateForm();
            return Ok(result);
        }
    }
}