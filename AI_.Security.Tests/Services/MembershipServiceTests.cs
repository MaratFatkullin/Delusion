﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using AI_.Data.Repository;
using AI_.Security.Models;
using AI_.Security.Services;
using AI_.Security.Tests.Mocks;
using AI_.Security.Tests.UtilityClasses;
using FluentAssertions;
using Xunit;
using Xunit.Extensions;

namespace AI_.Security.Tests.Services
{
    public class MembershipServiceTests
    {
        private readonly MembershipService _service;
        private readonly IUnitOfWork _unitOfWork;
        private MembershipCreateStatus _membershipCreateStatus;

        protected ICollection<User> UserStorage
        {
            get { return ((RepositoryMock<User>) _unitOfWork.GetRepository<User>()).Storage; }
        }

        public MembershipServiceTests()
        {
            _unitOfWork = new UnitOfWorkMock();
            _service = new MembershipService(_unitOfWork);
        }

        #region Utility methods

        public static IEnumerable<object[]> PagingTestData
        {
            get
            {
                // { pageSize, pageIndex, expectedItemsInPage }
                yield return new object[] {2, 0, 2};
                yield return new object[] {2, 1, 1};
                yield return new object[] {2, 2, 0};
            }
        }

        public static IEnumerable<object[]> UsernameInDifferentCase
        {
            get
            {
                yield return new object[] {"username"};
                yield return new object[] {"UserName"};
            }
        }

        private void AddUserViaProvider(User user)
        {
            _service.CreateUser(user.UserName,
                                user.Password,
                                user.Email,
                                user.PasswordQuestion,
                                user.PasswordAnswer,
                                user.IsApproved,
                                out _membershipCreateStatus);
        }

        private void AddUserDirectly(User user)
        {
            _unitOfWork.GetRepository<User>().Insert(user);
            _unitOfWork.Save();
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
            // Arrange
            var user = UtilityMethods.CreateUser();

            // Act
            AddUserViaProvider(user);

            // Assert
            _membershipCreateStatus.Should().Be(MembershipCreateStatus.Success);
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
        public void CreateUser_UserNameInDifferentCaseProvided_CreatedUsersUserNameInLowerCase()
        {
            // Arrange
            const string initialUsername = "UserName";
            var user = UtilityMethods.CreateUser(initialUsername);

            // Act
            AddUserViaProvider(user);

            // Assert
            var savedUsername = UserStorage.Single().UserName;
            savedUsername.Should().Be(initialUsername.ToLower());
        }

        [Fact]
        public void CreateUser_EmailInDifferentCaseProvided_CreatedUsersEmailInLowerCase()
        {
            // Arrange
            const string initialEmail = "A@b.C";
            var user = UtilityMethods.CreateUser(email: initialEmail);

            // Act
            AddUserViaProvider(user);

            // Assert
            var savedEmail = UserStorage.Single().Email;
            savedEmail.Should().Be(initialEmail.ToLower());
        }

        [Fact]
        public void CreateUser_UserWithSameUsernameExists_UserNotCreated()
        {
            // Arrange
            AddUserViaProvider(UtilityMethods.CreateUser());

            // Act
            AddUserViaProvider(UtilityMethods.CreateUser());

            // Assert
            _membershipCreateStatus.Should().Be(MembershipCreateStatus.DuplicateUserName);
            UserStorage.Should().HaveCount(1);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintNotSatisfied_UserNotCreated()
        {
            // Arrange
            _service.RequiresUniqueEmail = true;
            AddUserViaProvider(UtilityMethods.CreateUser("user1"));

            // Act
            AddUserViaProvider(UtilityMethods.CreateUser("user2"));

            // Assert
            _membershipCreateStatus.Should().Be(MembershipCreateStatus.DuplicateEmail);
            UserStorage.Should().HaveCount(1);
        }

        [Fact]
        public void CreateUser_UniqueEmailConstraintDisabled_UserCreated()
        {
            // Arrange
            _service.RequiresUniqueEmail = false;
            AddUserViaProvider(UtilityMethods.CreateUser("user1"));

            // Act
            AddUserViaProvider(UtilityMethods.CreateUser("user2"));

            // Assert
            _membershipCreateStatus.Should().Be(MembershipCreateStatus.Success);
            UserStorage.Should().HaveCount(2);
        }

        [Fact]
        public void CreateUser_RequiredEmailNotProvided_UserNotCreated()
        {
            // Arrange
            _service.RequiresEmail = true;

            // Act
            AddUserViaProvider(UtilityMethods.CreateUser(email: string.Empty));

            // Assert
            _membershipCreateStatus.Should().Be(MembershipCreateStatus.InvalidEmail);
            UserStorage.Should().BeEmpty();
        }

        [Fact]
        public void CreateUser_RequiredPasswordQuestionNotProvided_UserNotCreated()
        {
            // Arrange
            _service.RequiresQuestionAndAnswer = true;

            // Act
            AddUserViaProvider(UtilityMethods.CreateUser(passwordQuestion: string.Empty));

            // Assert
            _membershipCreateStatus.Should().Be(MembershipCreateStatus.InvalidQuestion);
            UserStorage.Should().BeEmpty();
        }

        [Fact]
        public void CreateUser_RequiredPasswordAnswerNotProvided_UserNotCreated()
        {
            // Arrange
            _service.RequiresQuestionAndAnswer = true;

            // Act
            AddUserViaProvider(UtilityMethods.CreateUser(passwordAnswer: string.Empty));

            // Assert
            _membershipCreateStatus.Should().Be(MembershipCreateStatus.InvalidAnswer);
            UserStorage.Should().BeEmpty();
        }

        [Fact]
        public void ChangePasswordQuestionAndAnswer_ValidUserDataProvided_QuestionAndAnswerChanged()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            // Act
            var changed = _service.ChangePasswordQuestionAndAnswer(user.UserName,
                                                                   user.Password,
                                                                   newPasswordQuestion,
                                                                   newPasswordAnswer);
            // Assert
            UserStorage.Single().PasswordQuestion.Should().Be(newPasswordQuestion);
            UserStorage.Single().PasswordAnswer.Should().Be(newPasswordAnswer);
            changed.Should().BeTrue();
        }


        [Fact]
        public void ChangePasswordQuestionAndAnswer_InvalidUserDataProvided_QuestionAndAnswerNotChanged()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            // Act
            var changed = _service.ChangePasswordQuestionAndAnswer(user.UserName,
                                                                   "invalidPassword",
                                                                   newPasswordQuestion,
                                                                   newPasswordAnswer);

            // Assert
            UserStorage.Single().PasswordQuestion.Should().Be(user.PasswordQuestion);
            UserStorage.Single().PasswordAnswer.Should().Be(user.PasswordAnswer);
            changed.Should().BeFalse();
        }

