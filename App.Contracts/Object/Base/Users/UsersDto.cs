using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Domain.Objects.Base;

namespace App.Contracts.Object.Base.Users
{
    public class UsersView
    {
        public int Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? ProfilePictureUrl { get; set; }  // جدید
        public UserStatus Status { get; set; }  // جدید
        public DateTime? LastSeen { get; set; }  // جدید
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
        public List<int> RoleIds { get; set; } = new List<int>();

    }

    public class UsersUpdate
    {
        public int Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Password { get; set; } = default!;
        public IFormFile? ProfilePicture { get; set; }
        public List<int> RoleIds { get; set; } = new List<int>();
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