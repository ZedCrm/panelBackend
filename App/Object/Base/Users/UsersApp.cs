
using App.Contracts.Object.Base.auth;
using App.Contracts.Object.Base.Users;
using App.utility;
using AutoMapper;
using Domain.Objects.Base;
using MyFrameWork.AppTool;

namespace App.Object.Base.Users
{
    public class UsersApp : IUsersApp
    {
        #region Dependencies
        private readonly IMyUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPermissionService _permissionService;
        private readonly IFileService _fileService;
        private readonly UserStatusService _statusService;
        private readonly IRoleRep _roleRep;

        public UsersApp(
            IMyUserRepository userRepository,
            IMapper mapper,
            IPermissionService permissionService,
            IFileService fileService,
            UserStatusService statusService,
            IRoleRep roleRep)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _permissionService = permissionService;
            _fileService = fileService;
            _statusService = statusService;
            _roleRep = roleRep;
        }
        #endregion

        #region Create
        public async Task<ApiResult<object>> Create(UsersCreat objectCreate)
        {
            // اعتبارسنجی مدل
            var validationResult = ModelValidator.ValidateToOptResult<UsersCreat>(objectCreate);
            if (!validationResult.IsSucceeded)
                return ApiResult<object>.Failed(400, validationResult.Message!);

            // چک یکتا بودن ایمیل و نام کاربری
            var uniqueEmail = await ValidationUtility.ValidateUniqueAsync<User, int>(
                _userRepository,
                u => u.Email == objectCreate.Email,
                MessageApp.DuplicateField("ایمیل")
            );
            if (!uniqueEmail.IsSucceeded) return ApiResult<object>.Failed(400, uniqueEmail.Message!);

            var uniqueUsername = await ValidationUtility.ValidateUniqueAsync<User, int>(
                _userRepository,
                u => u.Username == objectCreate.Username,
                MessageApp.DuplicateField("نام کاربری")
            );
            if (!uniqueUsername.IsSucceeded) return ApiResult<object>.Failed(400, uniqueUsername.Message!);

            // مپینگ و ایجاد کاربر
            var user = _mapper.Map<User>(objectCreate);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(objectCreate.Password);
            user.UserRoles = objectCreate.RoleIds.Select(rid => new UserRole { RoleId = rid }).ToList();

            // آپلود عکس پروفایل (اگر وجود داشت)
            if (objectCreate.ProfilePicture != null)
            {
                try
                {
                    user.ProfilePictureUrl = await _fileService.UploadProfilePictureAsync(objectCreate.ProfilePicture);
                }
                catch (Exception ex)
                {
                    return ApiResult<object>.Failed(400, ex.Message);
                }
            }

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();

            // وضعیت اولیه: آفلاین
            _statusService.UpdateStatus(user.Id, UserStatus.Offline);

            return ApiResult<object>.Success(message: MessageApp.CustomAddsuccses(objectCreate.FullName));
        }
        #endregion

        #region CreateForm
        public async Task<ApiResult<UserCreateFormData>> CreateForm()
        {
            var roles = await _roleRep.GetAsync();
            var formData = new UserCreateFormData
            {
                Roles = roles.Select(r => new RoleView { Id = r.Id, Name = r.Name }).ToList()
            };

            return ApiResult<UserCreateFormData>.Success(formData, 200,MessageApp.AcceptOpt);
        }
        #endregion

        #region DeleteBy
        public async Task<ApiResult<object>> DeleteBy(List<int> objectIds)
        {
            if (objectIds == null || !objectIds.Any())
                return ApiResult<object>.Failed(400, MessageApp.NotFound);

            try
            {
                foreach (var id in objectIds)
                {
                    _userRepository.DeleteById(id);
                    _statusService.UpdateStatus(id, UserStatus.Offline); // آفلاین کردن
                }

                await _userRepository.SaveChangesAsync();
                return ApiResult<object>.Success(message: MessageApp.CustomSuccess("حذف"));
            }
            catch (Exception ex)
            {
                return ApiResult<object>.Failed(500, MessageApp.CustomDeleteFail(ex.Message));
            }
        }
        #endregion

