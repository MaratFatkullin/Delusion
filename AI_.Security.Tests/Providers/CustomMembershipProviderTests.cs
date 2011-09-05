using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;
using AI_.Security.Models;
using AI_.Security.Providers;
using AI_.Security.Tests.Mocks;
using AI_.Security.Tests.UtilityClasses;
using Xunit;

namespace AI_.Security.Tests.Providers
{
    public class CustomMembershipProviderTests
    {
        private readonly CustomMembershipProvider _provider;
        private readonly SecurityUnitOfWorkMock _unitOfWork;
        private MembershipCreateStatus _membershipCreateStatus;

        protected IList<User> UserStorage
        {
            get { return ((RepositoryMock<User>) _unitOfWork.UserRepository).Storage; }
        }

        public CustomMembershipProviderTests()
        {
            _unitOfWork = new SecurityUnitOfWorkMock();
            _provider = new CustomMembershipProvider(_unitOfWork);
        }

        [Fact]
        public void Fact1()
        {
            User user = GetUser();
            AddUser(user);

            UserStorage.ShouldContainExactlyOneItem(user);
        }

        [Fact]
        public void Fact2()
        {
            AddUser(GetUser(email: "a@c.d"));
            AddUser(GetUser(email: "a@c.d"));

            Assert.Equal(MembershipCreateStatus.DuplicateUserName, _membershipCreateStatus);
        }

        [Fact]
        public void Fact3()
        {
            AddUser(GetUser(username: "user1"));
            AddUser(GetUser(username: "user1"));

            Assert.Equal(MembershipCreateStatus.DuplicateEmail, _membershipCreateStatus);
        }

        [Fact]
        public void Fact4()
        {
            var user = GetUser();
            AddUser(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            _provider.ChangePasswordQuestionAndAnswer(user.UserName,
                                                      user.Password,
                                                      newPasswordQuestion,
                                                      newPasswordAnswer);

            Assert.Equal(UserStorage.SingleOrDefault().PasswordQuestion, newPasswordQuestion);
            Assert.Equal(UserStorage.SingleOrDefault().PasswordAnswer, newPasswordAnswer);
        }

        [Fact]
        public void pcFact1()
        {
            var user = GetUser();
            AddUser(user);
            var newPassword = "newPassword";
            _provider.ChangePassword(user.UserName, user.Password, newPassword);

            Assert.Equal(UserStorage.Single().Password, newPassword);
        }

        #region ResetPassword

        [Fact]
        public void rpFact1()
        {
            var user = GetUser();
            AddUser(user);
            Assert.Throws<NotSupportedException>(delegate
                                                 {
                                                     _provider.ResetPassword(user.UserName,
                                                                             user.PasswordAnswer);
                                                 });
        }

        [Fact]
        public void rpFact2()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Initialize("name", config);
            var user = GetUser();
            AddUser(user);
            Assert.Throws<ProviderException>(delegate
                                             {
                                                 _provider.ResetPassword(user.UserName,
                                                                         null);
                                             });
        }

        [Fact]
        public void rpFact3()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Initialize("name", config);
            var user = GetUser();
            Assert.Throws<MembershipPasswordException>(delegate
                                                       {
                                                           _provider.ResetPassword(user.UserName,
                                                                                   user.PasswordAnswer);
                                                       });
        }

