using System.Collections.Generic;
using System.Web.Security;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.ApplicationServices.Services.Abstractions
{
    public interface IMembershipService
    {
        bool RequiresUniqueEmail { get; set; }
        int MinRequiredPasswordLength { get; set; }
        int MinRequiredNonAlphanumericCharacters { get; set; }
        int NewPasswordLength { get; set; }
        bool RequiresQuestionAndAnswer { get; set; }
        bool RequiresEmail { get; set; }
       
        UserPrinciple GetUserByEmail(string email);

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

        List<UserPrinciple> FindUsersByEmail(string emailToMatch,
                                    int pageIndex,
                                    int pageSize,
                                    out int totalRecords);

        User GetUser(string username);

        User CreateUser(string username,
                                        string password,
                                        string email,
                                        string phoneNumber,
                                        string passwordQuestion,
                                        string passwordAnswer,
                                        bool isApproved,
                                        out MembershipCreateStatus status);
    }
}