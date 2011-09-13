using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;
using AI_.Security.DAL;
using AI_.Security.Models;
using AutoMapper;

namespace AI_.Security.Providers
{
    public class CustomMembershipProvider : MembershipProvider
    {
        private readonly IUnitOfWorkFactory _factory;

        #region Configuration fields

        private int _maxInvalidPasswordAttempts;
        private int _minRequiredNonAlphanumericCharacters;
        private int _minRequiredPasswordLength;
        private int _newPasswordLength;
        private int _passwordAttemptWindow;
        private bool _enablePasswordReset;
        private bool _enablePasswordRetrieval;
        private bool _requiresQuestionAndAnswer;
        private bool _requiresUniqueEmail;
        private string _passwordStrengthRegularExpression;
        private MembershipPasswordFormat _passwordFormat;

        #endregion

        #region Configuration Properties

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

        public CustomMembershipProvider()
            :this(new UnitOfWorkFactory())
        {
        }

        public CustomMembershipProvider(IUnitOfWorkFactory factory)
        {
            _factory = factory;
            ConfigureMapping();
            ValidatingPassword += CustomMembershipProvider_ValidatingPassword;
        }

       
        private void CustomMembershipProvider_ValidatingPassword(object sender, ValidatePasswordEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (e.Password.Length < MinRequiredPasswordLength)
            {
                e.Cancel = true;
                e.FailureInformation = 
                    new MembershipPasswordException("Password is too short. Minimal lenght is 8 symbols");
            }

            if (e.Password.Contains(" "))
            {
                e.Cancel = true;
                e.FailureInformation =
                    new MembershipPasswordException("Password cannot contain white spaces.");
            }
        }

        private void ConfigureMapping()
        {
            Mapper.CreateMap<User, MembershipUser>()
                .ConstructUsing(usr => new MembershipUser(Name,
                                                          usr.UserName,
                                                          usr.ID,
                                                          usr.Email,
                                                          usr.PasswordQuestion,
                                                          string.Empty,
                                                          usr.IsApproved,
                                                          usr.IsLocked,
                                                          usr.CreateDate,
                                                          usr.LastLoginDate,
                                                          usr.LastActivityDate,
                                                          usr.LastPasswordChangedDate,
                                                          usr.LastLockoutDate));
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

            Configure(config);
        }

