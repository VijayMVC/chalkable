using System.Configuration;

namespace Chalkable.API.Configuration
{
    public class ApplicationConfigurations : ConfigurationSection
    {
        [ConfigurationProperty("environments")]
        public ApplicationEnvironments Environments => base["environments"] as ApplicationEnvironments;
    }
}