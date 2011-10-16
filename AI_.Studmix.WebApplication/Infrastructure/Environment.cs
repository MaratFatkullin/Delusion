using System.Configuration;

namespace AI_.Studmix.WebApplication.Infrastructure
{
    public static class Environment
    {
        public static string FileStoragePath
        {
            get { return ConfigurationManager.AppSettings["FileStoragePath"]; }
        }
    }
}