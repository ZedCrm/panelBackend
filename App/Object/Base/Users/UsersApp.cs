// App/Object/Base/Users/UsersApp.cs
using App.Contracts.Object.Base.Users;
using App.utility;
using AutoMapper;
using Domain.Objects.Base;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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
        public Task<ListDataResult<UsersView>> GetAll(Pagination pagination) => base.GetAllAsync(pagination);
        public Task<SingleDataResult<UsersUpdate>> GetById(int id) => base.GetByIdAsync(id);
        public Task<StatusResult> DeleteBy(List<int> ids) => base.DeleteAsync(ids);

        /*=== متدهای اختصاصی ===*/
        public async Task<SingleDataResult<UserCreateFormData>> CreateForm()
        {
            var roles = await _roleRep.GetAsync();
            return ResultFactory.Single<UserCreateFormData>( ResultStatusEnum.Success,
                new UserCreateFormData
                {
                    Roles = roles.Select(r => new RoleView { Id = r.Id, Name = r.Name }).ToList()
                });
        }

        public async Task<StatusResult> KeepAlive(int userId)
        {
            _statusService.UpdateStatus(userId, UserStatus.Online, DateTime.Now);
            return ResultFactory.Status(ResultStatusEnum.Accepted ,"وضعیت آنلاین شد.");
        }

        /*=== اوررایدهای ضروری (Business + فایل + وضعیت) ===*/
        public override async Task<StatusResult> CreateAsync(UsersCreat dto)
        {
            if (await _userRepository.ExistAsync(u => u.Email == dto.Email))
                return ResultFactory.Status(ResultStatusEnum.Conflict ,"ایمیل قبلاً استفاده شده است.");
            if (await _userRepository.ExistAsync(u => u.Username == dto.Username))
                return ResultFactory.Status(ResultStatusEnum.Conflict ,"نام کاربری قبلاً استفاده شده است.");

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
            return ResultFactory.Status(ResultStatusEnum.Accepted ,$"کاربر {dto.FullName} با موفقیت ایجاد شد.");
        }

        public override async Task<StatusResult> UpdateAsync(UsersUpdate dto)
        {
            if (await _userRepository.ExistAsync(u => u.Email == dto.Email && u.Id != dto.Id))
                return ResultFactory.Status(ResultStatusEnum.Conflict ,"ایمیل قبلاً استفاده شده است.");
            if (await _userRepository.ExistAsync(u => u.Username == dto.Username && u.Id != dto.Id))
                return ResultFactory.Status(ResultStatusEnum.Conflict ,"نام کاربری قبلاً استفاده شده است.");

            var user = await _userRepository.GetAsync(dto.Id);
            if (user == null) return ResultFactory.Status(ResultStatusEnum.NotFound,MessageApp.NotFound);


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
            return ResultFactory.Status(ResultStatusEnum.Success ,"کاربر با موفقیت به‌روزرسانی شد.");
        }

        public override async Task<ListDataResult<UsersView>> GetAllAsync(Pagination pagination)
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
            return ResultFactory.List<UsersView>(ResultStatusEnum.Accepted,vms, total,
                                                           pagination);
        }

        public override async Task<StatusResult> DeleteAsync(List<int> ids)
        {
            var res = await base.DeleteAsync(ids);
            if (res.IsSuccess) ids.ForEach(id => _statusService.UpdateStatus(id, UserStatus.Offline));
            return res;
        }

        public async Task<ListDataResult<UserList>> GetList()
        {
            var users = await _repo.GetAsync();
          var res =  _mapper.Map<List<UserList>>(users);
          return  ResultFactory.List(ResultStatusEnum.Accepted ,res);

        }
    }

    public interface IMyUserRepository : IBaseRep<User, int> { }
}