        public void Configure(NameValueCollection config)
        {
            //_applicationName = GetConfigValue(config["applicationName"],
            //                                System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(
                config["maxInvalidPasswordAttempts"], "5"));
            _passwordAttemptWindow = Convert.ToInt32(GetConfigValue(
                config["passwordAttemptWindow"], "10"));
            _minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(
                config["minRequiredNonAlphanumericCharacters"], "1"));
            _minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(
                config["minRequiredPasswordLength"], "8"));
            _newPasswordLength = Convert.ToInt32(GetConfigValue(
                config["newPasswordLength"], "12"));
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

            Enum.TryParse(config["passwordFormat"], true, out _passwordFormat);
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

            using (var unitOfWork = _factory.GetInstance())
            {
                MembershipUser existingUser = GetUser(username, false);
                if (existingUser != null)
                {
                    status = MembershipCreateStatus.DuplicateUserName;
                    return null;
                }


                var user = new User
                           {
                               UserName = username,
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
                return GetUser(username, false);
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username,
                                                             string password,
                                                             string newPasswordQuestion,
                                                             string newPasswordAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            using (var unitOfWork = _factory.GetInstance())
            {
                var user = GetUser(username,unitOfWork);

                user.PasswordQuestion = newPasswordQuestion;
                user.PasswordAnswer = newPasswordAnswer;

                unitOfWork.UserRepository.Update(user);
                unitOfWork.Save();

                return true;
            }
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

            using (var unitOfWork = _factory.GetInstance())
            {
                var user = unitOfWork.UserRepository
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
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (newPassword == null)
                throw new ArgumentNullException("newPassword");
            if (oldPassword == null)
                throw new ArgumentNullException("newPassword");

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

            using (var unitOfWork = _factory.GetInstance())
            {
                var user = unitOfWork.UserRepository
                    .Get(usr => usr.UserName == username).Single();
                user.Password = newPassword;
                return true;
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            var newPassword = Membership.GeneratePassword(_newPasswordLength,
                                                          MinRequiredNonAlphanumericCharacters);

            var args = new ValidatePasswordEventArgs(username, newPassword, false);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException(
                        "Reset password canceled due to password validation failure.");

            using (var unitOfWork = _factory.GetInstance())
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

                if (RequiresQuestionAndAnswer && answer != user.PasswordAnswer)
                {
                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                user.Password = newPassword;
                unitOfWork.Save();

                return newPassword;
            }
        }

        public override void UpdateUser(MembershipUser user)
        {
            using (var unitOfWork = _factory.GetInstance())
            {
                var existingUser = GetUser(user.UserName, unitOfWork);
                if (existingUser == null)
                {
                    throw new ProviderException("The supplied user name is not found.");
                }
                existingUser.Email = user.Email;
                existingUser.IsApproved = user.IsApproved;
                //todo:использовать автомэппер.

                unitOfWork.UserRepository.Update(existingUser);
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            using (var unitOfWork = _factory.GetInstance())
            {
                var user = GetUser(username, unitOfWork);
                if (user == null || user.IsLocked || !user.IsApproved)
                    return false;
                return ValidatePassword(user.Password, password);
            }
        }

        public override bool UnlockUser(string userName)
        {
            using (var unitOfWork = _factory.GetInstance())
            {
                var user = GetUser(userName, unitOfWork);
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
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            using (var unitOfWork = _factory.GetInstance())
            {
                var user = unitOfWork.UserRepository
                    .Get(usr => usr.ID == (int) providerUserKey).SingleOrDefault();

                if (user == null)
                    return null;
                return Mapper.Map<User, MembershipUser>(user);
            }
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (var unitOfWork = _factory.GetInstance())
            {
                var user = GetUser(username, unitOfWork);
                if (user == null)
                    return null;
                return Mapper.Map<User, MembershipUser>(user);
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            using (var unitOfWork = _factory.GetInstance())
            {
                var user = unitOfWork.UserRepository.Get(usr => usr.Email == email).FirstOrDefault();
                if (user == null)
                    return null;
                return user.UserName;
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            using (var unitOfWork = _factory.GetInstance())
            {
                var user = GetUser(username, unitOfWork);
                if (user == null)
                {
                    return false;
                }
                unitOfWork.UserRepository.Delete(user);
                return true;
            }
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex,
                                                             int pageSize,
                                                             out int totalRecords)
        {
            using (var unitOfWork = _factory.GetInstance())
            {
                var users = unitOfWork.UserRepository.Get();
                totalRecords = users.Count();

                var membershipUsers = new MembershipUserCollection();
                var usersInPage = users.Skip(pageIndex*pageSize).Take(pageSize);
                foreach (var user in usersInPage)
                {
                    var membershipUser = Mapper.Map<User, MembershipUser>(user);
                    membershipUsers.Add(membershipUser);
                }
                return membershipUsers;
            }
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
            throw new NotSupportedException("Searching users by name is not supported");
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch,
                                                                  int pageIndex,
                                                                  int pageSize,
                                                                  out int totalRecords)
        {
            using (var unitOfWork = _factory.GetInstance())
            {
                var users = unitOfWork.UserRepository.Get(usr => usr.Email == emailToMatch);
                totalRecords = users.Count();

                var membershipUsers = new MembershipUserCollection();
                var usersInPage = users.Skip(pageIndex*pageSize).Take(pageSize);
                foreach (var user in usersInPage)
                {
                    var membershipUser = Mapper.Map<User, MembershipUser>(user);
                    membershipUsers.Add(membershipUser);
                }
                return membershipUsers;
            }
        }

        protected User GetUser(string username,ISecurityUnitOfWork unitOfWork)
        {
            return unitOfWork.UserRepository
                .Get(user => user.UserName == username).SingleOrDefault();
        }

        private static bool ValidatePassword(string storedPassword, string providedPassword)
        {
            return providedPassword == storedPassword;
        }
    }
}