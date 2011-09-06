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
using Xunit.Extensions;
using FluentAssertions;

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

        private void AddUsers(int count,Func<object, User> func)
        {
            for (int i = 0; i < count; i++)
            {
                UserStorage.Add(func(i));
            }
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

        public static IEnumerable<object[]> PagingTestData
        {
            get
            {
                // { pageSize, pageIndex, expectedItemsInPage }
                yield return new object[] { 1, 0, 1 };
                yield return new object[] { 1, 1, 1 };
                yield return new object[] { 2, 0, 2 };
                yield return new object[] { 2, 1, 0 };
            }
        }

        #endregion

        [Fact]
        public void CreateUser_Simple_UserCreated()
        {
            var user = GetUser();
            AddUser(user);
            UserStorage.Should().HaveCount(1).And.Contain(user);
        }

        [Fact]
        public void CreateUser_Simple_SuccessCreateStatusReturned()
        {
            var user = GetUser();
            AddUser(user);
            _membershipCreateStatus.Should().Be(MembershipCreateStatus.Success);
        }

        [Fact]
        public void CreateUser_UserWithSameUserNameExists_DuplicateUserNameCreateStatusReturned()
        {
            AddUser(GetUser());
            AddUser(GetUser());
            _membershipCreateStatus.Should().Be(MembershipCreateStatus.DuplicateUserName);
        }

        [Fact]
        public void CreateUser_UserWithSameUserNameExists_UserNotCreated()
        {
            AddUser(GetUser());
            AddUser(GetUser());
            UserStorage.Should().HaveCount(1);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintEnabled_DuplicateEmailCreateStatusReturned()
        {
            var config = new NameValueCollection();
            config.Add("requiresUniqueEmail", "true");
            _provider.Initialize("name", config);

            AddUser(GetUser("user1"));
            AddUser(GetUser("user2"));
            _membershipCreateStatus.Should().Be(MembershipCreateStatus.DuplicateEmail);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintEnabled_UserWithSameEmailNotCreated()
        {
            var config = new NameValueCollection();
            config.Add("requiresUniqueEmail", "true");
            _provider.Initialize("name", config);

            AddUser(GetUser("user1"));
            AddUser(GetUser("user2"));
            UserStorage.Should().HaveCount(1);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintDisabled_UserWithSameEmailCreated()
        {
            AddUser(GetUser(username: "user1"));
            AddUser(GetUser(username: "user2"));

            _membershipCreateStatus.Should().Be(MembershipCreateStatus.Success);
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_ValidUserDataProvided_PasswordQuestionChanged()
        {
            var user = GetUser();
            AddUser(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            _provider.ChangePasswordQuestionAndAnswer(user.UserName,
                                                      user.Password,
                                                      newPasswordQuestion,
                                                      newPasswordAnswer);

            UserStorage.Single().PasswordQuestion.Should().Be(newPasswordQuestion);
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_ValidUserDataProvided_PasswordAnswerChanged()
        {
            var user = GetUser();
            AddUser(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            _provider.ChangePasswordQuestionAndAnswer(user.UserName,
                                                      user.Password,
                                                      newPasswordQuestion,
                                                      newPasswordAnswer);

            UserStorage.Single().PasswordAnswer.Should().Be(newPasswordAnswer);
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_InvalidUserDataProvided_PasswordQuestionNotChanged()
        {
            var user = GetUser();
            AddUser(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            _provider.ChangePasswordQuestionAndAnswer(user.UserName,
                                                      "invalidPassword",
                                                      newPasswordQuestion,
                                                      newPasswordAnswer);

            UserStorage.Single().PasswordQuestion.Should().Be(user.PasswordQuestion);
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_InvalidUserDataProvided_PasswordAnswerNotChanged()
        {
            var user = GetUser();
            AddUser(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            _provider.ChangePasswordQuestionAndAnswer(user.UserName,
                                                      "invalidPassword",
                                                      newPasswordQuestion,
                                                      newPasswordAnswer);

            UserStorage.Single().PasswordAnswer.Should().Be(user.PasswordAnswer);
        }

        [Fact]
        public void ChangePassword_ValidPasswordProvided_PasswordChanged()
        {
            var user = GetUser();
            AddUser(user);
            var newPassword = "newPassword";
            _provider.ChangePassword(user.UserName, user.Password, newPassword);

            UserStorage.Single().Password.Should().Be(newPassword);
        }

        [Fact]
        public void ChangePassword_InvalidOldPasswordProvided_PasswordNotChanged()
        {
            var user = GetUser();
            AddUser(user);
            _provider.ChangePassword(user.UserName, "invalidPassword", "newPassword");

            UserStorage.Single().Password.Should().Be(user.Password);
        }

        [Fact]
        public void ChangePassword_InvalidNewPasswordProvided_ExceptionThrown()
        {
            var user = GetUser();
            AddUser(user);
            var newPassword = "";

            _provider.Invoking(p => p.ChangePassword(user.UserName, user.Password, newPassword))
                .ShouldThrow<MembershipPasswordException>();
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
        public void GetUserByProviderUserKey_UserExists_UserReturned()
        {
            var user = GetUser();
            AddUser(user);
            var membershipUser = _provider.GetUser(user.ProviderUserKey, false);
            Assert.Equal(user.UserName, membershipUser.UserName);
        }

        [Fact]
        public void GetUserByProviderUserKey_UserDoesNotExists_NullReturned()
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
        public void GetPassword_PasswordRetrievalOptionIsEnabled_PasswordReturned()
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
        public void GetUserByUsername_UserDoesNotExists_NullReturned()
        {
            var user = GetUser();
            var membershipUser = _provider.GetUser(user.UserName, false);
            Assert.Null(membershipUser);
        }

        [Fact]
        public void GetUserByUsername_UserExists_UserReturned()
        {
            var user = GetUser();
            AddUser(user);
            var membershipUser = _provider.GetUser(user.UserName, false);
            Assert.Equal(user.UserName, membershipUser.UserName);
        }

        [Fact]
        public void GetUserNameByEmail_UserDoesNotExists_NullReturned()
        {
            var user = GetUser();
            var username = _provider.GetUserNameByEmail(user.Email);
            Assert.Null(username);
        }

        [Fact]
        public void GetUserNameByEmail_UserExists_UserNameReturned()
        {
            var user = GetUser();
            AddUser(user);
            var username = _provider.GetUserNameByEmail(user.Email);
            Assert.Equal(user.UserName, username);
        }

        [Fact]
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
            _provider.DeleteUser(user.UserName, false);
            Assert.Equal(0, UserStorage.Count);
        }

        [Fact]
        public void DeleteUser_UserExists_TrueReturned()
        {
            var user = GetUser();
            AddUser(user);
            var userDeleted = _provider.DeleteUser(user.UserName, false);
            Assert.True(userDeleted);
        }

        [Fact]
        public void GetAllUsers_NoUsersExist_EmptyCollectionReturned()
        {
            int totalRecords;
            var membershipUserCollection = _provider.GetAllUsers(0, 1, out totalRecords);
            Assert.Equal(0, membershipUserCollection.Count);
        }

        [Theory]
        [PropertyData("PagingTestData")]
        public void GetAllUsers_UsersExist_MaxUsersCountPerPageReturned(int pageSize,
                                                                   int pageIndex,
                                                                   int expectedFoundMemberships)
        {
            int totalRecords;
            AddUsers(3, i => GetUser());
            var membershipUserCollection = _provider.GetAllUsers(pageIndex,
                                                                 pageSize, 
                                                                 out totalRecords);

            Assert.Equal(expectedFoundMemberships, membershipUserCollection.Count);
        }

        [Fact]
        public void GetAllUsers_UsersExist_TotalRecordsCountEquelsUsersCount()
        {
            int totalRecords;
            AddUsers(3, i => GetUser());
            _provider.GetAllUsers(0, 1, out totalRecords);

            Assert.Equal(3, totalRecords);
        }

        [Fact]
        public void FindUsersByName_NoUsersExist_EmptyCollectionReturned()
        {
            int totalRecords;
            var membershipUserCollection = _provider.FindUsersByName("user0", 0, 1, out totalRecords);
            Assert.Equal(0, membershipUserCollection.Count);
        }

        [Theory]
        [PropertyData("PagingTestData")]
        public void FindUsersByName_UsersExist_MaxUsersCountPerPageReturned(int pageSize,
                                                                       int pageIndex,
                                                                       int expectedFoundMemberships)
        {
            int totalRecords;
            AddUsers(3, i => GetUser("user"));
            var membershipUserCollection = _provider.FindUsersByName("user",
                                                                     pageIndex,
                                                                     pageSize,
                                                                     out totalRecords);
            Assert.Equal(expectedFoundMemberships, membershipUserCollection.Count);
        }

        [Fact]
        public void FindUsersByName_UsersExist_TotalRecordsCountEquelsMatchingUsersCount()
        {
            int totalRecords;
            AddUsers(3, i => GetUser("user1"));
            AddUsers(1, i => GetUser("user2"));
            _provider.FindUsersByName("user1", 0, 1, out totalRecords);
            Assert.Equal(3, totalRecords);
        }

        [Fact]
        public void FindUsersByEmail_NoUsersExist_EmptyCollectionReturned()
        {
            int totalRecords;
            var membershipUserCollection = _provider.FindUsersByEmail("a@b.c", 0, 1, out totalRecords);
            Assert.Equal(0, membershipUserCollection.Count);
        }


        [Theory]
        [PropertyData("PagingTestData")]
        public void FindUsersByEmail_UsersExist_MaxUsersCountPerPageReturned(int pageSize,
                                                                        int pageIndex,
                                                                        int expectedFoundMemberships)
        {
            int totalRecords;
            AddUsers(3, i => GetUser(email: "a@b.c"));
            var membershipUserCollection = _provider.FindUsersByEmail("a@b.c",
                                                                      pageIndex,
                                                                      pageSize,
                                                                      out totalRecords);
            Assert.Equal(expectedFoundMemberships, membershipUserCollection.Count);
        }

        [Fact]
        public void FindUsersByEmail_UsersExist_TotalRecordsCountEquelsMatchingUsersCount()
        {
            int totalRecords;
            AddUsers(3, i => GetUser(email: "a@b.c"));
            AddUsers(1, i => GetUser(email: "b@b.c"));
            _provider.FindUsersByEmail("a@b.c",0,1,out totalRecords);
            Assert.Equal(3, totalRecords);
        }

        [Fact]
        public void ValidateUser_UserIsLocked_UserNotValid()
        {
            var user = GetUser();
            AddUser(user);
            UserStorage.Single().IsLocked = true;
            var isValid = _provider.ValidateUser(user.UserName, user.Password);
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateUser_UserDoesNotExists_UserNotValid()
        {
            var isValid = _provider.ValidateUser("username", "password");
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateUser_UserIsNotApproved_UserNotValid()
        {
            var user = GetUser();
            AddUser(user);
            UserStorage.Single().IsApproved = false;
            var isValid = _provider.ValidateUser(user.UserName, user.Password);
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateUser_InvalidPasswordProvided_UserNotValid()
        {
            var user = GetUser();
            AddUser(user);
            var isValid = _provider.ValidateUser(user.UserName, "invalidPassword");
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateUser_Simple_UserValid()
        {
            var user = GetUser();
            AddUser(user);
            var isValid = _provider.ValidateUser(user.UserName, user.Password);
            Assert.True(isValid);
        }
    }
}