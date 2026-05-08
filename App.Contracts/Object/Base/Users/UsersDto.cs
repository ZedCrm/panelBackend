// App.Contracts/Object/Base/Users/UsersDto.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Domain.Objects.Base;
using App.Contracts.Attributes;

namespace App.Contracts.Object.Base.Users
{
    public class UsersView
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
        public string? ProfilePictureUrl { get; set; }
        public UserStatus Status { get; set; }
        public DateTime? LastSeen { get; set; }
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
    }

    public class UsersCreat
    {
        [Required(ErrorMessage = "لطفاً نام کامل را وارد کنید.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "نام کامل باید بین 3 تا 100 کاراکتر باشد.")]
        [PersianText]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "لطفاً نام کاربری را وارد کنید.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "نام کاربری باید بین 3 تا 50 کاراکتر باشد.")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "لطفاً ایمیل را وارد کنید.")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست.")]
        [StringLength(100, ErrorMessage = "ایمیل نمی‌تواند بیشتر از 100 کاراکتر باشد.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "لطفاً رمز عبور را وارد کنید.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "رمز عبور باید بین 3 تا 100 کاراکتر باشد.")]
        public string Password { get; set; } = default!;

        public IFormFile? ProfilePicture { get; set; }
        public string? ProfilePictureUrl { get; set; }

        [MinCount(1, ErrorMessage = "حداقل یک نقش باید انتخاب شود.")]
        public List<int>? RoleIds { get; set; } = new();
    }

    public class UsersUpdate
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "لطفاً نام کامل را وارد کنید.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "نام کامل باید بین 3 تا 100 کاراکتر باشد.")]
        [PersianText]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "لطفاً نام کاربری را وارد کنید.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "نام کاربری باید بین 3 تا 50 کاراکتر باشد.")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "لطفاً ایمیل را وارد کنید.")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست.")]
        [StringLength(100, ErrorMessage = "ایمیل نمی‌تواند بیشتر از 100 کاراکتر باشد.")]
        public string Email { get; set; } = default!;

        [StringLength(100, MinimumLength = 3, ErrorMessage = "رمز عبور باید بین 3 تا 100 کاراکتر باشد.")]
        public string? Password { get; set; }

        public IFormFile? ProfilePicture { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public List<int>? RoleIds { get; set; } = new();
    }

    public class UserList
    {
        public int Id { get; set; }
        public string FullName { get; set; } = default!;
    }

    public class UserCreateFormData
    {
        public List<RoleViewForUser> Roles { get; set; } = new();
    }

    public class RoleViewForUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}