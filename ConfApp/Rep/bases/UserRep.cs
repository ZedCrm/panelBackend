using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Object.Base.Auth;
using Domain.Objects.Base;
using Microsoft.EntityFrameworkCore;

namespace ConfApp.Rep.bases
{
    public class UserRep : IUserRepository
    {

        private readonly MyContext _context;

        public UserRep(MyContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByUsername(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}