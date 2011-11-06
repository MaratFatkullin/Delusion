using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using AI_.Security.DAL;
using AI_.Security.Models;

namespace AI_.Security.Services
{
    public class MembershipService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public MembershipService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public bool RequiresUniqueEmail { get; set; }


        private bool ValidatingPassword(string password, int minRequiredPasswordLength)
        {
            if (password.Length < minRequiredPasswordLength)
                return false;

            if (password.Contains(" "))
                return false;

            return true;
        }

        public User CreateUser(string username,
                               string password,
                               string email,
                               string passwordQuestion,
                               string passwordAnswer,
                               bool isApproved,
                               out MembershipCreateStatus status)
        {
            if (RequiresUniqueEmail && GetUserByEmail(email) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                var existingUser = GetUser(username);
                if (existingUser != null)
                {
                    status = MembershipCreateStatus.DuplicateUserName;
                    return null;
                }


                var user = new User
                           {
                               UserName = username.ToLower(),
                               Password = password,
                               Email = email,
                               IsApproved = isApproved,
                               PasswordQuestion = passwordQuestion,
                               PasswordAnswer = passwordAnswer,
                               LastPasswordChangedDate = DateTime.Now,
                               LastActivityDate = DateTime.Now,
                               LastLockoutDate = DateTime.MinValue.ToLocalTime(),
                               LastLoginDate = DateTime.MinValue.ToLocalTime(),
                           };

                try
                {
                    unitOfWork.UserRepository.Insert(user);
                }
                catch (Exception)
                {
                    status = MembershipCreateStatus.ProviderError;
                    return null;
                }

                status = MembershipCreateStatus.Success;
                return user;
            }
        }

        public User GetUserByEmail(string email)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                return unitOfWork.UserRepository
                    .Get(us => us.Email == email)
                    .SingleOrDefault();
            }
        }

        public bool ChangePasswordQuestionAndAnswer(string username,
                                                    string password,
                                                    string newPasswordQuestion,
                                                    string newPasswordAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                var user = GetUser(username, unitOfWork);

                user.PasswordQuestion = newPasswordQuestion;
                user.PasswordAnswer = newPasswordAnswer;

                unitOfWork.UserRepository.Update(user);
                return true;
            }
        }

        public bool ChangePassword(string username,
                                   string oldPassword,
                                   string newPassword,
                                   int minRequiredPasswordLength = 6)
        {
            if (newPassword == null)
                throw new ArgumentNullException("newPassword");
            if (oldPassword == null)
                throw new ArgumentNullException("newPassword");

            if (!ValidateUser(username, oldPassword))
                return false;

            var isValid = ValidatingPassword(newPassword, minRequiredPasswordLength);

            if (!isValid)
                return false;

            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                var user = unitOfWork.UserRepository
                    .Get(usr => string.Equals(usr.UserName,
                                              username,
                                              StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefault();
                if (user == null)
                    throw new ArgumentException("User with specified username does not exists.");

                user.Password = newPassword;
                return true;
            }
        }

        public string ResetPassword(string username,
                                    string answer,
                                    int newPasswordLength = 12,
                                    int minRequiredNonAlphanumericCharacters = 4,
                                    bool requiresQuestionAndAnswer = false)
        {
            var newPassword = Membership.GeneratePassword(newPasswordLength,
                                                          minRequiredNonAlphanumericCharacters);

            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                var user = GetUser(username, unitOfWork);
                if (user == null)
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }

                if (user.IsLocked)
                {
                    throw new MembershipPasswordException("The supplied user is locked out.");
                }

                if (requiresQuestionAndAnswer && answer != user.PasswordAnswer)
                {
                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                user.Password = newPassword;

                return newPassword;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                var user = GetUser(username, unitOfWork);
                if (user == null || user.IsLocked || !user.IsApproved)
                    return false;
                return ValidatePassword(user.Password, password);
            }
        }

        public bool UnlockUser(string userName)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                var user = GetUser(userName, unitOfWork);
                if (user == null)
                    throw new ArgumentException("The supplied user name is not found.");
                
                user.IsLocked = false;
                return true;
            }
        }

        public User GetUser(int id)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                return unitOfWork.UserRepository.GetByID(id);
            }
        }

        public User GetUser(string username)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                var user = GetUser(username, unitOfWork);
                return user;
            }
        }

        public List<User> GetAllUsers(int pageIndex,
                                      int pageSize,
                                      out int totalRecords)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                var users = unitOfWork.UserRepository.Get();
                totalRecords = users.Count();

                return users.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        public List<User> FindUsersByEmail(string emailToMatch,
                                           int pageIndex,
                                           int pageSize,
                                           out int totalRecords)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetInstance())
            {
                var users = unitOfWork.UserRepository.Get(usr => usr.Email.ToLower() == emailToMatch.ToLower());
                totalRecords = users.Count();

                return users.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
        }

        protected User GetUser(string username, ISecurityUnitOfWork unitOfWork)
        {
            return unitOfWork.UserRepository
                .Get(user => user.UserName == username.ToLower())
                .SingleOrDefault();
        }

        private static bool ValidatePassword(string storedPassword, string providedPassword)
        {
            return providedPassword == storedPassword;
        }
    }
}