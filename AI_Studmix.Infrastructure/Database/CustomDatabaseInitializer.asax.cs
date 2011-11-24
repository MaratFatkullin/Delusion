using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using AI_.Studmix.Domain.Models;

namespace AI_.Studmix.Domain.DAL.Database
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

            context.Set<Role>().Add(adminRole);
            context.Set<Role>().Add(userRole);

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

            context.Set<User>().Add(admin);
            context.Set<User>().Add(user);

            var userProfile = new UserProfile {CreateDate = DateTime.Now, User = user, Balance = 50};
            var adminProfile = new UserProfile {CreateDate = DateTime.Now, User = admin, Balance = 100};

            context.Set<UserProfile>().Add(userProfile);
            context.Set<UserProfile>().Add(adminProfile);

            var countryProp = new Property
                              {
                                  Name = "Страна",
                                  Order = 1,
                                  CreateDate = DateTime.Now
                              };
            var cityProp = new Property
                           {
                               Name = "Город",
                               Order = 2,
                               CreateDate = DateTime.Now
                           };
            var instituteType = new Property
                                {
                                    Name = "Вид",
                                    Order = 3,
                                    CreateDate = DateTime.Now
                                };
            var instituteName = new Property
                                {
                                    Name = "Наименование учереждения",
                                    Order = 4,
                                    CreateDate = DateTime.Now
                                };
            var studingForm = new Property
                              {
                                  Name = "Форма обучения",
                                  Order = 5,
                                  CreateDate = DateTime.Now
                              };
            var faculty = new Property
                          {
                              Name = "Факультет",
                              Order = 6,
                              CreateDate = DateTime.Now
                          };
            var course = new Property
                         {
                             Name = "Курс",
                             Order = 7,
                             CreateDate = DateTime.Now
                         };
            var group = new Property
                        {
                            Name = "Группа",
                            Order = 8,
                            CreateDate = DateTime.Now
                        };
            var disciple = new Property
                           {
                               Name = "Дисциплина",
                               Order = 9,
                               CreateDate = DateTime.Now
                           };
            var book = new Property
                       {
                           Name = "Учебник\\год вып.",
                           Order = 10,
                           CreateDate = DateTime.Now
                       };
            var variant = new Property
                          {
                              Name = "Вариант",
                              Order = 11,
                              CreateDate = DateTime.Now
                          };
            var data = new Property
                       {
                           Name = "Данные\\кр.",
                           Order = 12,
                           CreateDate = DateTime.Now
                       };

            context.Set<Property>().Add(countryProp);
            context.Set<Property>().Add(cityProp);
            context.Set<Property>().Add(instituteType);
            context.Set<Property>().Add(instituteName);
            context.Set<Property>().Add(studingForm);
            context.Set<Property>().Add(faculty);
            context.Set<Property>().Add(course);
            context.Set<Property>().Add(group);
            context.Set<Property>().Add(disciple);
            context.Set<Property>().Add(book);
            context.Set<Property>().Add(variant);
            context.Set<Property>().Add(data);

            var russia = new PropertyState
                         {
                             Property = countryProp,
                             Value = "Россия",
                             CreateDate = DateTime.Now,
                             Index = 1
                         };
            var czech = new PropertyState
                        {
                            Property = countryProp,
                            Value = "Чешская республика",
                            CreateDate = DateTime.Now,
                            Index = 2
                        };
            var french = new PropertyState
                         {
                             Property = countryProp,
                             Value = "Франция",
                             CreateDate = DateTime.Now,
                             Index = 3
                         };

            context.Set<PropertyState>().Add(russia);
            context.Set<PropertyState>().Add(czech);
            context.Set<PropertyState>().Add(french);

            var moscow = new PropertyState
                         {
                             Property = cityProp,
                             Value = "Москва",
                             CreateDate = DateTime.Now,
                             Index = 1
                         };
            var kazan = new PropertyState
                        {
                            Property = cityProp,
                            Value = "Казань",
                            CreateDate = DateTime.Now,
                            Index = 2
                        };
            var prague = new PropertyState
                         {
                             Property = cityProp,
                             Value = "Прага",
                             CreateDate = DateTime.Now,
                             Index = 3
                         };
            var paris = new PropertyState
                        {
                            Property = cityProp,
                            Value = "Париж",
                            CreateDate = DateTime.Now,
                            Index = 4
                        };
            var marsel = new PropertyState
                         {
                             Property = cityProp,
                             Value = "Марсель",
                             CreateDate = DateTime.Now,
                             Index = 5
                         };

            context.Set<PropertyState>().Add(moscow);
            context.Set<PropertyState>().Add(kazan);
            context.Set<PropertyState>().Add(prague);
            context.Set<PropertyState>().Add(paris);
            context.Set<PropertyState>().Add(marsel);

            var contentPackage1 = new ContentPackage {CreateDate = DateTime.Now, Price = 70, Owner = user};
            contentPackage1.PropertyStates = new Collection<PropertyState> {russia, moscow};

            var contentPackage2 = new ContentPackage {CreateDate = DateTime.Now, Owner = user};
            contentPackage2.PropertyStates = new Collection<PropertyState> {russia, kazan};

            var contentPackage3 = new ContentPackage {CreateDate = DateTime.Now, Owner = admin};
            contentPackage3.PropertyStates = new Collection<PropertyState> {czech, prague};

            context.Set<ContentPackage>().Add(contentPackage1);
            context.Set<ContentPackage>().Add(contentPackage2);
            context.Set<ContentPackage>().Add(contentPackage3);

            context.SaveChanges();
        }
    }
}