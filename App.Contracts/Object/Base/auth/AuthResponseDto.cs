using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Objects.Base;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Contracts.Object.Base.auth
{
    public class AuthResponseDto
    {
        public int UserId { get; set; } = default! ;
        public string Token { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string FullName { get; set; } = default!;
        
    }

     public class LoginRequestDto
    {
        public string Username  { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

        public class RegisterRequestDto
    {
        public string FullName { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

      public interface IAuthApp
    {
         Task<StatusResult>  RegisterAsync(RegisterRequestDto dto);
        Task<SingleDataResult<AuthResponseDto>> LoginAsync(LoginRequestDto dto);
        
    }

     public interface ITokenApp
    {
        string GenerateToken(User user);
    }
}