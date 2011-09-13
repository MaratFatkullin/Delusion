using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AI_.Security.Models;


namespace AI_.Security.DAL
{
    public class SecurityDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
        }

        public SecurityDbContext(string nameOrConnectionString) 
            : base(nameOrConnectionString)
        {
        }

        public SecurityDbContext()
            : base("ApplicationServices")
        {
        }
    }
}