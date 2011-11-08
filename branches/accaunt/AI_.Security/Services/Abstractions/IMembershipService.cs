using System.Collections.Generic;
using System.Web.Security;
using AI_.Security.Models;

namespace AI_.Security.Services.Abstractions
{
    public interface IMembershipService
    {
        bool RequiresUniqueEmail { get; set; }
        int MinRequiredPasswordLength { get; set; }
        int MinRequiredNonAlphanumericCharacters { get; set; }
        int NewPasswordLength { get; set; }
        bool RequiresQuestionAndAnswer { get; set; }
        bool RequiresEmail { get; set; }

        User CreateUser(string username,
                        string password,
                        string email,
                        string passwordQuestion,
                        string passwordAnswer,
                        bool isApproved,
                        out MembershipCreateStatus status);

        User GetUserByEmail(string email);

        bool ChangePasswordQuestionAndAnswer(string username,
                                             string password,
                                             string newPasswordQuestion,
                                             string newPasswordAnswer);

        bool ChangePassword(string username,
                            string oldPassword,
                            string newPassword);

        string ResetPassword(string username, string answer);
        bool ValidateUser(string username, string password);
        bool UnlockUser(string userName);
        User GetUser(int id);

        List<User> GetAllUsers(int pageIndex,
                               int pageSize,
                               out int totalRecords);

        List<User> FindUsersByEmail(string emailToMatch,
                                    int pageIndex,
                                    int pageSize,
                                    out int totalRecords);

        User GetUser(string username);
    }
}