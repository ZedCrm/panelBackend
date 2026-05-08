using App.Contracts.Object.Base.auth;
using Domain.Objects.Base;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using ConfApp;

namespace App.Object.Base.Auth
{
    public class AuthApp : IAuthApp
    {
        private readonly MyContext _context;
        private readonly ITokenApp _tokenService;

        public AuthApp(MyContext context, ITokenApp tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<SingleDataResult<AuthResponseDto>> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && !u.IsDeleted);
            if (user == null)
                return ResultFactory.Single<AuthResponseDto>(ResultStatusEnum.NotFound, null, "کاربر یافت نشد");
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return ResultFactory.Single<AuthResponseDto>(ResultStatusEnum.NotFound, null, "رمز عبور اشتباه است.");
            var token = _tokenService.GenerateToken(user);
            return ResultFactory.Single(ResultStatusEnum.Accepted, new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Token = token
            });
        }

        public async Task<StatusResult> RegisterAsync(RegisterRequestDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username && !u.IsDeleted))
                return ResultFactory.Status(ResultStatusEnum.Conflict, "نام کاربری تکراری است.");
            var user = new User
            {
                FullName = request.FullName,
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Email = request.Username // یا از فیلد مجزا
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return ResultFactory.Status(ResultStatusEnum.Accepted);
        }
    }
}