using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Objects.Base;
using MyFrameWork.AppTool;

namespace App.Contracts.Object.Base.auth
{
    public class AuthResponseDto
    {
        public int UserId { get; set; } = default! ;
        public string Token { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
        
    }

     public class LoginRequestDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

        public class RegisterRequestDto
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

      public interface IAuthApp
    {
         Task<OPTResult<AuthResponseDto>>  RegisterAsync(RegisterRequestDto dto);
        Task<OPTResult<AuthResponseDto>> LoginAsync(LoginRequestDto dto);
        
    }

     public interface ITokenApp
    {
        string GenerateToken(User user);
    }
}