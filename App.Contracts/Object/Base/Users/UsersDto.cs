// App.Contracts/Object/Base/Users/UsersDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Domain.Objects.Base;
using App.Contracts.Attributes; // برای MinCount

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
        [Required(ErrorMessage = "نام کامل الزامی است.")]
        [MaxLength(100, ErrorMessage = "نام کامل نمی‌تواند بیش از 100 کاراکتر باشد.")]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "نام کاربری الزامی است.")]
        [MaxLength(50, ErrorMessage = "نام کاربری نمی‌تواند بیش از 50 کاراکتر باشد.")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "ایمیل الزامی است.")]
        [EmailAddress(ErrorMessage = "ایمیل معتبر نیست.")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "رمز عبور الزامی است.")]
        [MinLength(6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد.")]
        public string Password { get; set; } = default!;

        public IFormFile? ProfilePicture { get; set; }

        [Required(ErrorMessage = "حداقل یک نقش انتخاب کنید.")]
        [MinCount(1, ErrorMessage = "حداقل یک نقش باید انتخاب شود.")]
        public List<int> RoleIds { get; set; } = new();
    }

    public class UsersUpdate
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "نام کامل الزامی است.")]
        [MaxLength(100)]
        public string FullName { get; set; } = default!;

        [Required(ErrorMessage = "نام کاربری الزامی است.")]
        [MaxLength(50)]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "ایمیل الزامی است.")]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [MinLength(6, ErrorMessage = "رمز عبور باید حداقل 6 کاراکتر باشد.")]
        public string? Password { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        [MinCount(1, ErrorMessage = "حداقل یک نقش باید انتخاب شود.")]
        public List<int> RoleIds { get; set; } = new();
    }

    public class UserCreateFormData
    {
        public List<RoleView> Roles { get; set; } = new();
    }

    public class RoleView
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
    }
}