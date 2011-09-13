using System;
using System.Data.Entity;
using AI_.Security.DAL;

namespace AI_.Studmix.WebApplication.DAL
{
    public class DataContext : SecurityDbContext
    {
        public DataContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DataContext()
            : base("ApplicationServices")
        {
        }
    }
}