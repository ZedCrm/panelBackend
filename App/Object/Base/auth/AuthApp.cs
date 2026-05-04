using App.Contracts.Object.Base.auth;
using Domain.Objects.Base;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Object.Base.Auth
{

    public class AuthApp : IAuthApp
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenApp _tokenService;

        public AuthApp(IUserRepository userRepository, ITokenApp tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

 public async Task<SingleDataResult<AuthResponseDto>> LoginAsync(LoginRequestDto request)
{
    var user = await _userRepository.GetByUsernameAsync(request.Username);
    if (user == null)
        return ResultFactory.Single<AuthResponseDto>(ResultStatusEnum.NotFound ,null ,"کاربر یافت نشد");

    var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
    if (!isPasswordCorrect)
        return ResultFactory.Single<AuthResponseDto>(ResultStatusEnum.NotFound,null,"رمز عبور اشتباه است.");

    var token = _tokenService.GenerateToken(user);

    return ResultFactory.Single<AuthResponseDto>(ResultStatusEnum.Accepted,new AuthResponseDto
    {
        UserId = user.Id,
        Username = user.Username,
        FullName = user.FullName,
        Token = token
    });
}

      public async Task<StatusResult> RegisterAsync(RegisterRequestDto request)
{
    if (await _userRepository.ExistsByUsername(request.Username))
        return ResultFactory.Status(ResultStatusEnum.Conflict,null,"ایمیل وارد شده قبلاً ثبت شده است.");

    var user = new User
    {
        FullName = request.FullName,
        Username = request.Username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
    };

    await _userRepository.AddAsync(user);
    var token = _tokenService.GenerateToken(user);

    return ResultFactory.Status(ResultStatusEnum.Accepted);
}
    }
    public interface IUserRepository 
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsername(string username);
    }

}