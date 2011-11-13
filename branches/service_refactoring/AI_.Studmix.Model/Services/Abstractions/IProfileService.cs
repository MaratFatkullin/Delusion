using AI_.Security.Models;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.Services.Abstractions
{
    public interface IProfileService
    {
        UserProfile GetUserProfile(User user);
        UserProfile GetUserProfile(int user);
        void CreateUserProfile(User user, string phoneNumber);
        void Save();
    }
}