        #region GetAll
        public async Task<ApiResult<UsersView>> GetAll(Pagination pagination)
        {
            var users = await _userRepository.GetAsync(pagination);
            var viewModels = _mapper.Map<List<UsersView>>(users);

            var totalRecords = await _userRepository.CountAsync();
            var totalPages = pagination.CalculateTotalPages(totalRecords);

            // اضافه کردن وضعیت آنلاین و آخرین بازدید
            foreach (var view in viewModels)
            {
                var (status, lastSeen) = _statusService.GetStatus(view.Id);
                view.Status = status;
                view.LastSeen = lastSeen;
            }

            return ApiResult<UsersView>.SuccessPaged(
                data: viewModels,
                totalRecords: totalRecords,
                pageNumber: pagination.PageNumber,
                pageSize: pagination.PageSize,
                message: MessageApp.AcceptOpt
            );
        }
        #endregion

        #region GetById
        public async Task<ApiResult<UsersUpdate>> GetById(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
                return ApiResult<UsersUpdate>.Failed(404, MessageApp.NotFound);

            var updateDto = _mapper.Map<UsersUpdate>(user);

            // اضافه کردن وضعیت فعلی
            var (status, lastSeen) = _statusService.GetStatus(id);
            // اگر DTO فیلد نداشته باشه، می‌تونی در ViewModel جدا نشون بدی

            return ApiResult<UsersUpdate>.Success(updateDto, 200,MessageApp.AcceptOpt);
        }
        #endregion

        #region Update
        public async Task<ApiResult<object>> Update(UsersUpdate objectView)
        {
            var validationResult = ModelValidator.ValidateToOptResult<UsersUpdate>(objectView);
            if (!validationResult.IsSucceeded)
                return ApiResult<object>.Failed(400, validationResult.Message!);

            // چک یکتا بودن ایمیل و نام کاربری (به جز خودش)
            var uniqueEmail = await ValidationUtility.ValidateUniqueAsync<User, int>(
                _userRepository,
                u => u.Email == objectView.Email && u.Id != objectView.Id,
                MessageApp.DuplicateField("ایمیل")
            );
            if (!uniqueEmail.IsSucceeded) return ApiResult<object>.Failed(400, uniqueEmail.Message!);

            var uniqueUsername = await ValidationUtility.ValidateUniqueAsync<User, int>(
                _userRepository,
                u => u.Username == objectView.Username && u.Id != objectView.Id,
                MessageApp.DuplicateField("نام کاربری")
            );
            if (!uniqueUsername.IsSucceeded) return ApiResult<object>.Failed(400, uniqueUsername.Message!);

            var user = await _userRepository.GetAsync(objectView.Id);
            if (user == null)
                return ApiResult<object>.Failed(404, MessageApp.NotFound);

            // بروزرسانی فیلدها
            user.Username = objectView.Username;
            user.Email = objectView.Email;
            user.FullName = objectView.FullName;

            if (!string.IsNullOrEmpty(objectView.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(objectView.Password);
            }

            user.UserRoles = objectView.RoleIds.Select(rid => new UserRole { RoleId = rid, UserId = objectView.Id }).ToList();

            // آپلود عکس جدید (اگر آپلود شده)
            if (objectView.ProfilePicture != null)
            {
                try
                {
                    user.ProfilePictureUrl = await _fileService.UploadProfilePictureAsync(
                        objectView.ProfilePicture,
                        user.ProfilePictureUrl // فایل قدیمی حذف میشه
                    );
                }
                catch (Exception ex)
                {
                    return ApiResult<object>.Failed(400, ex.Message);
                }
            }

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return ApiResult<object>.Success(message: MessageApp.AcceptOpt);
        }
        #endregion

        #region KeepAlive (برای بروزرسانی وضعیت آنلاین)
        public async Task<ApiResult<object>> KeepAlive(int userId)
        {
            _statusService.UpdateStatus(userId, UserStatus.Online, DateTime.Now);
            return ApiResult<object>.Success(message: "وضعیت بروز شد");
        }
        #endregion
    }

    // --- رابط‌ها ---
    public interface IMyUserRepository : IBaseRep<User, int> { }
}