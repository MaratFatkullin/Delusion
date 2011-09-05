using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;
using AI_.Security.DAL;
using AI_.Security.Models;

namespace AI_.Security.Providers
{
    public class CustomMembershipProvider : MembershipProvider
    {
        private readonly ISecurityUnitOfWork _unitOfWork;

        #region Configuration fields

        private bool _enablePasswordReset;
        private bool _enablePasswordRetrieval;
        private int _maxInvalidPasswordAttempts;
        private int _minRequiredNonAlphanumericCharacters;
        private int _minRequiredPasswordLength;
        private int _passwordAttemptWindow;
        private MembershipPasswordFormat _passwordFormat;
        private string _passwordStrengthRegularExpression;
        private bool _requiresQuestionAndAnswer;
        private bool _requiresUniqueEmail;
        private int _newPasswordLength;

        #endregion

        #region Properties

        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }

        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }

        public override string ApplicationName { get; set; }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }

        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqueEmail; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _passwordFormat; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        #endregion

        public CustomMembershipProvider(ISecurityUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public CustomMembershipProvider()
        {
            _unitOfWork = new SecurityUnitOfWork();
        }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            base.Initialize(name, config);

            //_applicationName = GetConfigValue(config["applicationName"],
            //                                System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(
                config["maxInvalidPasswordAttempts"], "5"));
            _passwordAttemptWindow = Convert.ToInt32(GetConfigValue(
                config["passwordAttemptWindow"], "10"));
            _minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(
                config["minRequiredNonAlphanumericCharacters"], "1"));
            _minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(
                config["minRequiredPasswordLength"], "7"));
            _passwordStrengthRegularExpression = Convert.ToString(GetConfigValue(
                config["passwordStrengthRegularExpression"], ""));
            _enablePasswordReset = Convert.ToBoolean(GetConfigValue(
                config["enablePasswordReset"], "false"));
            _enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(
                config["enablePasswordRetrieval"], "false"));
            _requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(
                config["requiresQuestionAndAnswer"], "false"));
            _requiresUniqueEmail = Convert.ToBoolean(GetConfigValue(
                config["requiresUniqueEmail"], "false"));
        }

        public override MembershipUser CreateUser(string username,
                                                  string password,
                                                  string email,
                                                  string passwordQuestion,
                                                  string passwordAnswer,
                                                  bool isApproved,
                                                  object providerUserKey,
                                                  out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != null)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser existingUser = GetUser(username, false);
            if (existingUser != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            DateTime createDate = DateTime.Now;

            if (providerUserKey == null)
            {
                providerUserKey = Guid.NewGuid();
            }
            else
            {
                if (!(providerUserKey is Guid))
                {
                    status = MembershipCreateStatus.InvalidProviderUserKey;
                    return null;
                }
            }

            var user = new User
                       {
                           UserName = username,
                           Email = email,
                           IsApproved = isApproved,
                           PasswordQuestion = passwordQuestion,
                           PasswordAnswer = passwordAnswer,
                           ProviderUserKey = providerUserKey,
                           CreateDate = createDate
                       };


            try
            {
                _unitOfWork.UserRepository.Insert(user);
            }
            catch (Exception)
            {
                status = MembershipCreateStatus.ProviderError;
                return null;
            }

            status = MembershipCreateStatus.Success;
            return GetUser(username, false);
        }

        public override bool ChangePasswordQuestionAndAnswer(string username,
                                                             string password,
                                                             string newPasswordQuestion,
                                                             string newPasswordAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            var user = _unitOfWork.UserRepository.Get(usr => usr.UserName == username).SingleOrDefault();

            user.PasswordQuestion = newPasswordQuestion;
            user.PasswordAnswer = newPasswordAnswer;

            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.Save();

            return true;
        }

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException("Password Retrieval Not Enabled.");
            }

            //todo: покрыть
            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot retrieve Hashed passwords.");
            }

            var user = _unitOfWork.UserRepository
                .Get(usr => usr.UserName == username)
                .FirstOrDefault();

            if (user == null)
            {
                throw new MembershipPasswordException("The supplied user name is not found.");
            }

            if (user.IsLocked)
            {
                throw new MembershipPasswordException("The supplied user is locked out.");
            }

            if (RequiresQuestionAndAnswer && answer == user.PasswordAnswer)
            {
                throw new MembershipPasswordException("Incorrect password answer.");
            }

            return user.Password;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
                return false;
            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException(
                        "Change password canceled due to new password validation failure.");
            var user = _unitOfWork.UserRepository.Get(usr => usr.UserName == username).Single();
            user.Password = newPassword;
            _unitOfWork.Save();
            return true;
        }

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            if (answer == null && RequiresQuestionAndAnswer)
            {
                throw new ProviderException("Password answer required for password reset.");
            }

            var newPassword = Membership.GeneratePassword(_newPasswordLength, MinRequiredNonAlphanumericCharacters);

            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

            var user = GetUser(username);
            if (user == null)
            {
                throw new MembershipPasswordException("The supplied user name is not found.");
            }

            if (user.IsLocked)
            {
                throw new MembershipPasswordException("The supplied user is locked out.");
            }

            if (RequiresQuestionAndAnswer && answer != user.PasswordAnswer)
            {
                throw new MembershipPasswordException("Incorrect password answer.");
            }

            user.Password = newPassword;
            _unitOfWork.Save();

            return newPassword;
        }

        public override void UpdateUser(MembershipUser user)
        {
            var existingUser = GetUser(user.UserName);
            if (existingUser == null)
            {
                throw new ProviderException("The supplied user name is not found.");
            }
            existingUser.Email = user.Email;
            existingUser.IsApproved = user.IsApproved;
            //todo:использовать автомэппер.

            _unitOfWork.UserRepository.Update(existingUser);
            _unitOfWork.Save();
        }

        public override bool ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            var user = GetUser(userName);
            if (user == null)
            {
                throw new ProviderException("The supplied user name is not found.");
            }

            if (!user.IsLocked)
            {
                throw new ProviderException("The supplied user is not locked out.");
            }
            user.IsLocked = false;
            return true;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            var user = _unitOfWork.UserRepository.
                Get(usr => usr.ProviderUserKey == providerUserKey).SingleOrDefault();

            //todo: использовать автомаппер.
            return null;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex,
                                                             int pageSize,
                                                             out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            //note: пока не трогаем.
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch,
                                                                 int pageIndex,
                                                                 int pageSize,
                                                                 out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch,
                                                                  int pageIndex,
                                                                  int pageSize,
                                                                  out int totalRecords)
        {
            throw new NotImplementedException();
        }

        protected User GetUser(string username)
        {
            return _unitOfWork.UserRepository.Get(user => user.UserName == username).SingleOrDefault();
        }
    }
}