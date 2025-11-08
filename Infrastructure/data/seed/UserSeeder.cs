using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfApp;
using Domain.Objects.Base;

namespace Infrastructure.data.seed
{
    // UserSeeder.cs
public class UserSeeder
{
    private readonly MyContext _context;

    public UserSeeder(MyContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (!_context.Users.Any(u => u.Email == "test@example.com"))
        {
            var user = new User
            {
                FullName = "Test User",
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123"),
                Status = UserStatus.Offline,
                Username = "test"
            };
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}

// RoleSeeder.cs (مثال دیگه)
public class RoleSeeder
{
    private readonly MyContext _context;

    public RoleSeeder(MyContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (!_context.Roles.Any())
        {
            _context.Roles.AddRange(new List<Role>
            {
                new Role { Name = "Admin" },
                new Role { Name = "User" }
            });
            _context.SaveChanges();
        }
    }
 }

// DatabaseSeeder.cs
public class DatabaseSeeder
{
    private readonly MyContext _context;

    public DatabaseSeeder(MyContext context)
    {
        _context = context;
    }

    public void SeedAll()
    {
        new UserSeeder(_context).Seed();
         new RoleSeeder(_context).Seed();
        // اینجا میتونی Seeder های دیگه هم اضافه کنی
    }
}

}