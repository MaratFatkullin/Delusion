using System.Data.Entity;
using AI_.Security.DAL;
using AI_.Studmix.WebApplication.Models;

namespace AI_.Studmix.WebApplication.DAL.Database
{
    public class DataContext : SecurityDbContext
    {
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyState> PropertyStates { get; set; }
        public DbSet<ContentFile> ContentFiles { get; set; }
        public DbSet<ContentPackage> ContentPackages { get; set; }

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