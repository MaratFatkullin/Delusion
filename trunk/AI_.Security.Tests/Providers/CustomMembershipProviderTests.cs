using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;
using AI_.Data.Repository;
using AI_.Security.DAL;
using AI_.Security.Models;
using AI_.Security.Providers;
using AI_.Security.Tests.Mocks;
using AI_.Security.Tests.UtilityClasses;
using AutoMapper;
using FluentAssertions;
using Xunit;
using Xunit.Extensions;
using Microsoft.Practices.Unity;


namespace AI_.Security.Tests.Providers
{
    public class CustomMembershipProviderTests 
    {
        private readonly CustomMembershipProvider _provider;
        private readonly SecurityUnitOfWorkMock _unitOfWork;
        private MembershipCreateStatus _membershipCreateStatus;

        protected ICollection<User> UserStorage
        {
            get { return ((RepositoryMock<User>) _unitOfWork.UserRepository).Storage; }
        }

        public CustomMembershipProviderTests()
        {
            _unitOfWork = new SecurityUnitOfWorkMock();
            _provider = new CustomMembershipProvider();
            _provider.Container.RegisterInstance<ISecurityUnitOfWork>(_unitOfWork);
            _provider.Initialize("CustomMembershipProvider", new NameValueCollection());

            Mapper.CreateMap<User, User>();
        }

        #region Utility methods

        public static IEnumerable<object[]> PagingTestData
        {
            get
            {
                // { pageSize, pageIndex, expectedItemsInPage }
                yield return new object[]
                             {
                                 2, 0, 2
                             };
                yield return new object[]
                             {
                                 2, 1, 1
                             };
                yield return new object[]
                             {
                                 2, 2, 0
                             };
            }
        }

        private MembershipUser AddUserViaProvider(User user)
        {
            user.ID = UserStorage.Count + 1;
            return _provider.CreateUser(user.UserName,
                                        user.Password,
                                        user.Email,
                                        user.PasswordQuestion,
                                        user.PasswordAnswer,
                                        user.IsApproved,
                                        user.ID,
                                        out _membershipCreateStatus);
        }

        private void AddUserDirectly(User user)
        {
            UserStorage.Add(Mapper.Map<User, User>(user));
        }

        protected void AddUsers(int count, Func<object, User> func)
        {
            for (int i = 0; i < count; i++)
            {
                AddUserDirectly(func(i));
            }
        }

        #endregion

        [Fact]
        public void CreateUser_Simple_UserCreated()
        {
            var user = UtilityMethods.GetUser();
            AddUserViaProvider(user);

            UserStorage.Single().ShouldHave().Properties(
                usr => usr.UserName,
                usr => usr.Password,
                usr => usr.Email,
                usr => usr.IsApproved,
                usr => usr.PasswordQuestion,
                usr => usr.PasswordAnswer)
                .EqualTo(user);
        }

        [Fact]
        public void CreateUser_Simple_SuccessCreateStatusReturned()
        {
            var user = UtilityMethods.GetUser();
            AddUserViaProvider(user);

            _membershipCreateStatus.Should().Be(MembershipCreateStatus.Success);
        }

        [Fact]
        public void CreateUser_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.GetUser();
            AddUserViaProvider(user);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void CreateUser_UserWithSameUserNameExists_DuplicateUserNameCreateStatusReturned()
        {
            AddUserViaProvider(UtilityMethods.GetUser());
            AddUserViaProvider(UtilityMethods.GetUser());

            _membershipCreateStatus.Should().Be(MembershipCreateStatus.DuplicateUserName);
        }

