using System.Configuration;

namespace Chalkable.Common
{
    public static class Settings
    {

        public static string MasterConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ChalkableMaster"].ConnectionString;
                return connectionString;
            }
        }

        public static string SchoolConnectionStringTemplate
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ChalkableSchool"].ConnectionString;
                return connectionString;
            }
        }

        public static string SchoolTemplateConnectionString
        {
            get
            {
                var connectionString = ConfigurationManager.ConnectionStrings["ChalkableSchoolTemplate"].ConnectionString;
                return connectionString;
            }
        }

        private const string APPLICATION_CONFIG = "ChalkableConfiguration";
        static Settings()
        {
            var section = ConfigurationManager.GetSection(APPLICATION_CONFIG);
            configuration = (ApplicationConfiguration)section;
            Servers = new string[configuration.Servers.Count];
            int i = 0;
            foreach (var server in configuration.Servers)
            {
                Servers[i] = server.ToString();
                i++;
            }
        }

        private static ApplicationConfiguration configuration;
        public static ApplicationConfiguration Configuration
        {
            get { return configuration; }
        }

        public static string[] Servers { get; private set; }
    }

    public class ApplicationConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("SchoolTemplateDataBase")]
        public string SchoolTemplateDataBase
        {
            get { return (string)this["SchoolTemplateDataBase"]; }
        }

        [ConfigurationProperty("Servers")]
        public KeyValueConfigurationCollection Servers
        {
            get { return (KeyValueConfigurationCollection)this["Servers"]; }
        }
    }

}
