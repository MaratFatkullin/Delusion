﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using AI_.Data.Repository;
using AI_.Security.Models;
using AI_.Security.Services.Abstractions;

namespace AI_.Security.Services
{
    public class MembershipService : ServiceBase, IMembershipService
    {
        public bool RequiresUniqueEmail { get; set; }
        public int MinRequiredPasswordLength { get; set; }
        public int MinRequiredNonAlphanumericCharacters { get; set; }
        public int NewPasswordLength { get; set; }
        public bool RequiresQuestionAndAnswer { get; set; }
        public bool RequiresEmail { get; set; }

        public MembershipService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


        private bool ValidatingPassword(string password)
        {
            if (password.Length < MinRequiredPasswordLength)
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
            if (RequiresEmail)
            {
                if (string.IsNullOrEmpty(email))
                {
                    status = MembershipCreateStatus.InvalidEmail;
                    return null;
                }
            }

            if (RequiresUniqueEmail && !string.IsNullOrEmpty(email) && GetUserByEmail(email) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            if (RequiresQuestionAndAnswer)
            {
                if (string.IsNullOrEmpty(passwordQuestion))
                {
                    status = MembershipCreateStatus.InvalidQuestion;
                    return null;
                }
                if (string.IsNullOrEmpty(passwordAnswer))
                {
                    status = MembershipCreateStatus.InvalidAnswer;
                    return null;
                }
            }

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
                           Email = email.ToLower(),
                           IsApproved = isApproved,
                           PasswordQuestion = passwordQuestion,
                           PasswordAnswer = passwordAnswer,
                           LastPasswordChangedDate = DateTime.Now,
                           LastActivityDate = DateTime.Now,
                           LastLockoutDate = null,
                           LastLoginDate = null,
                       };

            UnitOfWork.GetRepository<User>().Insert(user);
            UnitOfWork.Save();

            status = MembershipCreateStatus.Success;
            return user;
        }

        public User GetUserByEmail(string email)
        {
            return UnitOfWork.GetRepository<User>()
                .Get(us => us.Email == email.ToLower())
                .SingleOrDefault();
        }

        public bool ChangePasswordQuestionAndAnswer(string username,
                                                    string password,
                                                    string newPasswordQuestion,
                                                    string newPasswordAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            var user = GetUser(username);

            user.PasswordQuestion = newPasswordQuestion;
            user.PasswordAnswer = newPasswordAnswer;

            UnitOfWork.GetRepository<User>().Update(user);
            UnitOfWork.Save();
            return true;
        }

        public bool ChangePassword(string username,
                                   string oldPassword,
                                   string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
                return false;

            if (!ValidatingPassword(newPassword))
                return false;

            var user = GetUser(username);
            if (user == null)
                throw new ArgumentException("User with specified username does not exists.");

            user.Password = newPassword;

            UnitOfWork.Save();
            return true;
        }

        public string ResetPassword(string username, string answer)
        {
            var user = GetUser(username);
            if (user == null)
            {
                throw new ArgumentException("The supplied user name is not found.");
            }

            if (user.IsLocked)
            {
                throw new InvalidOperationException("The supplied user is locked out.");
            }

            if (RequiresQuestionAndAnswer && answer != user.PasswordAnswer)
            {
                throw new ArgumentException("Incorrect password answer.");
            }

            user.Password = Membership.GeneratePassword(NewPasswordLength,
                                                        MinRequiredNonAlphanumericCharacters);

            UnitOfWork.Save();
            return user.Password;
        }

        public bool ValidateUser(string username, string password)
        {
            var user = GetUser(username);
            if (user == null || user.IsLocked || !user.IsApproved)
                return false;
            return ValidatePassword(user.Password, password);
        }

        public bool UnlockUser(string userName)
        {
            var user = GetUser(userName);
            if (user == null)
                throw new ArgumentException("The supplied user name is not found.");

            user.IsLocked = false;

            UnitOfWork.Save();
            return true;
        }

        public User GetUser(int id)
        {
            return UnitOfWork.GetRepository<User>().GetByID(id);
        }

        public List<User> GetAllUsers(int pageIndex,
                                      int pageSize,
                                      out int totalRecords)
        {
            var users = UnitOfWork.GetRepository<User>().Get();
            totalRecords = users.Count;

            return users.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public List<User> FindUsersByEmail(string emailToMatch,
                                           int pageIndex,
                                           int pageSize,
                                           out int totalRecords)
        {
            var users = UnitOfWork.GetRepository<User>()
                .Get(usr => usr.Email == emailToMatch.ToLower());
            totalRecords = users.Count;

            return users.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        public User GetUser(string username)
        {
            return UnitOfWork.GetRepository<User>()
                .Get(user => user.UserName == username.ToLower())
                .SingleOrDefault();
        }

        private static bool ValidatePassword(string storedPassword, string providedPassword)
        {
            return providedPassword == storedPassword;
        }
    }
}