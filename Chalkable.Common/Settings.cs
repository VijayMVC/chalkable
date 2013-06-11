using System.Collections.Generic;
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

        private const string APPLICATION_CONFIG = "ChalkableConfiguration";
        static Settings()
        {
            var section = ConfigurationManager.GetSection(APPLICATION_CONFIG);
            configuration = section as ApplicationConfiguration;
        }

        private static ApplicationConfiguration configuration;
        public static ApplicationConfiguration Configuration
        {
            get { return configuration; }
        }

        private static List<string> servers;
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
