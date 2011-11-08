using System.Data.Entity;
using AI_.Security.DAL;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.DAL.Database
{
    public class DataContext : SecurityDbContext
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