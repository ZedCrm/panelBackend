using ConfApp.Mapping;
using Domain.Objects.Base;
using Domain.Objects.Shop;
using Microsoft.EntityFrameworkCore;


namespace ConfApp
{
    public class MyContext : DbContext
    {
        #region DbSet 
        public DbSet<Person> Persons { get; set; }
        public DbSet<Product> Products { get; set; }
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
