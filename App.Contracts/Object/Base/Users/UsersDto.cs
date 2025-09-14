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
     
        public string Username { get; set; }  = default!;
        public string Email { get; set; } = default!;
        
    }
        public class UsersCreat
    {
         public int Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Username { get; set; }  = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}