using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AI_.Security.Models;

namespace AI_.Security.DAL
{
    public abstract class SecurityDbContext : DbContext
    {
        public SecurityDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public SecurityDbContext()
            : base("DataContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();

            modelBuilder.Entity<User>();
            modelBuilder.Entity<Role>();

            base.OnModelCreating(modelBuilder);
        }
    }
}