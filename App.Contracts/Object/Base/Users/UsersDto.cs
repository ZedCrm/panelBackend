using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Contracts.Object.Base.Users
{
    public class UsersView
    {

        public string FullName { get; set; } = default!;
     
        public string Username { get; set; }  = default!;
        public string Email { get; set; } = default!;
        
    }
        public class UsersCreat
    {
        public string FullName { get; set; } = default!;
        public string Username { get; set; }  = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
    }
}