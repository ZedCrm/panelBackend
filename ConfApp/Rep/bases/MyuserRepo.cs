using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Object.Base.Users;
using Domain.Objects.Base;
using Microsoft.EntityFrameworkCore;

namespace ConfApp.Rep.bases
{
    public class MyuserRepo :  BaseRep<User , int> , IMyUserRepository 
    {
        
        private readonly MyContext _context;

        public MyuserRepo(MyContext context) : base(context) 
        {
            _context = context;
        }



       public async Task<User> GetAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }
    }
}