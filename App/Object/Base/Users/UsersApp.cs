// App/Object/Base/Users/UsersApp.cs
using App.Contracts.Object.Base.Users;
using App.Object.Base;
using App.utility;
using AutoMapper;
using Domain.Objects.Base;
using MyFrameWork.AppTool;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Object.Base.Users
{
    public class UsersApp : CrudService<User, UsersView, UsersCreat, UsersUpdate, int>,
                            IUsersApp
    {
        private readonly IMyUserRepository _userRepository;
        private readonly IFileService _fileService;
        private readonly UserStatusService _statusService;
        private readonly IRoleRep _roleRep;

        public UsersApp(
            IMyUserRepository userRepository,
            IMapper mapper,
            IFileService fileService,
            UserStatusService statusService,
            IRoleRep roleRep)
            : base(userRepository, mapper)
        {
            _userRepository = userRepository;
            _fileService = fileService;
            _statusService = statusService;
            _roleRep = roleRep;
        }

        /*=== اینترفیس IUsersApp ===*/
        public Task<ApiResult<List<UsersView>>> GetAll(Pagination pagination) => base.GetAllAsync(pagination);
        public Task<ApiResult<UsersUpdate>> GetById(int id) => base.GetByIdAsync(id);
        public Task<ApiResult> DeleteBy(List<int> ids) => base.DeleteAsync(ids);

        /*=== متدهای اختصاصی ===*/
        public async Task<ApiResult<UserCreateFormData>> CreateForm()
        {
            var roles = await _roleRep.GetAsync();
            return ApiResult<UserCreateFormData>.Success(
                new UserCreateFormData
                {
                    Roles = roles.Select(r => new RoleView { Id = r.Id, Name = r.Name }).ToList()
                });
        }

        public async Task<ApiResult> KeepAlive(int userId)
        {
            _statusService.UpdateStatus(userId, UserStatus.Online, DateTime.Now);
            return ApiResult.Success("وضعیت آنلاین شد.");
        }

        /*=== اوررایدهای ضروری (Business + فایل + وضعیت) ===*/
        public override async Task<ApiResult> CreateAsync(UsersCreat dto)
        {
            if (await _userRepository.ExistAsync(u => u.Email == dto.Email))
                return ApiResult.Failed("ایمیل قبلاً استفاده شده است.", 400);
            if (await _userRepository.ExistAsync(u => u.Username == dto.Username))
                return ApiResult.Failed("نام کاربری قبلاً استفاده شده است.", 400);

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.UserRoles = dto.RoleIds.Select(rid => new UserRole { RoleId = rid }).ToList();

            if (dto.ProfilePicture != null)
            {
                var resizeOptions = new ResizeOptions
                {
                    Size = new Size(200, 200),
                    Mode = ResizeMode.Max
                };

                user.ProfilePictureUrl = await _fileService.UploadAsync(
                    file: dto.ProfilePicture,
                    folderPath: "uploads/profiles",
                    existingUrl: user.ProfilePictureUrl,
                    resizeOptions: resizeOptions
                );
            }








            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();
            _statusService.UpdateStatus(user.Id, UserStatus.Offline);
            return ApiResult.Success(message: $"کاربر {dto.FullName} با موفقیت ایجاد شد.");
        }

        public override async Task<ApiResult> UpdateAsync(UsersUpdate dto)
        {
            if (await _userRepository.ExistAsync(u => u.Email == dto.Email && u.Id != dto.Id))
                return ApiResult.Failed("ایمیل قبلاً استفاده شده است.", 400);
            if (await _userRepository.ExistAsync(u => u.Username == dto.Username && u.Id != dto.Id))
                return ApiResult.Failed("نام کاربری قبلاً استفاده شده است.", 400);

            var user = await _userRepository.GetAsync(dto.Id);
            if (user == null) return ApiResult.Failed(MessageApp.NotFound, 404);


            var oldPasswordHash = user.PasswordHash;

            _mapper.Map(dto, user);
            // اگر کاربر رمز جدید نفرستاده بود، پسورد قبلی را برگردان
            if (string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = oldPasswordHash;
            else
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            user.UserRoles = dto.RoleIds.Select(rid => new UserRole { RoleId = rid, UserId = dto.Id }).ToList();

            if (dto.ProfilePicture != null)
            {
                var resizeOptions = new ResizeOptions
                {
                    Size = new Size(200, 200),
                    Mode = ResizeMode.Max
                };

                user.ProfilePictureUrl = await _fileService.UploadAsync(
                    file: dto.ProfilePicture,
                    folderPath: "uploads/profiles",
                    existingUrl: user.ProfilePictureUrl,
                    resizeOptions: resizeOptions
                );
            }

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
            return ApiResult.Success(message: "کاربر با موفقیت به‌روزرسانی شد.");
        }

        public override async Task<ApiResult<List<UsersView>>> GetAllAsync(Pagination pagination)
        {
            var users = await _userRepository.GetAsync(pagination);
            var vms = _mapper.Map<List<UsersView>>(users);

            foreach (var vm in vms)
            {
                var (status, lastSeen) = _statusService.GetStatus(vm.Id);
                vm.Status = status;
                vm.LastSeen = lastSeen;
            }

            var total = await _userRepository.CountAsync();
            return ApiResult<List<UsersView>>.PagedSuccess(vms, total,
                                                           pagination.PageNumber,
                                                           pagination.PageSize);
        }

        public override async Task<ApiResult> DeleteAsync(List<int> ids)
        {
            var res = await base.DeleteAsync(ids);
            if (res.IsSucceeded) ids.ForEach(id => _statusService.UpdateStatus(id, UserStatus.Offline));
            return res;
        }
    }

    public interface IMyUserRepository : IBaseRep<User, int> { }
}