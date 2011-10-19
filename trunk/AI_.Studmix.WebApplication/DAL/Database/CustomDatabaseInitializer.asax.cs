using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using AI_.Security.Models;
using AI_.Studmix.WebApplication.Models;

namespace AI_.Studmix.WebApplication.DAL.Database
{
    public class CustomDatabaseInitializer : DropCreateDatabaseAlways<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            base.Seed(context);

            var adminRole = new Role
                            {
                                RoleName = "admin",
                                CreateDate = DateTime.Now
                            };
            var userRole = new Role
                           {
                               RoleName = "user",
                               CreateDate = DateTime.Now
                           };

            context.Roles.Add(adminRole);
            context.Roles.Add(userRole);

            var user = new User
                       {
                           UserName = "marat",
                           Password = "123",
                           IsApproved = true,
                           CreateDate = DateTime.Now,
                           Roles = new Collection<Role> {userRole}
                       };


            var admin = new User
                        {
                            UserName = "admin",
                            Password = "123",
                            IsApproved = true,
                            CreateDate = DateTime.Now,
                            Roles = new Collection<Role> {adminRole}
                        };

            context.Users.Add(admin);
            context.Users.Add(user);

            var countryProp = new Property
                              {
                                  Name = "������",
                                  Order = 1,
                                  CreateDate = DateTime.Now
                              };
            var cityProp = new Property
                           {
                               Name = "�����",
                               Order = 2,
                               CreateDate = DateTime.Now
                           };
            var instituteType = new Property
                                {
                                    Name = "���",
                                    Order = 3,
                                    CreateDate = DateTime.Now
                                };
            var instituteName = new Property
                                {
                                    Name = "������������ �����������",
                                    Order = 4,
                                    CreateDate = DateTime.Now
                                };
            var studingForm = new Property
                              {
                                  Name = "����� ��������",
                                  Order = 5,
                                  CreateDate = DateTime.Now
                              };
            var faculty = new Property
                          {
                              Name = "���������",
                              Order = 6,
                              CreateDate = DateTime.Now
                          };
            var course = new Property
                         {
                             Name = "����",
                             Order = 7,
                             CreateDate = DateTime.Now
                         };
            var group = new Property
                        {
                            Name = "������",
                            Order = 8,
                            CreateDate = DateTime.Now
                        };
            var disciple = new Property
                           {
                               Name = "����������",
                               Order = 9,
                               CreateDate = DateTime.Now
                           };
            var book = new Property
                       {
                           Name = "�������\\��� ���.",
                           Order = 10,
                           CreateDate = DateTime.Now
                       };
            var variant = new Property
                          {
                              Name = "�������",
                              Order = 11,
                              CreateDate = DateTime.Now
                          };
            var data = new Property
                       {
                           Name = "������\\��.",
                           Order = 12,
                           CreateDate = DateTime.Now
                       };

            context.Properties.Add(countryProp);
            context.Properties.Add(cityProp);
            context.Properties.Add(instituteType);
            context.Properties.Add(instituteName);
            context.Properties.Add(studingForm);
            context.Properties.Add(faculty);
            context.Properties.Add(course);
            context.Properties.Add(group);
            context.Properties.Add(disciple);
            context.Properties.Add(book);
            context.Properties.Add(variant);
            context.Properties.Add(data);

            var russia = new PropertyState
                         {
                             Property = countryProp,
                             Value = "������",
                             CreateDate = DateTime.Now
                         };
            var czech = new PropertyState
                        {
                            Property = countryProp,
                            Value = "������� ����������",
                            CreateDate = DateTime.Now
                        };
            var french = new PropertyState
                         {
                             Property = countryProp,
                             Value = "�������",
                             CreateDate = DateTime.Now
                         };

            context.PropertyStates.Add(russia);
            context.PropertyStates.Add(czech);
            context.PropertyStates.Add(french);

            var moscow = new PropertyState
                         {
                             Property = cityProp,
                             Value = "������",
                             CreateDate = DateTime.Now
                         };
            var kazan = new PropertyState
                        {
                            Property = cityProp,
                            Value = "������",
                            CreateDate = DateTime.Now
                        };
            var prague = new PropertyState
                         {
                             Property = cityProp,
                             Value = "�����",
                             CreateDate = DateTime.Now
                         };
            var paris = new PropertyState
                        {
                            Property = cityProp,
                            Value = "�����",
                            CreateDate = DateTime.Now
                        };
            var marsel = new PropertyState
                         {
                             Property = cityProp,
                             Value = "�������",
                             CreateDate = DateTime.Now
                         };

            context.PropertyStates.Add(moscow);
            context.PropertyStates.Add(kazan);
            context.PropertyStates.Add(prague);
            context.PropertyStates.Add(paris);
            context.PropertyStates.Add(marsel);

            var contentPackage1 = new ContentPackage {CreateDate = DateTime.Now};
            contentPackage1.PropertyStates = new Collection<PropertyState> {russia, moscow};

            var contentPackage2 = new ContentPackage {CreateDate = DateTime.Now};
            contentPackage2.PropertyStates = new Collection<PropertyState> {russia, kazan};

            var contentPackage3 = new ContentPackage {CreateDate = DateTime.Now};
            contentPackage3.PropertyStates = new Collection<PropertyState> {czech, prague};

            context.ContentPackages.Add(contentPackage1);
            context.ContentPackages.Add(contentPackage2);
            context.ContentPackages.Add(contentPackage3);

            context.SaveChanges();
        }
    }
}