using System.IO;
using Microsoft.Extensions.Configuration;

namespace Dishes.Services
{
    public static class AppSettingsService
    {
        private static AppSettings _instance;
        private static readonly object Padlock = new object();

        public static AppSettings Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                        var configuration = builder.Build();

                        _instance = new AppSettings(configuration);
                    }
                    return _instance;
                }
            }
        }

        public class AppSettings
        {
            private const string DefaultDbFileName = "dishes.db";
            private const string DbFileNameKey = "dbFileName";

            public AppSettings(IConfiguration configuration)
            {
                ConnectionString = configuration.GetSection(DbFileNameKey).Value != null
                    ? configuration.GetSection(DbFileNameKey).Value
                    : DefaultDbFileName;
            }

            public string ConnectionString { get; }
        }
    }
}