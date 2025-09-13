
using App.Contracts.Object.Base.Users;
using App.Object.Base.Users;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;

namespace API.Controllers.bases
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseController
    {
        
        private readonly IUsersApp _usersApp;

          public UserController (IUsersApp usersApp){
                this._usersApp = usersApp;
          }
        [HttpPost]
        [Route("/api/Users/GetAll")]
        public async Task<ActionResult<OPTResult<UsersView>>> Index([FromBody] Pagination pagination)
        {

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("کاربر شناسایی نشد.");
            }


            return await _usersApp.GetAll(pagination , userId);




        }
    }
}