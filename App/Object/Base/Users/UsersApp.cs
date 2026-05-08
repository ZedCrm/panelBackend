using App.Contracts.Object.Base.Users;
using App.Object.Base.Roles;
using App.utility;
using AutoMapper;
using ConfApp;  // برای MyContext
using Domain.Objects.Base;
using Microsoft.EntityFrameworkCore;
using MyFrameWork.AppTool;
using MyFrameWork.AppTool.ResultType;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Linq.Expressions;

namespace App.Object.Base.Users
{
    public class UsersApp : CrudService<User, UsersView, UsersCreat, UsersUpdate, int>,
                            IUsersApp
    {
        private readonly MyContext _context;
        private readonly IFileService _fileService;
        private readonly UserStatusService _statusService;
        private readonly IMapper _mapper;

        public UsersApp(
            MyContext context,
            IMapper mapper,
            IFileService fileService,
            UserStatusService statusService)
            : base(context, mapper)   // ← پایه CrudService از context استفاده می‌کند
        {
            _context = context;
            _fileService = fileService;
            _statusService = statusService;
            _mapper = mapper;
        }

        // متدهای قبلی با استفاده از DbSet بازنویسی می‌شوند
        public async Task<SingleDataResult<UserCreateFormData>> CreateForm()
        {
            var roles = await _context.Roles
                .Where(r => !r.IsDeleted)
                .Select(r => new RoleViewForUser { Id = r.Id, Name = r.Name })
                .ToListAsync();
            return ResultFactory.Single<UserCreateFormData>(
                ResultStatusEnum.Success,
                new UserCreateFormData { Roles = roles });
        }

        public async Task<StatusResult> KeepAlive(int userId)
        {
            _statusService.UpdateStatus(userId, UserStatus.Online, DateTime.Now);
            return ResultFactory.Status(ResultStatusEnum.Accepted, "وضعیت آنلاین شد.");
        }

        public override async Task<StatusResult> CreateAsync(UsersCreat dto)
        {
            // بررسی یکتایی با DbSet
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted))
                return ResultFactory.Status(ResultStatusEnum.Conflict, "ایمیل قبلاً استفاده شده است.");
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username && !u.IsDeleted))
                return ResultFactory.Status(ResultStatusEnum.Conflict, "نام کاربری قبلاً استفاده شده است.");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            // نقش‌ها
            user.UserRoles = dto.RoleIds.Select(rid => new UserRole { RoleId = rid }).ToList();

            if (dto.ProfilePicture != null)
            {
                var resizeOptions = new ResizeOptions { Size = new Size(200, 200), Mode = ResizeMode.Max };
                user.ProfilePictureUrl = await _fileService.UploadAsync(
                    dto.ProfilePicture, "uploads/profiles", user.ProfilePictureUrl, resizeOptions);
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            _statusService.UpdateStatus(user.Id, UserStatus.Offline);
            return ResultFactory.Status(ResultStatusEnum.Accepted, $"کاربر {dto.FullName} ایجاد شد.");
        }

        public override async Task<StatusResult> UpdateAsync(UsersUpdate dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != dto.Id && !u.IsDeleted))
                return ResultFactory.Status(ResultStatusEnum.Conflict, "ایمیل قبلاً استفاده شده است.");
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username && u.Id != dto.Id && !u.IsDeleted))
                return ResultFactory.Status(ResultStatusEnum.Conflict, "نام کاربری قبلاً استفاده شده است.");

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == dto.Id && !u.IsDeleted);
            if (user == null)
                return ResultFactory.Status(ResultStatusEnum.NotFound, MessageApp.NotFound);

            var oldPasswordHash = user.PasswordHash;
            _mapper.Map(dto, user);
            if (string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = oldPasswordHash;
            else
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // حذف نقش‌های قدیمی و اضافه کردن جدید
            _context.UserRoles.RemoveRange(user.UserRoles);
            user.UserRoles = dto.RoleIds.Select(rid => new UserRole { RoleId = rid, UserId = dto.Id }).ToList();

            if (dto.ProfilePicture != null)
            {
                var resizeOptions = new ResizeOptions { Size = new Size(200, 200), Mode = ResizeMode.Max };
                user.ProfilePictureUrl = await _fileService.UploadAsync(
                    dto.ProfilePicture, "uploads/profiles", user.ProfilePictureUrl, resizeOptions);
            }

            await _context.SaveChangesAsync();
            return ResultFactory.Status(ResultStatusEnum.Success, "کاربر بروزرسانی شد.");
        }

        public override async Task<ListDataResult<UsersView>> GetAllAsync(Pagination pagination)
        {
            var query = _context.Users.Where(u => !u.IsDeleted).AsNoTracking();
            // مرتب‌سازی داینامیک مانند CrudService
            if (!string.IsNullOrEmpty(pagination.SortBy))
                query = ApplySorting(query, pagination.SortBy, pagination.SortDirection);
            var total = await query.CountAsync();
            var users = await query.Skip(pagination.CalculateSkip()).Take(pagination.PageSize).ToListAsync();
            var vms = _mapper.Map<List<UsersView>>(users);
            foreach (var vm in vms)
            {
                var (status, lastSeen) = _statusService.GetStatus(vm.Id);
                vm.Status = status;
                vm.LastSeen = lastSeen;
            }
            return ResultFactory.List(ResultStatusEnum.Accepted, vms, total, pagination);
        }

        public async Task<ListDataResult<UserList>> GetList()
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserList { Id = u.Id, FullName = u.FullName })
                .ToListAsync();
            return ResultFactory.List(ResultStatusEnum.Accepted, users);
        }

        // متد کمکی برای مرتب‌سازی (مشابه CrudService)
        private static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string sortBy, bool ascending)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, sortBy);
            var lambda = Expression.Lambda(property, parameter);
            string methodName = ascending ? "OrderBy" : "OrderByDescending";
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type);
            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
        }
    }
}