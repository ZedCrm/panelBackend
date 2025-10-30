using App.Contracts.Object.Base.Users;
using App.Object.Base.Users;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;
using System.Security.Claims;

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

        [HttpPost]
        [Route("/api/User/GetAll")]
        public async Task<ActionResult<OPTResult<UsersView>>> GetAll([FromBody] Pagination pagination)
        {

            
            
            var result = await _usersApp.GetAll(pagination);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/User/GetById")]
        public async Task<ActionResult<OPTResult<UsersUpdate>>> GetById([FromQuery] int id)
        {
            var result = await _usersApp.GetById(id);
            return Ok(result);
        }

        [HttpPost]
        [Route("/api/User/create")]
        public async Task<ActionResult<OPT>> Create([FromBody] UsersCreat userCreate)
        {
            var result = await _usersApp.Create(userCreate);
            return Ok(result);
        }

        [HttpPost]
        [Route("/api/User/update")]
        public async Task<ActionResult<OPT>> Update([FromBody] UsersUpdate userUpdate)
        {
            var result = await _usersApp.Update(userUpdate);
            return Ok(result);
        }

        [HttpPost]
        [Route("/api/User/delete")]
        public async Task<ActionResult<OPT>> Delete([FromBody] List<int> ids)
        {
            var result = await _usersApp.DeleteBy(ids);
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/User/getcreateform")]
        public async Task<ActionResult<OPTResult<UserCreateFormData>>> GetCreateForm()
        {
            var result = await _usersApp.CreateForm();
            return Ok(result);
        }
    }
}