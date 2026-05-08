// App/Object/Base/Users/UserBusinessService.cs
using App.Contracts.Object.Base.Users;
using App.utility;
using AutoMapper;
using ConfApp;
using Domain.Objects.Base;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;

namespace App.Object.Base.Users
{
    public class UserBusinessService : BaseService<User>
    {
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly UserStatusService _statusService;

        public UserBusinessService(
            MyContext context,
            IMapper mapper,
            IFileService fileService,
            UserStatusService statusService) : base(context)
        {
            _mapper = mapper;
            _fileService = fileService;
            _statusService = statusService;
        }

        public async Task<StatusResult> ValidateUniqueUserAsync(string username, string email, int? excludeId = null)
        {
            var query = GetActiveQuery();

            if (excludeId.HasValue)
            {
                var usernameExists = await query.AnyAsync(u => u.Username == username && u.Id != excludeId.Value);
                if (usernameExists)
                    return ResultFactory.Status(ResultStatusEnum.Conflict, MessageApp.DuplicateField("نام کاربری"));

                var emailExists = await query.AnyAsync(u => u.Email == email && u.Id != excludeId.Value);
                if (emailExists)
                    return ResultFactory.Status(ResultStatusEnum.Conflict, MessageApp.DuplicateField("ایمیل"));
            }
            else
            {
                var usernameExists = await query.AnyAsync(u => u.Username == username);
                if (usernameExists)
                    return ResultFactory.Status(ResultStatusEnum.Conflict, MessageApp.DuplicateField("نام کاربری"));

                var emailExists = await query.AnyAsync(u => u.Email == email);
                if (emailExists)
                    return ResultFactory.Status(ResultStatusEnum.Conflict, MessageApp.DuplicateField("ایمیل"));
            }

            return ResultFactory.Status(ResultStatusEnum.Success);
        }

        public async Task<SingleDataResult<UserCreateFormData>> GetCreateFormAsync()
        {
            var roles = await _context.Roles
                .Where(r => !r.IsDeleted)
                .Select(r => new RoleViewForUser { Id = r.Id, Name = r.Name })
                .ToListAsync();

            var formData = new UserCreateFormData { Roles = roles };
            return ResultFactory.Single(ResultStatusEnum.Success, formData);
        }

        public async Task<StatusResult> KeepAliveAsync(int userId)
        {
            _statusService.UpdateStatus(userId, UserStatus.Online, DateTime.Now);
            return ResultFactory.Status(ResultStatusEnum.Accepted, "وضعیت آنلاین شد.");
        }

        public async Task<SingleDataResult<UserWithRolesDto>> GetUserWithRolesAsync(int userId)
        {
            var user = await GetActiveQuery()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return ResultFactory.Single<UserWithRolesDto>(ResultStatusEnum.NotFound, null, MessageApp.NotFoundItem("کاربر"));

            var dto = new UserWithRolesDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };

            return ResultFactory.Single(ResultStatusEnum.Success, dto);
        }
    }

    public class UserWithRolesDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public List<string> Roles { get; set; } = new();
    }
}