        [Fact]
        public void CreateUser_UserWithSameUserNameExists_UserNotCreated()
        {
            AddUserViaProvider(UtilityMethods.GetUser());
            AddUserViaProvider(UtilityMethods.GetUser());

            UserStorage.Should().HaveCount(1);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintEnabled_DuplicateEmailCreateStatusReturned()
        {
            var config = new NameValueCollection();
            config.Add("requiresUniqueEmail", "true");
            _provider.Configure(config);

            AddUserViaProvider(UtilityMethods.GetUser("user1"));
            AddUserViaProvider(UtilityMethods.GetUser("user2"));

            _membershipCreateStatus.Should().Be(MembershipCreateStatus.DuplicateEmail);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintEnabled_UserWithSameEmailNotCreated()
        {
            var config = new NameValueCollection();
            config.Add("requiresUniqueEmail", "true");
            _provider.Configure(config);

            AddUserViaProvider(UtilityMethods.GetUser("user1"));
            AddUserViaProvider(UtilityMethods.GetUser("user2"));

            UserStorage.Should().HaveCount(1);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintDisabled_UserWithSameEmailCreated()
        {
            AddUserViaProvider(UtilityMethods.GetUser(username: "user1"));
            AddUserViaProvider(UtilityMethods.GetUser(username: "user2"));

            _membershipCreateStatus.Should().Be(MembershipCreateStatus.Success);
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_ValidUserDataProvided_PasswordQuestionChanged()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
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
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";
            _provider.ChangePasswordQuestionAndAnswer(user.UserName,
                                                      user.Password,
                                                      newPasswordQuestion,
                                                      newPasswordAnswer);

            UserStorage.Single().PasswordAnswer.Should().Be(newPasswordAnswer);
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";
            _provider.ChangePasswordQuestionAndAnswer(user.UserName,
                                                      user.Password,
                                                      newPasswordQuestion,
                                                      newPasswordAnswer);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_InvalidUserDataProvided_PasswordQuestionNotChanged()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
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
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
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
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var newPassword = "newPassword";
            _provider.ChangePassword(user.UserName, user.Password, newPassword);

            UserStorage.Single().Password.Should().Be(newPassword);
        }

        [Fact]
        public void ChangePassword_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var newPassword = "newPassword";
            _provider.ChangePassword(user.UserName, user.Password, newPassword);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void ChangePassword_InvalidOldPasswordProvided_PasswordNotChanged()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            _provider.ChangePassword(user.UserName, "invalidPassword", "newPassword");

            UserStorage.Single().Password.Should().Be(user.Password);
        }

        [Fact]
        public void ChangePassword_ShortNewPasswordProvided_ExceptionThrown()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var newPassword = "shrtPwd";

            _provider.Invoking(p => p.ChangePassword(user.UserName, user.Password, newPassword))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void ChangePassword_NewPasswordWithWhiteSpaceProvided_ExceptionThrown()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var newPassword = "shrt Pwd";

            _provider.Invoking(p => p.ChangePassword(user.UserName, user.Password, newPassword))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void ChangePassword_NullAsNewPasswordProvided_ExceptionThrown()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            string newPassword = null;

            _provider.Invoking(p => p.ChangePassword(user.UserName, user.Password, newPassword))
                .ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ChangePassword_UserDoesNotExists_PasswordNotChangedResultReturned()
        {
            var user = UtilityMethods.GetUser();
            var newPassword = "newPassword";
            var changePassword = _provider.ChangePassword(user.UserName, user.Password, newPassword);

            changePassword.Should().BeFalse();
        }

        [Fact]
        public void UpdateUser_UserExists_UserUpdated()
        {
            AddUserDirectly(UtilityMethods.GetUser());
            var user = UtilityMethods.GetUser(email: "newEmail@a.b");
            var membershipUser = Mapper.Map<User, MembershipUser>(user);
            _provider.UpdateUser(membershipUser);
            var updatedUser = Mapper.Map<User, MembershipUser>(UserStorage.Single());

            updatedUser.ShouldHave().AllPropertiesBut(u => u.ProviderUserKey, u => u.CreationDate).EqualTo(
                membershipUser);
        }

        [Fact]
        public void UpdateUser_Simple_UnitOfWorkDisposed()
        {
            AddUserDirectly(UtilityMethods.GetUser());
            var user = UtilityMethods.GetUser(email: "newEmail@a.b");
            var membershipUser = Mapper.Map<User, MembershipUser>(user);
            _provider.UpdateUser(membershipUser);
            Mapper.Map<User, MembershipUser>(UserStorage.Single());

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void UpdateUser_UserDoesNotExists_ExceptionThrown()
        {
            var user = UtilityMethods.GetUser();
            var membershipUser = Mapper.Map<User, MembershipUser>(user);

            _provider.Invoking(p => p.UpdateUser(membershipUser))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void UnlockUser_UserDoesNotExists_ExceptionThrown()
        {
            var user = UtilityMethods.GetUser();

            _provider.Invoking(p => p.UnlockUser(user.UserName))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void UnlockUser_UserIsNotLocked_ExceptionThrown()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);

            _provider.Invoking(p => p.UnlockUser(user.UserName))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void UnlockUser_UserIsLocked_UserUnlocked()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;
            _provider.UnlockUser(user.UserName);

            UserStorage.Single().IsLocked.Should().BeFalse();
        }

        [Fact]
        public void UnlockUser_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;
            _provider.UnlockUser(user.UserName);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void GetUserByProviderUserKey_UserExists_UserReturned()
        {
            var user = UtilityMethods.GetUser();
            var addUser = AddUserViaProvider(user);
            var membershipUser = _provider.GetUser(addUser.ProviderUserKey, false);

            membershipUser.UserName.Should().Be(user.UserName);
        }

        [Fact]
        public void GetUserByProviderUserKey_UserDoesNotExists_NullReturned()
        {
            var user = UtilityMethods.GetUser();
            var membershipUser = _provider.GetUser(user.ID, false);

            membershipUser.Should().BeNull();
        }

        [Fact]
        public void GetUserByProviderUserKey_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.GetUser();
            _provider.GetUser(user.ID, false);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void ResetPassword_PasswordResetOptionIsDisabled_ExceptionThrown()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);

            _provider.Invoking(p => p.ResetPassword(user.UserName, user.PasswordAnswer))
                .ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ResetPassword_RequiredPasswordAnswerIsNull_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);

            _provider.Invoking(p => p.ResetPassword(user.UserName, null))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void ResetPassword_UserDoesNotExists_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.GetUser();

            _provider.Invoking(p => p.ResetPassword(user.UserName, user.PasswordAnswer))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void ResetPassword_UserIsLocked_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;

            _provider.Invoking(p => p.ResetPassword(user.UserName, user.PasswordAnswer))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void ResetPassword_ValidUserDataProvided_PasswordReseted()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            _provider.ResetPassword(user.UserName, user.PasswordAnswer);

            UserStorage.Single().Password.Should().NotBe(user.Password);
        }