        [Fact]
        public void ChangePassword_ValidPasswordProvided_PasswordChanged()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            const string newPassword = "newPassword";

            // Act
            _service.ChangePassword("username", user.Password, newPassword);

            // Assert
            UserStorage.Single().Password.Should().Be(newPassword);
        }

        [Theory]
        [PropertyData("UsernameInDifferentCase")]
        public void ChangePasswordQuestionAndAnswer_Simple_UsernamesCaseIgnored(string username)
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            var newPasswordQuestion = "newPasswordQuestion";
            var newPasswordAnswer = "newPasswordAnswer";

            // Act
            var changed = _service.ChangePasswordQuestionAndAnswer(username,
                                                                   user.Password,
                                                                   newPasswordQuestion,
                                                                   newPasswordAnswer);
            // Assert
            changed.Should().BeTrue();
        }

        [Fact]
        public void ChangePassword_InvalidOldPasswordProvided_PasswordNotChanged()
        {
            // Arrange
            const string oldPassword = "oldPassword";
            var user = UtilityMethods.CreateUser(password: oldPassword);
            AddUserDirectly(user);

            // Act
            var passwordChanged = _service.ChangePassword(user.UserName, "invalidPassword", "newPassword");

            // Assert
            passwordChanged.Should().BeFalse();
            UserStorage.Single().Password.Should().Be(oldPassword);
        }

        [Fact]
        public void ChangePassword_ShortNewPasswordProvided_PasswordNotChanged()
        {
            // Arrange
            _service.MinRequiredPasswordLength = 8;
            const string oldPassword = "oldPassword";
            var user = UtilityMethods.CreateUser(password: oldPassword);
            AddUserDirectly(user);

            // Act
            var passwordChanged = _service.ChangePassword(user.UserName, user.Password, "shrtPwd");

            // Assert
            passwordChanged.Should().BeFalse();
            UserStorage.Single().Password.Should().Be(oldPassword);
        }

        [Fact]
        public void ChangePassword_NewPasswordWithWhiteSpaceProvided_PasswordNotChanged()
        {
            // Arrange
            const string oldPassword = "oldPassword";
            var user = UtilityMethods.CreateUser(password: oldPassword);
            AddUserDirectly(user);

            // Act
            var passwordChanged = _service.ChangePassword(user.UserName, user.Password, "Pwd with witeSpace");

            // Assert
            passwordChanged.Should().BeFalse();
            UserStorage.Single().Password.Should().Be(oldPassword);
        }

        [Fact]
        public void ChangePassword_UserNotExists_ExceptionThrown()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();

            // Act
            var passwordChanged = _service.ChangePassword(user.UserName, user.Password, "newPassword");

            // Assert
            passwordChanged.Should().BeFalse();
        }

        [Theory]
        [PropertyData("UsernameInDifferentCase")]
        public void ChangePassword_Simple_UsernamesCaseIgnored(string username)
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            const string newPassword = "newPassword";

            // Act
            _service.ChangePassword(username, user.Password, newPassword);

            // Assert
            UserStorage.Single().Password.Should().Be(newPassword);
        }

        [Fact]
        public void UnlockUser_UserExists_UserUnlocked()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            user.IsLocked = true;
            AddUserDirectly(user);

            // Act
            _service.UnlockUser(user.UserName);

            // Assert
            user.IsLocked.Should().BeFalse();
        }

        [Fact]
        public void UnlockUser_UserNotExists_ExceptionThrown()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();

            // Act, Assert
            _service.Invoking(p => p.UnlockUser(user.UserName))
                .ShouldThrow<ArgumentException>();
        }

        [Theory]
        [PropertyData("UsernameInDifferentCase")]
        public void UnlockUser_Simple_UsernamesCaseIgnored(string username)
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            user.IsLocked = true;
            AddUserDirectly(user);

            // Act
            _service.UnlockUser(username);

            // Assert
            user.IsLocked.Should().BeFalse();
        }

        [Fact]
        public void GetUserById_UserExists_UserReturned()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            // Act
            var foundedUser = _service.GetUser(user.ID);

            // Assert
            foundedUser.Should().Be(user);
        }

        [Fact]
        public void GetUserById_UserNotExists_NullReturned()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();

            // Act
            var membershipUser = _service.GetUser(user.ID);

            // Assert
            membershipUser.Should().BeNull();
        }

        [Fact]
        public void ResetPassword_UserNotExists_ExceptionThrown()
        {
            // Arrange
            _service.RequiresQuestionAndAnswer = true;
            var user = UtilityMethods.CreateUser();

            // Act, Assert
            _service.Invoking(
                p => p.ResetPassword(user.UserName, user.PasswordAnswer))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void ResetPassword_UserLocked_ExceptionThrown()
        {
            // Arrange
            _service.RequiresQuestionAndAnswer = true;
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;

            // Act, Assert
            _service.Invoking(
                p => p.ResetPassword(user.UserName, user.PasswordAnswer))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void ResetPassword_RequieredPasswordAnswerInvalid_ExceptionThrown()
        {
            // Arrange
            _service.RequiresQuestionAndAnswer = true;
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            // Act, Assert
            _service.Invoking(
                p => p.ResetPassword(user.UserName, "invalidAnswer"))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void ResetPassword_ValidUserDataProvided_PasswordReseted()
        {
            // Arrange
            _service.NewPasswordLength = 8;
            const string initialPassword = "password";
            var user = UtilityMethods.CreateUser(password: initialPassword);
            AddUserDirectly(user);

            // Act
            var newPassword = _service.ResetPassword(user.UserName, user.PasswordAnswer);

            // Assert
            var password = UserStorage.Single().Password;
            password.Should().Be(newPassword);
            password.Should().NotBe(initialPassword);
        }
        
        [Theory]
        [PropertyData("UsernameInDifferentCase")]
        public void ResetPassword_Simple_UsernamesCaseIgnored(string username)
        {
            // Arrange
            _service.NewPasswordLength = 8;
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            // Act
            _service.ResetPassword(username, user.PasswordAnswer);

            // Assert
            user.Password.Should().NotBe("password");
        }

        [Fact]
        public void GetUserByUsername_UserNotExists_NullReturned()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();

            // Act
            var foundedUser = _service.GetUser(user.UserName);

            // Assert
            foundedUser.Should().BeNull();
        }

       [Fact]
        public void GetUserByUsername_UserExists_UserReturned()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            // Act
            var foundedUser = _service.GetUser(user.UserName);

            // Assert
            foundedUser.Should().Be(user);
        }

        [Theory]
        [PropertyData("UsernameInDifferentCase")]
        public void GetUserByUsername_Simple_UsernamesCaseIgnored(string username)
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            // Act
            var foundedUser = _service.GetUser(username);

            // Assert
            foundedUser.Should().Be(user);
        }

        [Fact]
        public void GetUserByEmail_UserNotExists_NullReturned()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();

            // Act
            var foundedUser = _service.GetUserByEmail(user.Email);

            // Assert
            foundedUser.Should().BeNull();
        }

        [Fact]
        public void GetUserByEmail_UserExists_UserReturned()
        {
            // Arrange
            var user = UtilityMethods.CreateUser(email: "a@b.c");
            AddUserDirectly(user);

            // Act
            var foundedUser = _service.GetUserByEmail("a@b.c");

            // Assert
            foundedUser.Should().Be(user);
        }

        [Theory]
        [InlineData("a@b.c")]
        [InlineData("A@B.C")]
        public void GetUserByEmail_Simple_EmailsCaseIgnored(string email)
        {
            // Arrange
            var user = UtilityMethods.CreateUser(email: "a@b.c");
            AddUserDirectly(user);

            // Act
            var foundedUser = _service.GetUserByEmail(email);

            // Assert
            foundedUser.Should().Be(user);
        }


        [Fact]
        public void GetAllUsers_NoUsersExist_EmptyCollectionReturned()
        {
            // Arrange
            int totalRecords;

            // Act
            var users = _service.GetAllUsers(0, 1, out totalRecords);

            // Assert
            users.Should().HaveCount(0);
            totalRecords.Should().Be(0);
        }

        [Theory]
        [PropertyData("PagingTestData")]
        public void GetAllUsers_UsersExist_MaxUsersCountPerPageReturned(int pageSize,
                                                                        int pageIndex,
                                                                        int expectedFoundMemberships)
        {
            // Arrange
            int totalRecords;
            AddUsers(3, i => UtilityMethods.CreateUser("user" + i));

            // Act
            var users = _service.GetAllUsers(pageIndex,
                                             pageSize,
                                             out totalRecords);

            // Assert
            users.Should().HaveCount(expectedFoundMemberships);
        }

        [Fact]
        public void GetAllUsers_UsersExist_TotalRecordsCountReturned()
        {
            // Arrange
            int totalRecords;
            AddUsers(3, i => UtilityMethods.CreateUser());

            // Act
            _service.GetAllUsers(0, 1, out totalRecords);

            // Assert
            totalRecords.Should().Be(3);
        }

        [Fact]
        public void FindUsersByEmail_NoUsersExist_EmptyCollectionReturned()
        {
            // Arrange
            int totalRecords;

            // Act
            var users = _service.FindUsersByEmail("a@b.c", 0, 1, out totalRecords);

            // Assert
            users.Should().BeEmpty();
        }

        [Theory]
        [PropertyData("PagingTestData")]
        public void FindUsersByEmail_UsersExist_MaxUsersCountPerPageReturned(int pageSize,
                                                                             int pageIndex,
                                                                             int expectedFoundMemberships)
        {
            // Arrange
            int totalRecords;
            AddUsers(3, i => UtilityMethods.CreateUser(username: "user" + i, email: "a@b.c"));

            // Act
            var users = _service.FindUsersByEmail("a@b.c",
                                                  pageIndex,
                                                  pageSize,
                                                  out totalRecords);

            // Assert
            users.Should().HaveCount(expectedFoundMemberships);
        }

        [Fact]
        public void FindUsersByEmail_UsersExist_TotalRecordsCountEquelsMatchingUsersCount()
        {
            // Arrange
            int totalRecords;
            AddUsers(3, i => UtilityMethods.CreateUser(email: "a@b.c"));
            AddUsers(1, i => UtilityMethods.CreateUser(email: "b@b.c"));

            // Act
            _service.FindUsersByEmail("a@b.c", 0, 1, out totalRecords);

            // Assert
            totalRecords.Should().Be(3);
        }

        [Theory]
        [InlineData("a@B.C")]
        [InlineData("A@b.C")]
        public void FindUsersByEmail_Simple_EmailsCaseIgnored(string email)
        {
            // Arrange
            int totalRecords;
            AddUserDirectly(UtilityMethods.CreateUser(email: "a@b.c"));

            // Act
            _service.FindUsersByEmail(email, 0, 1, out totalRecords);

            // Assert
            totalRecords.Should().Be(1);
        }

        [Fact]
        public void ValidateUser_ValidPasswordProvided_UserValid()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            // Act
            var isValid = _service.ValidateUser(user.UserName, user.Password);

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void ValidateUser_UserIsLocked_UserNotValid()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);
            UserStorage.Single().IsLocked = true;

            // Act
            var isValid = _service.ValidateUser(user.UserName, user.Password);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateUser_UserDoesNotExists_UserNotValid()
        {
            // Act
            var isValid = _service.ValidateUser("username", "password");

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateUser_UserNotApproved_UserNotValid()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            user.IsApproved = false;
            AddUserDirectly(user);

            // Act
            var isValid = _service.ValidateUser(user.UserName, user.Password);

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void ValidateUser_InvalidPasswordProvided_UserNotValid()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            // Act
            var isValid = _service.ValidateUser(user.UserName, "invalidPassword");

            // Assert
            isValid.Should().BeFalse();
        }

        [Theory]
        [PropertyData("UsernameInDifferentCase")]
        public void ValidateUser_Simple_UsernamesCaseIgnored(string username)
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUserDirectly(user);

            // Act
            var isValid = _service.ValidateUser(username, user.Password);

            // Assert
            isValid.Should().BeTrue();
        }
    }
}