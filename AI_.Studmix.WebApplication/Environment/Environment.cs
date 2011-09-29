using System.Configuration;

namespace AI_.Studmix.WebApplication.Environment
{
    public static class Environment
    {
        public static string FileStoragePath
        {
            get { return ConfigurationManager.AppSettings["FileStoragePath"]; }
        }
    }
}