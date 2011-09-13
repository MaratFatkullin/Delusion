using System;
using System.Data.Entity;
using AI_.Security.Models;

namespace AI_.Studmix.WebApplication.DAL
{
    class CustomDatabaseInitializer : DropCreateDatabaseAlways<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            base.Seed(context);

            //context.Users.Add(new User() {UserName = "marat", Password = "123"});
            context.Roles.Add(new Role() {
                RoleName = "Admin123",
                CreateDate = DateTime.Now
            });

            context.SaveChanges();
        }
    }
}