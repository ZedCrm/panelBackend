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
      
        public string FullName { get; set; } = default!;


        public string Username { get; set; } = default!;


        public string Email { get; set; } = default!;

        public string Password { get; set; } = default!;

        public IFormFile? ProfilePicture { get; set; }


        public List<int> RoleIds { get; set; } = new();
    }

    public class UsersUpdate
    {
        [Required]
        public int Id { get; set; }


        public string FullName { get; set; } = default!;


        public string Username { get; set; } = default!;


        public string Email { get; set; } = default!;


        public string? Password { get; set; }

        public IFormFile? ProfilePicture { get; set; }


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