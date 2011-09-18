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
using AutoMapper;
using FluentAssertions;
using Xunit;
using Xunit.Extensions;


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
            _provider = new CustomMembershipProvider(new UnitOfWorkFactoryMock(_unitOfWork));
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
            _unitOfWork.UserRepository.Insert(Mapper.Map<User, User>(user));
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
            var user = UtilityMethods.CreateUser();
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
            var user = UtilityMethods.CreateUser();
            AddUserViaProvider(user);

            _membershipCreateStatus.Should().Be(MembershipCreateStatus.Success);
        }

        [Fact]
        public void CreateUser_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.CreateUser();
            AddUserViaProvider(user);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void CreateUser_UserNameInDifferentCaseProvided_CretedUsersNameInLowerCase()
        {
            var user = UtilityMethods.CreateUser("UserName");
            AddUserViaProvider(user);

            UserStorage.Single().UserName.Should().Be(user.UserName.ToLower());
        }

        [Fact]
        public void CreateUser_UserWithSameUserNameExists_DuplicateUserNameCreateStatusReturned()
        {
            AddUserViaProvider(UtilityMethods.CreateUser());
            AddUserViaProvider(UtilityMethods.CreateUser());

            _membershipCreateStatus.Should().Be(MembershipCreateStatus.DuplicateUserName);
        }

        [Fact]
        public void CreateUser_UserWithSameUserNameExists_UserNotCreated()
        {
            AddUserViaProvider(UtilityMethods.CreateUser());
            AddUserViaProvider(UtilityMethods.CreateUser());

            UserStorage.Should().HaveCount(1);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintEnabled_DuplicateEmailCreateStatusReturned()
        {
            var config = new NameValueCollection();
            config.Add("requiresUniqueEmail", "true");
            _provider.Configure(config);

            AddUserViaProvider(UtilityMethods.CreateUser("user1"));
            AddUserViaProvider(UtilityMethods.CreateUser("user2"));

            _membershipCreateStatus.Should().Be(MembershipCreateStatus.DuplicateEmail);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintEnabled_UserWithSameEmailNotCreated()
        {
            var config = new NameValueCollection();
            config.Add("requiresUniqueEmail", "true");
            _provider.Configure(config);

            AddUserViaProvider(UtilityMethods.CreateUser("user1"));
            AddUserViaProvider(UtilityMethods.CreateUser("user2"));

            UserStorage.Should().HaveCount(1);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintDisabled_UserWithSameEmailCreated()
        {
            AddUserViaProvider(UtilityMethods.CreateUser(username: "user1"));
            AddUserViaProvider(UtilityMethods.CreateUser(username: "user2"));

            _membershipCreateStatus.Should().Be(MembershipCreateStatus.Success);
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_ValidUserDataProvided_PasswordQuestionChanged()
        {
            var user = UtilityMethods.CreateUser();
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
        public void ChangePasswordQuestionAndAnswer_UserNameInDifferentCaseProvided_PasswordAnswerChanged()
        {
            var user = UtilityMethods.CreateUser("username");
            AddUserDirectly(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            _provider.ChangePasswordQuestionAndAnswer("UserName",
                                                       user.Password,
                                                       newPasswordQuestion,
                                                       newPasswordAnswer);

            UserStorage.Single().PasswordAnswer.Should().Be(newPasswordAnswer);

            UserStorage.Single().PasswordAnswer.Should().Be(newPasswordAnswer);

        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_ValidUserDataProvided_PasswordAnswerChanged()
        {
            var user = UtilityMethods.CreateUser();
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
            var user = UtilityMethods.CreateUser();
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
            var user = UtilityMethods.CreateUser();
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
            var user = UtilityMethods.CreateUser();
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
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var newPassword = "newPassword";
            _provider.ChangePassword(user.UserName, user.Password, newPassword);

            UserStorage.Single().Password.Should().Be(newPassword);
        }

        [Fact]
        public void ChangePassword_UserNameInDifferentCaseProvided_PasswordChanged()
        {
            var user = UtilityMethods.CreateUser("username");
            AddUserDirectly(user);
            var newPassword = "newPassword";
            _provider.ChangePassword("UserName", user.Password, newPassword);

            UserStorage.Single().Password.Should().Be(newPassword);
        }

        [Fact]
        public void ChangePassword_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var newPassword = "newPassword";
            _provider.ChangePassword(user.UserName, user.Password, newPassword);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void ChangePassword_InvalidOldPasswordProvided_PasswordNotChanged()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            _provider.ChangePassword(user.UserName, "invalidPassword", "newPassword");

            UserStorage.Single().Password.Should().Be(user.Password);
        }

        [Fact]
        public void ChangePassword_ShortNewPasswordProvided_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var newPassword = "shrtPwd";

            _provider.Invoking(p => p.ChangePassword(user.UserName, user.Password, newPassword))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void ChangePassword_NewPasswordWithWhiteSpaceProvided_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var newPassword = "shrt Pwd";

            _provider.Invoking(p => p.ChangePassword(user.UserName, user.Password, newPassword))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void ChangePassword_NullAsNewPasswordProvided_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            string newPassword = null;

            _provider.Invoking(p => p.ChangePassword(user.UserName, user.Password, newPassword))
                .ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ChangePassword_UserNotExists_PasswordNotChangedResultReturned()
        {
            var user = UtilityMethods.CreateUser();
            var newPassword = "newPassword";
            var changePassword = _provider.ChangePassword(user.UserName, user.Password, newPassword);

            changePassword.Should().BeFalse();
        }

        //todo: пересмотреть весь UpdateUser нахрен
        [Fact]
        public void UpdateUser_UserExists_UserUpdated()
        {
            AddUserDirectly(UtilityMethods.CreateUser());
            var user = UtilityMethods.CreateUser(email: "newEmail@a.b");
            var membershipUser = Mapper.Map<User, MembershipUser>(user);
            _provider.UpdateUser(membershipUser);
            var updatedUser = Mapper.Map<User, MembershipUser>(UserStorage.Single());

            updatedUser.ShouldHave().AllPropertiesBut(u => u.ProviderUserKey, u => u.CreationDate).EqualTo(
                membershipUser);
        }

        [Fact]
        public void UpdateUser_UserNameInDifferentCaseProvided_ExceptionNotThrown()
        {
            var user = UtilityMethods.CreateUser(username:"username",email: "newEmail@a.b");
            AddUserDirectly(user);
            user.UserName = "UserName";
            var membershipUser = Mapper.Map<User, MembershipUser>(user);

            _provider.Invoking(p=>p.UpdateUser(membershipUser)).ShouldNotThrow();
        }

        [Fact]
        public void UpdateUser_Simple_UnitOfWorkDisposed()
        {
            AddUserDirectly(UtilityMethods.CreateUser());
            var user = UtilityMethods.CreateUser(email: "newEmail@a.b");
            var membershipUser = Mapper.Map<User, MembershipUser>(user);
            _provider.UpdateUser(membershipUser);
            Mapper.Map<User, MembershipUser>(UserStorage.Single());

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void UpdateUser_UserDoesNotExists_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            var membershipUser = Mapper.Map<User, MembershipUser>(user);

            _provider.Invoking(p => p.UpdateUser(membershipUser))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void UnlockUser_UserNotExists_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();

            _provider.Invoking(p => p.UnlockUser(user.UserName))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void UnlockUser_UserNotLocked_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            _provider.Invoking(p => p.UnlockUser(user.UserName))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void UnlockUser_UserLocked_UserUnlocked()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;
            _provider.UnlockUser(user.UserName);

            UserStorage.Single().IsLocked.Should().BeFalse();
        }

        [Fact]
        public void UnlockUser_UserNameInDifferentCaseProvided_ExceptionNotThrown()
        {
            var user = UtilityMethods.CreateUser("username");
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;

            _provider.Invoking(p => p.UnlockUser("UserName"))
                .ShouldNotThrow();
        }

        [Fact]
        public void UnlockUser_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;
            _provider.UnlockUser(user.UserName);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void GetUserByProviderUserKey_UserExists_UserReturned()
        {
            var user = UtilityMethods.CreateUser();
            var addUser = AddUserViaProvider(user);
            var membershipUser = _provider.GetUser(addUser.ProviderUserKey, false);

            membershipUser.UserName.Should().Be(user.UserName);
        }

        [Fact]
        public void GetUserByProviderUserKey_UserNotExists_NullReturned()
        {
            var user = UtilityMethods.CreateUser();
            var membershipUser = _provider.GetUser(user.ID, false);

            membershipUser.Should().BeNull();
        }

        [Fact]
        public void GetUserByProviderUserKey_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.CreateUser();
            _provider.GetUser(user.ID, false);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void ResetPassword_PasswordResetOptionDisabled_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
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

            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            _provider.Invoking(p => p.ResetPassword(user.UserName, null))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void ResetPassword_UserNotExists_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.CreateUser();

            _provider.Invoking(p => p.ResetPassword(user.UserName, user.PasswordAnswer))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void ResetPassword_UserLocked_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.CreateUser();
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

            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            _provider.ResetPassword(user.UserName, user.PasswordAnswer);

            UserStorage.Single().Password.Should().NotBe(user.Password);
        }

        [Fact]
        public void ResetPassword_UserNameInDifferentCaseProvided_ExceptionNotThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.CreateUser("username");
            AddUserDirectly(user);

            _provider.Invoking(p=>p.ResetPassword("UserName", user.PasswordAnswer))
                .ShouldNotThrow();
        }

        [Fact]
        public void ResetPassword_Simple_UnitOfWorkDisposed()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            _provider.ResetPassword(user.UserName, user.PasswordAnswer);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void ResetPassword_RequieredPasswordAnswerInvalid_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordReset", "true");
            config.Add("requiresQuestionAndAnswer", "true");
            _provider.Configure(config);

            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            _provider.Invoking(p => p.ResetPassword(user.UserName, "invalidAnswer"))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void GetPassword_PasswordRetrievalOptionDisabled_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            _provider.Invoking(p => p.GetPassword(user.UserName, user.PasswordAnswer))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void GetPassword_PasswordRetrievalOptionEnabled_PasswordReturned()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Configure(config);

            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var password = _provider.GetPassword(user.UserName, user.PasswordAnswer);

            password.Should().Be(user.Password);
        }

        [Fact]
        public void GetPassword_UserNameInDifferentCaseProvided_ExceptionNotThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Configure(config);

            var user = UtilityMethods.CreateUser("username");
            AddUserDirectly(user);
            var password = _provider.GetPassword("UserName", user.PasswordAnswer);

            password.Should().Be(user.Password);
        }

        [Fact]
        public void GetPassword_Simple_UnitOfWorkDisposed()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Configure(config);

            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            _provider.GetPassword(user.UserName, user.PasswordAnswer);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void GetPassword_UserNotExists_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Configure(config);

            _provider.Invoking(p => p.GetPassword("username", "passwordanswer"))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        public void GetPassword_UserLocked_ExceptionThrown()
        {
            var config = new NameValueCollection();
            config.Add("enablePasswordRetrieval", "true");
            _provider.Configure(config);

            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;

            _provider.Invoking(p => p.GetPassword(user.UserName, user.PasswordAnswer))
                .ShouldThrow<MembershipPasswordException>();
        }

        [Fact]
        //todo: via theory (userIsOnLine) evrywhere
        public void GetUserByUsername_UserNotExists_NullReturned()
        {
            var user = UtilityMethods.CreateUser();
            var membershipUser = _provider.GetUser(user.UserName, false);

            membershipUser.Should().BeNull();
        }

        [Fact]
        //todo: via theory (userIsOnLine) evrywhere
        public void GetUserByUsername_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.CreateUser();
            _provider.GetUser(user.UserName, false);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void GetUserByUsername_UserExists_UserReturned()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var membershipUser = _provider.GetUser(user.UserName, false);

            membershipUser.UserName.Should().Be(user.UserName);
        }

        [Fact]
        public void GetUserByUsername_UserNameInDifferentCaseProvided_ExceptionNotThrown()
        {
            var user = UtilityMethods.CreateUser("username");
            AddUserDirectly(user);

            var membershipUser = _provider.GetUser("UserName", false);

            membershipUser.UserName.Should().Be(user.UserName);
        }

        [Fact]
        public void GetUserNameByEmail_UserNotExists_NullReturned()
        {
            var user = UtilityMethods.CreateUser();
            var username = _provider.GetUserNameByEmail(user.Email);

            username.Should().BeNull();
        }

        [Fact]
        public void GetUserNameByEmail_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.CreateUser();
            _provider.GetUserNameByEmail(user.Email);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void GetUserNameByEmail_UserExists_UserNameReturned()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var username = _provider.GetUserNameByEmail(user.Email);

            username.Should().Be(user.UserName);
        }

        [Fact]
        public void DeleteUser_UserNotExists_UserNotDeletedResultReturned()
        {
            var user = UtilityMethods.CreateUser();
            var userDeleted = _provider.DeleteUser(user.UserName, false);

            userDeleted.Should().BeFalse();
        }

        [Fact]
        public void DeleteUser_UserExists_UserDeleted()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            _provider.DeleteUser(user.UserName, false);

            UserStorage.Should().HaveCount(0);
        }

        [Fact]
        public void DeleteUser_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            _provider.DeleteUser(user.UserName, false);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void DeleteUser_UserExists_UserDeletedResultReturned()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var userDeleted = _provider.DeleteUser(user.UserName, false);

            userDeleted.Should().BeTrue();
        }

        [Fact]
        public void DeleteUser_UserNameInDifferentCaseProvided_UserDeleted()
        {
            var user = UtilityMethods.CreateUser("username");
            AddUserDirectly(user);
            _provider.DeleteUser("UserName", false);

            UserStorage.Should().BeEmpty();
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
            AddUsers(3, i => UtilityMethods.CreateUser("user" + i));
            var membershipUserCollection = _provider.GetAllUsers(pageIndex,
                                                                 pageSize,
                                                                 out totalRecords);

            membershipUserCollection.Should().HaveCount(expectedFoundMemberships);
        }

        [Fact]
        public void GetAllUsers_UsersExist_TotalRecordsCountEquelsUsersCount()
        {
            int totalRecords;
            AddUsers(3, i => UtilityMethods.CreateUser());
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
            AddUsers(3, i => UtilityMethods.CreateUser(username: "user" + i, email: "a@b.c"));
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
            AddUsers(3, i => UtilityMethods.CreateUser(email: "a@b.c"));
            AddUsers(1, i => UtilityMethods.CreateUser(email: "b@b.c"));
            _provider.FindUsersByEmail("a@b.c", 0, 1, out totalRecords);

            totalRecords.Should().Be(3);
        }

        [Fact]
        public void ValidateUser_UserIsLocked_UserNotValid()
        {
            var user = UtilityMethods.CreateUser();
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
        public void ValidateUser_UserNotApproved_UserNotValid()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            UserStorage.Single().IsApproved = false;
            var isValid = _provider.ValidateUser(user.UserName, user.Password);

            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateUser_InvalidPasswordProvided_UserNotValid()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var isValid = _provider.ValidateUser(user.UserName, "invalidPassword");

            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateUser_Simple_UserValid()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var isValid = _provider.ValidateUser(user.UserName, user.Password);

            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidateUser_UserNameInDifferentCaseProvided_UserValid()
        {
            var user = UtilityMethods.CreateUser("username");
            AddUserDirectly(user);
            var isValid = _provider.ValidateUser("UserName", user.Password);

            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidateUser_Simple_UnitOfWorkDisposed()
        {
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            _provider.ValidateUser(user.UserName, user.Password);

            _unitOfWork.IsDisposed.Should().BeTrue();
        }
    }
}