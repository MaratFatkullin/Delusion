namespace AI_.Studmix.WebApplication.Infrastructure.Authentication
{
    public interface IAuthenticationProvider
    {
        void LogOn(string userName, bool createPersistentCookie);
        void LogOut();
    }
}