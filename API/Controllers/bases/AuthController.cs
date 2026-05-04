using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using App.Contracts.Object.Base.auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace API.Controllers.bases
{

   // [ApiExplorerSettings(IgnoreApi = true)] 
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthApp _authApp;

        public AuthController(IAuthApp authApp)
        {
            _authApp = authApp;
        }

        [HttpPost("login")]
        public async Task<ActionResult<SingleDataResult<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authApp.LoginAsync(request);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<StatusResult>> Register(RegisterRequestDto dto)
        {
            var result = await _authApp.RegisterAsync(dto);
            if (!result.IsSuccess)
                return BadRequest(result.Messages);
            return Ok(result);
        }
    }
}