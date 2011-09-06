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

        [Fact]
        public void CreateUser_DefaultConfiguration_UserCreated()
        {
            User user = GetUser();
            AddUser(user);

            UserStorage.ShouldContainExactlyOneItem(user);
        }

        [Fact]
        public void CreateUser_UserWithSameUserNameExists_DuplicateUserNameCreateStatusGot()
        {
            AddUser(GetUser());
            AddUser(GetUser());

            Assert.Equal(MembershipCreateStatus.DuplicateUserName, _membershipCreateStatus);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintEnabled_DuplicateEmailCreateStatusGot()
        {
            var config = new NameValueCollection();
            config.Add("requiresUniqueEmail", "true");
            _provider.Initialize("name", config);

            AddUser(GetUser(username: "user1"));
            AddUser(GetUser(username: "user2"));

            Assert.Equal(MembershipCreateStatus.DuplicateEmail, _membershipCreateStatus);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintDisabled_UserCreated()
        {
            var config = new NameValueCollection();
            config.Add("requiresUniqueEmail", "true");
            _provider.Initialize("name", config);

            AddUser(GetUser(username: "user1"));
            AddUser(GetUser(username: "user2"));

            Assert.Equal(MembershipCreateStatus.DuplicateEmail, _membershipCreateStatus);
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_ValidUserDataProvided_PasswordQuestionAndAnswerChanged()
        {
            var user = GetUser();
            AddUser(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            _provider.ChangePasswordQuestionAndAnswer(user.UserName,
                                                      user.Password,
                                                      newPasswordQuestion,
                                                      newPasswordAnswer);

            Assert.Equal(UserStorage.Single().PasswordQuestion, newPasswordQuestion);
            Assert.Equal(UserStorage.Single().PasswordAnswer, newPasswordAnswer);
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_InvalidUserDataProvided_PasswordQuestionAndAnswerNotChanged()
        {
            var user = GetUser();
            AddUser(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            _provider.ChangePasswordQuestionAndAnswer(user.UserName,
                                                      "invalidPassword",
                                                      newPasswordQuestion,
                                                      newPasswordAnswer);

            Assert.Equal(UserStorage.Single().PasswordQuestion, user.PasswordQuestion);
            Assert.Equal(UserStorage.Single().PasswordAnswer, user.PasswordAnswer);
        }


        [Fact]
        public void ChangePassword_ValidPasswordProvided_PasswordChanged()
        {
            var user = GetUser();
            AddUser(user);
            var newPassword = "newPassword";
            _provider.ChangePassword(user.UserName, user.Password, newPassword);

            Assert.Equal(UserStorage.Single().Password, newPassword);
        }

        [Fact]
        public void ChangePassword_InvalidOldPasswordProvided_PasswordNotChanged()
        {
            var user = GetUser();
            AddUser(user);
            _provider.ChangePassword(user.UserName, "invalidPassword", "newPassword");

            Assert.Equal(UserStorage.Single().Password, user.Password);
        }

        [Fact]
        public void ChangePassword_InvalidNewPasswordProvided_ExceptionThrown()
        {
            var user = GetUser();
            AddUser(user);
            var newPassword = "";

            Assert.Throws<MembershipPasswordException>(
                () => _provider.ChangePassword(user.UserName, user.Password, newPassword));
        }

        [Fact]
        public void ChangePassword_UserDoesNotExists_PasswordNotChanged()
        {
            var user = GetUser();
            AddUser(user);
            var newPassword = "newPassword";
            _provider.ChangePassword(user.UserName, user.Password, newPassword);

            Assert.Equal(UserStorage.Single().Password, user.Password);
        }

        [Fact]
        public void UpdateUser_UserExists_UserUpdated()
        {
            var user = GetUser();
            AddUser(user);
            var membershipUser = GetMembershipUser();
            _provider.UpdateUser(membershipUser);
            var updatedUser = _provider.GetUser(membershipUser.UserName, false);

            Assert.Equal(membershipUser, updatedUser);
        }

        [Fact]
        public void UpdateUser_UserDoesNotExists_ExceptionThrown()
        {
            var user = GetUser();
            AddUser(user);
            var membershipUser = GetMembershipUser();
            Assert.Throws<ProviderException>(
                () => _provider.UpdateUser(membershipUser));
        }

        [Fact]
        public void UnlockUser_UserDoesNotExists_ExceptionThrown()
        {
            var user = GetUser();

            Assert.Throws<ProviderException>(
                () => _provider.UnlockUser(user.UserName));
        }

        [Fact]
        public void UnlockUser_UserIsNotLocked_ExceptionThrown()
        {
            var user = GetUser();
            AddUser(user);

            Assert.Throws<ProviderException>(
                () => _provider.UnlockUser(user.UserName));
        }

        [Fact]
        public void UnlockUser_UserIsLocked_UserUnlocked()
        {
            var user = GetUser();
            AddUser(user);
            UserStorage.Single().IsLocked = true;
            _provider.UnlockUser(user.UserName);

            Assert.False(UserStorage.Single().IsLocked);
        }

        [Fact]
        public void GetUserByProviderUserKey_UserExists_UserGot()
        {
            var user = GetUser();
            AddUser(GetUser());
            var membershipUser = _provider.GetUser(user.ProviderUserKey, false);
            Assert.Equal(user.UserName, membershipUser.UserName);
        }

        [Fact]
        public void GetUserByProviderUserKey_UserDoesNotExists_NullGot()
        {
            var user = GetUser();
            var membershipUser = _provider.GetUser(user.ProviderUserKey, false);
            Assert.Equal(null, membershipUser);
        }

        [Fact]
        public void ResetPassword_PasswordResetOptionIsDisabled_ExceptionThrown()
        {
            var user = GetUser();
            AddUser(user);
            Assert.Throws<NotSupportedException>(
                () => _provider.ResetPassword(user.UserName, user.PasswordAnswer));
        }

        [Fact]
        public void ResetPassword_RequiredPasswordAnswerIsNull_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Initialize("name", config);
            var user = GetUser();
            AddUser(user);
            Assert.Throws<ProviderException>(
                () => _provider.ResetPassword(user.UserName, null));
        }

        [Fact]
        public void ResetPassword_UserDoesNotExists_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Initialize("name", config);
            var user = GetUser();
            Assert.Throws<MembershipPasswordException>(
                () => _provider.ResetPassword(user.UserName, user.PasswordAnswer));
        }

        [Fact]
        public void ResetPassword_UserIsLocked_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Initialize("name", config);
            var user = GetUser();
            AddUser(user);
            UserStorage.Single().IsLocked = true;
            Assert.Throws<MembershipPasswordException>(
                () => _provider.ResetPassword(user.UserName, user.PasswordAnswer));
        }

        [Fact]
        public void ResetPassword_ValidUserDataProvided_PasswordReseted()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Initialize("name", config);
            var user = GetUser();
            AddUser(user);
            _provider.ResetPassword(user.UserName, user.PasswordAnswer);
            Assert.NotEqual(user.Password, UserStorage.Single().Password);
        }

        [Fact]
        public void GetPassword_PasswordRetrievalOptionIsDisabled_ExceptionThrown()
        {
            var user = GetUser();
            AddUser(user);

            Assert.Throws<ProviderException>(
                () => _provider.GetPassword(user.UserName, user.PasswordAnswer));
        }

        [Fact]
        public void GetPassword_PasswordRetrievalOptionIsEnabled_PasswordGot()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Initialize("name", config);

            var user = GetUser();
            AddUser(user);
            var password = _provider.GetPassword(user.UserName, user.PasswordAnswer);
            Assert.Equal(user.Password, password);
        }


        [Fact]
        public void GetPassword_UserDoesNotExists_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Initialize("name", config);

            Assert.Throws<MembershipPasswordException>(
                () => _provider.GetPassword("username", "passwordanswer"));
        }

        [Fact]
        public void GetPassword_UserIsLocked_ExceptionThrown()
        {
            var user = GetUser();
            AddUser(user);

            UserStorage.Single().IsLocked = true;

            Assert.Throws<MembershipPasswordException>(
                () => _provider.GetPassword(user.UserName, user.PasswordAnswer));
        }

        [Fact]
        //todo: via theory (userIsOnLine) evrywhere
        public void GetUserByUsername_UserDoesNotExists_NullGot()
        {
            var user = GetUser();
            var membershipUser = _provider.GetUser(user.UserName, false);
            Assert.Null(membershipUser);
        }

        [Fact]
        public void GetUserByUsername_UserExists_UserGot()
        {
            var user = GetUser();
            AddUser(user);
            var membershipUser = _provider.GetUser(user.UserName, false);
            Assert.Equal(user.UserName, membershipUser.UserName);
        }

        [Fact]
        public void GetUserNameByEmail_UserDoesNotExists_NullGot()
        {
            var user = GetUser();
            var username = _provider.GetUserNameByEmail(user.Email);
            Assert.Null(username);
        }

        [Fact]
        public void GetUserNameByEmail_UserExists_UserNameGot()
        {
            var user = GetUser();
            AddUser(user);
            var username = _provider.GetUserNameByEmail(user.Email);
            Assert.Equal(user.UserName, username);
        }

        [Fact]
        //todo:theory
        public void DeleteUser_UserDoesNotExists_UserNotDeleted()
        {
            var user = GetUser();
            var userDeleted = _provider.DeleteUser(user.UserName, false);
            Assert.False(userDeleted);
        }

        [Fact]
        public void DeleteUser_UserExists_UserDeleted()
        {
            var user = GetUser();
            AddUser(user);
            var userDeleted = _provider.DeleteUser(user.UserName, false);
            Assert.True(userDeleted);
            Assert.Equal(0, UserStorage.Count);
        }

        [Fact]
        public void GetAllUsers_NoUsersExist_EmptyCollectionGot()
        {
            int totalRecords;
            var membershipUserCollection = _provider.GetAllUsers(0, 1, out totalRecords);
            Assert.Equal(0, membershipUserCollection.Count);
            Assert.Equal(0, totalRecords);
        }

        [Fact]
        //todo:theory
        public void GetAllUsers_UsersExist_MaxUsersCountPerPageGot()
        {
            int totalRecords;
            AddUser(GetUser());
            AddUser(GetUser());
            var membershipUserCollection = _provider.GetAllUsers(0, 1, out totalRecords);

            Assert.Equal(1, membershipUserCollection.Count);
            Assert.Equal(2, totalRecords);
        }

        [Fact]
        //todo:theory
        public void FindUsersByName_NoUsersExist_EmptyCollectionGot()
        {
            var user = GetUser();
            int totalRecords;
            var membershipUserCollection = _provider.FindUsersByName(user.UserName, 0, 1, out totalRecords);
            Assert.Equal(0, totalRecords);
            Assert.Equal(0, membershipUserCollection.Count);
        }

        [Fact]
        //todo:theory
        public void FindUsersByName_UsersExist_MaxUsersCountPerPageGot()
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
        public void FindUsersByEmail_NoUsersExist_EmptyCollectionGot()
        {
            var user = GetUser();
            int totalRecords;
            var membershipUserCollection = _provider.FindUsersByEmail(user.Email, 0, 1, out totalRecords);
            Assert.Equal(0, totalRecords);
            Assert.Equal(0, membershipUserCollection.Count);
        }


        [Fact]
        //todo:theory
        public void FindUsersByEmail_UsersExist_MaxUsersCountPerPageGot()
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