// App\Object\Base\Users\UsersApp.cs
using App.Contracts.Object.Base.Users;
using App.utility;
using AutoMapper;
using Domain.Objects.Base;
using MyFrameWork.AppTool;

namespace App.Object.Base.Users
{
    public class UsersApp : IUsersApp
    {
        private readonly IMyUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly UserStatusService _statusService;
        private readonly IRoleRep _roleRep;

        public UsersApp(
            IMyUserRepository userRepository,
            IMapper mapper,
            IFileService fileService,
            UserStatusService statusService,
            IRoleRep roleRep)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _fileService = fileService;
            _statusService = statusService;
            _roleRep = roleRep;
        }

        #region Create
        public async Task<ApiResult> Create(UsersCreat objectCreate)
        {
            // فقط Business Rules: یونیک بودن ایمیل و نام کاربری
            if (await _userRepository.ExistAsync(u => u.Email == objectCreate.Email))
                return ApiResult.Failed("ایمیل قبلاً استفاده شده است.", 400);

            if (await _userRepository.ExistAsync(u => u.Username == objectCreate.Username))
                return ApiResult.Failed("نام کاربری قبلاً استفاده شده است.", 400);

            var user = _mapper.Map<User>(objectCreate);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(objectCreate.Password);
            user.UserRoles = objectCreate.RoleIds.Select(rid => new UserRole { RoleId = rid }).ToList();

            if (objectCreate.ProfilePicture != null)
            {
                try
                {
                    user.ProfilePictureUrl = await _fileService.UploadProfilePictureAsync(objectCreate.ProfilePicture);
                }
                catch (Exception ex)
                {
                    return ApiResult.Failed($"خطا در آپلود تصویر: {ex.Message}", 400);
                }
            }

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();

            _statusService.UpdateStatus(user.Id, UserStatus.Offline);

            return ApiResult.Success(message: $"کاربر {objectCreate.FullName} با موفقیت ایجاد شد.");
        }
        #endregion

        #region Update
        public async Task<ApiResult> Update(UsersUpdate objectView)
        {
            // Business Rule: یونیک بودن (به جز خودش)
            if (await _userRepository.ExistAsync(u => u.Email == objectView.Email && u.Id != objectView.Id))
                return ApiResult.Failed("ایمیل قبلاً استفاده شده است.", 400);

            if (await _userRepository.ExistAsync(u => u.Username == objectView.Username && u.Id != objectView.Id))
                return ApiResult.Failed("نام کاربری قبلاً استفاده شده است.", 400);

            var user = await _userRepository.GetAsync(objectView.Id);
            if (user == null)
                return ApiResult.Failed(MessageApp.NotFound, 404);

            user.Username = objectView.Username;
            user.Email = objectView.Email;
            user.FullName = objectView.FullName;

            if (!string.IsNullOrEmpty(objectView.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(objectView.Password);

            user.UserRoles = objectView.RoleIds.Select(rid => new UserRole { RoleId = rid, UserId = objectView.Id }).ToList();

            if (objectView.ProfilePicture != null)
            {
                try
                {
                    user.ProfilePictureUrl = await _fileService.UploadProfilePictureAsync(
                        objectView.ProfilePicture,
                        user.ProfilePictureUrl
                    );
                }
                catch (Exception ex)
                {
                    return ApiResult.Failed($"خطا در آپلود تصویر: {ex.Message}", 400);
                }
            }

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return ApiResult.Success(message: "کاربر با موفقیت به‌روزرسانی شد.");
        }
        #endregion

        #region GetAll
        public async Task<ApiResult> GetAll(Pagination pagination)
        {
            var users = await _userRepository.GetAsync(pagination);
            var viewModels = _mapper.Map<List<UsersView>>(users);
            var totalRecords = await _userRepository.CountAsync();

            foreach (var view in viewModels)
            {
                var (status, lastSeen) = _statusService.GetStatus(view.Id);
                view.Status = status;
                view.LastSeen = lastSeen;
            }

            return ApiResult<List<UsersView>>.PagedSuccess(
                data: viewModels,
                totalRecords: totalRecords,
                pageNumber: pagination.PageNumber,
                pageSize: pagination.PageSize
            );
        }
        #endregion

        #region GetById
        public async Task<ApiResult> GetById(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
                return ApiResult.Failed(MessageApp.NotFound, 404);

            var updateDto = _mapper.Map<UsersUpdate>(user);
            return ApiResult<UsersUpdate>.Success(updateDto);
        }
        #endregion

        #region DeleteBy
        public async Task<ApiResult> DeleteBy(List<int> objectIds)
        {
            if (objectIds == null || !objectIds.Any())
                return ApiResult.Failed("هیچ کاربری انتخاب نشده است.", 400);

            try
            {
                foreach (var id in objectIds)
                {
                    _userRepository.DeleteById(id);
                    _statusService.UpdateStatus(id, UserStatus.Offline);
                }
                await _userRepository.SaveChangesAsync();
                return ApiResult.Success(message: "کاربران با موفقیت حذف شدند.");
            }
            catch (Exception ex)
            {
                return ApiResult.Failed($"خطا در حذف: {ex.Message}", 500);
            }
        }
        #endregion

        #region CreateForm
        public async Task<ApiResult> CreateForm()
        {
            var roles = await _roleRep.GetAsync();
            var formData = new UserCreateFormData
            {
                Roles = roles.Select(r => new RoleView { Id = r.Id, Name = r.Name }).ToList()
            };

            return ApiResult<UserCreateFormData>.Success( formData);
        }
        #endregion

        #region KeepAlive
        public async Task<ApiResult> KeepAlive(int userId)
        {
            _statusService.UpdateStatus(userId, UserStatus.Online, DateTime.Now);
            return ApiResult.Success(message: "وضعیت آنلاین شد.");
        }
        #endregion
    }



    public interface IMyUserRepository : IBaseRep<User, int> { }
}