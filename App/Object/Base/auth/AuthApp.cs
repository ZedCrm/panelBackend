using App.Contracts.Object.Base.auth;
using Domain.Objects.Base;
using MyFrameWork.AppTool;

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

      public async Task<OPTResult<AuthResponseDto>> LoginAsync(LoginRequestDto request)
{
    var user = await _userRepository.GetByEmailAsync(request.Email);
    if (user == null)
        return OPTResult<AuthResponseDto>.Failed("کاربری با این ایمیل یافت نشد.");

    var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
    if (!isPasswordCorrect)
        return OPTResult<AuthResponseDto>.Failed("رمز عبور اشتباه است.");

    var token = _tokenService.GenerateToken(user);

    return OPTResult<AuthResponseDto>.Success(new AuthResponseDto
    {
        Email = user.Email,
        FullName = user.FullName,
        Token = token
    });
}

      public async Task<OPTResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
{
    if (await _userRepository.ExistsByEmailAsync(request.Email))
        return OPTResult<AuthResponseDto>.Failed("ایمیل وارد شده قبلاً ثبت شده است.");

    var user = new User
    {
        FullName = request.FullName,
        Email = request.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
    };

    await _userRepository.AddAsync(user);
    var token = _tokenService.GenerateToken(user);

    return OPTResult<AuthResponseDto>.Success(new AuthResponseDto
    {
        Email = user.Email,
        FullName = user.FullName,
        Token = token
    });
}
    }
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<bool> ExistsByEmailAsync(string email);
    }

}