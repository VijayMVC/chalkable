using System;
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
                return string.Format(connectionString, "{0}", "{1}", configuration.SchoolDbUser, configuration.SchoolDbPassword);
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
            foreach (var key in configuration.Servers.AllKeys)
            {
                Servers[i] = configuration.Servers[key].Value;
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

        [Obsolete]
        [ConfigurationProperty("EmptySchoolsReserved")]
        public int EmptySchoolsReserved
        {
            get { return (int)this["EmptySchoolsReserved"]; }
        }

        [Obsolete]
        [ConfigurationProperty("DemoSchoolsReserved")]
        public int DemoSchoolsReserved
        {
            get { return (int)this["DemoSchoolsReserved"]; }
        }

        [ConfigurationProperty("SchoolDbUser")]
        public string SchoolDbUser
        {
            get { return (string)this["SchoolDbUser"]; }
        }
        
        [ConfigurationProperty("SchoolDbPassword")]
        public string SchoolDbPassword
        {
            get { return (string)this["SchoolDbPassword"]; }
        }


        [ConfigurationProperty("TaskProcessorDelay")]
        public int TaskProcessorDelay
        {
            get { return (int) this["TaskProcessorDelay"]; }
        }
    }

}
