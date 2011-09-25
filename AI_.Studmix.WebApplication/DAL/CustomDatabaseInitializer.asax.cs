using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using AI_.Security.Models;

namespace AI_.Studmix.WebApplication.DAL
{
    internal class CustomDatabaseInitializer : DropCreateDatabaseAlways<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            base.Seed(context);

            var role = new Role()
                           {
                               RoleName = "admin",
                               CreateDate = DateTime.Now
                           };
            context.Roles.Add(role);

            var user = new User()
                           {
                               UserName = "marat",
                               Password = "123",
                               IsApproved = true,
                               CreateDate = DateTime.Now,
                               Roles = new Collection<Role> {role}
                           };

            context.Users.Add(user);

            context.SaveChanges();
        }
    }
}