using AI_.Security.DAL;

namespace AI_.Studmix.WebApplication.DAL.Database
{
    public class DataContext : SecurityDbContext
    {
        public DataContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DataContext()
            : base("DataContext")
        {
        }
    }
}