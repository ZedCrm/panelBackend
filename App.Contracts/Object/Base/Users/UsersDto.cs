using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Contracts.Object.Base.Users
{
    public class UsersView
    {
        public int Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;

    }
    public class UsersCreat
    {

        public string FullName { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        // شناسه‌ رول‌هایی که کاربر باید داشته باشه
        public List<int> RoleIds { get; set; } = new List<int>();

    }

    public class UsersUpdate
    {
        public int Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? Password { get; set; } = default!;
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