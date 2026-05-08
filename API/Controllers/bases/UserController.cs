// API/Controllers/bases/UserController.cs - نسخه بهبود یافته
using API.Attributes;
using App.Contracts.Object.Base.Users;
using App.Object.Base.Users;
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
        private readonly UserBusinessService _userBusiness; // سرویس بیزینس جدید

        public UserController(IUsersApp usersApp, UserBusinessService userBusiness) : base(usersApp)
        {
            _usersApp = usersApp;
            _userBusiness = userBusiness;
        }

        [HttpGet("/api/user/getcreateform")]
        public async Task<ActionResult<SingleDataResult<UserCreateFormData>>> GetCreateForm()
            => await _userBusiness.GetCreateFormAsync();  // خیلی تمیز!

        [HttpPost("/api/User/keepalive")]
        public async Task<ActionResult<StatusResult>> KeepAlive()
        {
            var userId = GetCurrentUserId();
            return await _userBusiness.KeepAliveAsync(userId);
        }

        [HttpGet("/api/user/{id}/with-roles")]
        public async Task<ActionResult<SingleDataResult<UserWithRolesDto>>> GetUserWithRoles(int id)
            => await _userBusiness.GetUserWithRolesAsync(id);
    }
}