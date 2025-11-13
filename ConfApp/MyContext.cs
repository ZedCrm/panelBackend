using ConfApp.Mapping;
using Domain.Objects.Base;
using Domain.Objects.Chat;
using Domain.Objects.Shop;
using Microsoft.EntityFrameworkCore;


namespace ConfApp
{
    public class MyContext : DbContext
    {
        #region DbSet 
        public DbSet<Person> Persons { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CountType> CountTypes { get; set; }
        public DbSet<Inv> Inventories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatGroupMember> ChatGroupMembers { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }

        #endregion
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            var assembly = typeof(PersonMapping).Assembly;
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