        [Fact]
        public void ResetPassword_Simple_UnitOfWorkDisposed()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            _provider.ResetPassword(user.UserName, user.PasswordAnswer);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void ResetPassword_RequieredPasswordAnswerIsInvalid_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);

            _provider.Invoking(p => p.ResetPassword(user.UserName, "invalidAnswer"))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void GetPassword_PasswordRetrievalOptionIsDisabled_ExceptionThrown()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);

            _provider.Invoking(p => p.GetPassword(user.UserName, user.PasswordAnswer))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void GetPassword_PasswordRetrievalOptionIsEnabled_PasswordReturned()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Configure(config);

            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var password = _provider.GetPassword(user.UserName, user.PasswordAnswer);

            password.Should().Be(user.Password);
        }

        [Fact]
        public void GetPassword_Simple_UnitOfWorkDisposed()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Configure(config);

            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            _provider.GetPassword(user.UserName, user.PasswordAnswer);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void GetPassword_UserDoesNotExists_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Configure(config);

            _provider.Invoking(p => p.GetPassword("username", "passwordanswer"))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void GetPassword_UserIsLocked_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Configure(config);

            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;

            _provider.Invoking(p => p.GetPassword(user.UserName, user.PasswordAnswer))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        //todo: via theory (userIsOnLine) evrywhere
        public void GetUserByUsername_UserDoesNotExists_NullReturned()
        {
            var user = UtilityMethods.GetUser();
            var membershipUser = _provider.GetUser(user.UserName, false);

            membershipUser.Should().BeNull();
        }

        [Fact]
        //todo: via theory (userIsOnLine) evrywhere
        public void GetUserByUsername_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.GetUser();
            _provider.GetUser(user.UserName, false);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void GetUserByUsername_UserExists_UserReturned()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var membershipUser = _provider.GetUser(user.UserName, false);

            membershipUser.UserName.Should().Be(user.UserName);
        }

        [Fact]
        public void GetUserNameByEmail_UserDoesNotExists_NullReturned()
        {
            var user = UtilityMethods.GetUser();
            var username = _provider.GetUserNameByEmail(user.Email);

            username.Should().BeNull();
        }

        [Fact]
        public void GetUserNameByEmail_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.GetUser();
            _provider.GetUserNameByEmail(user.Email);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void GetUserNameByEmail_UserExists_UserNameReturned()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var username = _provider.GetUserNameByEmail(user.Email);

            username.Should().Be(user.UserName);
        }

        [Fact]
        public void DeleteUser_UserDoesNotExists_UserNotDeletedResultReturned()
        {
            var user = UtilityMethods.GetUser();
            var userDeleted = _provider.DeleteUser(user.UserName, false);

            userDeleted.Should().BeFalse();
        }

        [Fact]
        public void DeleteUser_UserExists_UserDeleted()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            _provider.DeleteUser(user.UserName, false);

            UserStorage.Should().HaveCount(0);
        }

        [Fact]
        public void DeleteUser_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            _provider.DeleteUser(user.UserName, false);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void DeleteUser_UserExists_TrueReturned()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var userDeleted = _provider.DeleteUser(user.UserName, false);

            userDeleted.Should().BeTrue();
        }

        [Fact]
        public void GetAllUsers_NoUsersExist_EmptyCollectionReturned()
        {
            int totalRecords;
            var membershipUserCollection = _provider.GetAllUsers(0, 1, out totalRecords);

            membershipUserCollection.Should().HaveCount(0);
        }

        [Fact]
        public void GetAllUsers_Simple_UnitOfWorkDisposed()
        {
            int totalRecords;
            _provider.GetAllUsers(0, 1, out totalRecords);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Theory]
        [PropertyData("PagingTestData")]
        public void GetAllUsers_UsersExist_MaxUsersCountPerPageReturned(int pageSize,
                                                                        int pageIndex,
                                                                        int expectedFoundMemberships)
        {
            int totalRecords;
            AddUsers(3, i => UtilityMethods.GetUser("user" + i));
            var membershipUserCollection = _provider.GetAllUsers(pageIndex,
                                                                 pageSize,
                                                                 out totalRecords);

            membershipUserCollection.Should().HaveCount(expectedFoundMemberships);
        }

        [Fact]
        public void GetAllUsers_UsersExist_TotalRecordsCountEquelsUsersCount()
        {
            int totalRecords;
            AddUsers(3, i => UtilityMethods.GetUser());
            _provider.GetAllUsers(0, 1, out totalRecords);

            totalRecords.Should().Be(3);
        }

        [Fact]
        public void FindUsersByName_Simple_ExceptionThrown()
        {
            int totalRecords;
            _provider.Invoking(p => p.FindUsersByName(null, 0, 0, out totalRecords))
                .ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void FindUsersByEmail_NoUsersExist_EmptyCollectionReturned()
        {
            int totalRecords;
            var membershipUserCollection = _provider.FindUsersByEmail("a@b.c", 0, 1, out totalRecords);

            membershipUserCollection.Should().BeEmpty();
        }

        [Fact]
        public void FindUsersByEmail_Simple_UnitOfWorkDisposed()
        {
            int totalRecords;
            _provider.FindUsersByEmail("a@b.c", 0, 1, out totalRecords);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Theory]
        [PropertyData("PagingTestData")]
        public void FindUsersByEmail_UsersExist_MaxUsersCountPerPageReturned(int pageSize,
                                                                             int pageIndex,
                                                                             int expectedFoundMemberships)
        {
            int totalRecords;
            AddUsers(3, i => UtilityMethods.GetUser(username: "user" + i, email: "a@b.c"));
            var membershipUserCollection = _provider.FindUsersByEmail("a@b.c",
                                                                      pageIndex,
                                                                      pageSize,
                                                                      out totalRecords);

            membershipUserCollection.Should().HaveCount(expectedFoundMemberships);
        }

        [Fact]
        public void FindUsersByEmail_UsersExist_TotalRecordsCountEquelsMatchingUsersCount()
        {
            int totalRecords;
            AddUsers(3, i => UtilityMethods.GetUser(email: "a@b.c"));
            AddUsers(1, i => UtilityMethods.GetUser(email: "b@b.c"));
            _provider.FindUsersByEmail("a@b.c", 0, 1, out totalRecords);

            totalRecords.Should().Be(3);
        }

        [Fact]
        public void ValidateUser_UserIsLocked_UserNotValid()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;
            var isValid = _provider.ValidateUser(user.UserName, user.Password);

            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateUser_UserDoesNotExists_UserNotValid()
        {
            var isValid = _provider.ValidateUser("username", "password");

            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateUser_UserIsNotApproved_UserNotValid()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            UserStorage.Single().IsApproved = false;
            var isValid = _provider.ValidateUser(user.UserName, user.Password);

            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateUser_InvalidPasswordProvided_UserNotValid()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var isValid = _provider.ValidateUser(user.UserName, "invalidPassword");

            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateUser_Simple_UserValid()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            var isValid = _provider.ValidateUser(user.UserName, user.Password);

            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidateUser_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.GetUser();
            AddUserDirectly(user);
            _provider.ValidateUser(user.UserName, user.Password);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }
    }
}