using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AI_.Studmix.Domain.Models;

namespace AI_.Studmix.Domain.DAL.Database
{
    public class DataContext : DbContext
    {
        public DataContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DataContext()
            : base("DataContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();

            modelBuilder.Entity<User>();
            modelBuilder.Entity<Role>();
            modelBuilder.Entity<UserProfile>();
            modelBuilder.Entity<ContentPackage>();
            modelBuilder.Entity<ContentFile>();
            modelBuilder.Entity<Order>();
            modelBuilder.Entity<Property>();
            modelBuilder.Entity<PropertyState>();

            modelBuilder.Entity<ContentPackage>()
                .HasMany<PropertyState>(package => package.PropertyStates)
                .WithMany(state => state.ContentPackages);
            //.Map(mapping => mapping.ToTable("ContentPackagePropertyStates"));

            modelBuilder.Entity<PropertyState>()
                .HasRequired<Property>(state => state.Property)
                .WithMany(property => property.States);
        }
    }
}