        [Fact]
        public void rpFact4()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Initialize("name", config);
            var user = GetUser();
            AddUser(user);
            UserStorage.Single().IsLocked = true;
            Assert.Throws<MembershipPasswordException>(delegate
                                                       {
                                                           _provider.ResetPassword(user.UserName,
                                                                                   user.PasswordAnswer);
                                                       });
        }

        [Fact]
        public void rpFact5()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Initialize("name", config);
            var user = GetUser();
            AddUser(user);
            Assert.Throws<MembershipPasswordException>(delegate
                                                       {
                                                           _provider.ResetPassword(user.UserName,
                                                                                   user.PasswordAnswer);
                                                       });
        }



        #endregion

        #region Utility methods

        private User GetUser(string username = "username",
                             string password = "password",
                             string email = "email",
                             string passwordQuestion = "passwordQuestion",
                             string passwordAnswer = "passwordAnswer",
                             bool isApproved = true,
                             object providerUserKey = null)
        {
            var user = new User
                       {
                           UserName = username,
                           Password = password,
                           Email = email,
                           PasswordQuestion = passwordQuestion,
                           PasswordAnswer = passwordAnswer,
                           IsApproved = isApproved,
                           ProviderUserKey = providerUserKey
                       };

            return user;
        }

        private MembershipUser AddUser(User user)
        {
            return _provider.CreateUser(user.UserName,
                                        user.Password,
                                        user.Email,
                                        user.PasswordQuestion,
                                        user.PasswordAnswer,
                                        user.IsApproved,
                                        user.ProviderUserKey,
                                        out _membershipCreateStatus);
        }

        private static MembershipUser GetMembershipUser()
        {
            return new MembershipUser("providerName",
                                      "username",
                                      Guid.NewGuid(),
                                      "a@b.c",
                                      "passwordQuestion",
                                      "comment",
                                      true,
                                      false,
                                      DateTime.Now,
                                      DateTime.Now,
                                      DateTime.Now,
                                      DateTime.Now,
                                      DateTime.MinValue);
        }

        #endregion

        #region GetPassword

        [Fact]
        public void gpFact1()
        {
            var user = GetUser();
            AddUser(user);
            var password = _provider.GetPassword(user.UserName, user.PasswordAnswer);
            Assert.Equal(user.Password, password);
        }

        [Fact]
        public void gpFact2()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Initialize("name", config);

            var user = GetUser();
            AddUser(user);

            Assert.Throws<ProviderException>(delegate
                                             {
                                                 _provider.GetPassword(user.UserName,
                                                                       user.PasswordAnswer);
                                             });
        }

        [Fact]
        public void gpFact3()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");

            _provider.Initialize("name", config);
            var user = GetUser();
            AddUser(user);

            Assert.Throws<ProviderException>(delegate
                                             {
                                                 _provider.GetPassword(user.UserName,
                                                                       user.PasswordAnswer);
                                             });
        }


        [Fact]
        public void gpFact4()
        {
            Assert.Throws<MembershipPasswordException>(delegate
                                                       {
                                                           _provider.GetPassword("username",
                                                                                 "passwordanswer");
                                                       });
        }

        [Fact]
        public void gpFact5()
        {
            var user = GetUser();
            AddUser(user);

            UserStorage.Single().IsLocked = true;

            Assert.Throws<MembershipPasswordException>(delegate
                                                       {
                                                           _provider.GetPassword(user.UserName,
                                                                                 user.PasswordAnswer);
                                                       });
        }

        #endregion

        [Fact]
        public void updFact1()
        {
            var user = GetUser();
            AddUser(user);
            var membershipUser = GetMembershipUser();
            _provider.UpdateUser(membershipUser);
            var updatedUser = _provider.GetUser("username", false);
            Assert.Equal(membershipUser,updatedUser);
        }

        [Fact]
        public void unlockFact1()
        {
            var user = GetUser();
            AddUser(user);

            Assert.Throws<ProviderException>(delegate
                                             {
                                                 _provider.UnlockUser(user.UserName);
                                             });
        }

        [Fact]
        public void unlockFact2()
        {
            var user = GetUser();
            AddUser(user);
            UserStorage.Single().IsLocked = true;
                _provider.UnlockUser(user.UserName);
            Assert.False(UserStorage.Single().IsLocked);
        }

        [Fact]
        public void getuserFact1()
        {
            var user = GetUser();
            AddUser(GetUser());
            var membershipUser = _provider.GetUser(user.ProviderUserKey, false);
            Assert.Equal(user.UserName, membershipUser.UserName);
        }

        [Fact]
        //todo: via theory (userIsOnLine)
        public void getUserFact2()
        {
            var user = GetUser();
            var membershipUser = _provider.GetUser(user.UserName, false);
            Assert.Null(membershipUser);
        }

        [Fact]
        public void getUserFact3()
        {
            var user = GetUser();
            AddUser(user);
            var membershipUser = _provider.GetUser(user.UserName, false);
            Assert.Equal(user.UserName,membershipUser.UserName);
        }


        [Fact]
        public void getUsernameByEMFact1()
        {
            var user = GetUser();
            var username = _provider.GetUserNameByEmail(user.Email);
            Assert.Null(username);
        }

        [Fact]
        public void getUsernameByEMFact2()
        {
            var user = GetUser();
            AddUser(user);
            var username = _provider.GetUserNameByEmail(user.Email);
            Assert.Equal(user.UserName,username);
        }

        [Fact]
        //todo:theory
        public void deleteUserFact1()
        {
            var user = GetUser();
            var userDeleted = _provider.DeleteUser(user.UserName, false);
            Assert.False(userDeleted);
        }

        [Fact]
        public void deleteuserFact2()
        {
            var user = GetUser();
            AddUser(user);
            var userDeleted = _provider.DeleteUser(user.UserName,false);
            Assert.True(userDeleted);
        }

        [Fact]
        public void getAllUserFact1()
        {
            int totalRecords;
            var membershipUserCollection = _provider.GetAllUsers(0, 1, out totalRecords);
            Assert.Equal(0,membershipUserCollection.Count);
            Assert.Equal(0,totalRecords);
        }

        [Fact]
        //todo:theory
        public void getAllUserFact2()
        {
            int totalRecords;
            AddUser(GetUser());
            AddUser(GetUser());
            var membershipUserCollection = _provider.GetAllUsers(0, 1, out totalRecords);

            Assert.Equal(1,membershipUserCollection.Count);
            Assert.Equal(2,totalRecords);
        }

        [Fact]
        //todo:theory
        public void findUsersByNameFact1()
        {
            var user = GetUser();
            int totalRecords;
            var membershipUserCollection = _provider.FindUsersByName(user.UserName, 0, 1, out totalRecords);
            Assert.Equal(0,totalRecords);
            Assert.Equal(null,membershipUserCollection);
        }

        [Fact]
        //todo:theory
        public void findUsersByNameFact2()
        {
            var user = GetUser();
            int totalRecords;
            AddUser(user);
            var membershipUserCollection = _provider.FindUsersByName(user.UserName, 0, 1, out totalRecords);
            Assert.Equal(1, totalRecords);
            Assert.Equal(1, membershipUserCollection.Count);
        }

        
        [Fact]
        //todo:theory
        public void findUsersByEMailFact2()
        {
            var user = GetUser();
            int totalRecords;
            AddUser(user);
            var membershipUserCollection = _provider.FindUsersByEmail(user.Email, 0, 1, out totalRecords);
            Assert.Equal(1, totalRecords);
            Assert.Equal(1, membershipUserCollection.Count);
        }
    }
}