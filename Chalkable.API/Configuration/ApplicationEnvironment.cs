using System.Configuration;

namespace Chalkable.API.Configuration
{
    public class ApplicationEnvironment : ConfigurationSection
    {
        [ConfigurationProperty("env")]
        public string Environment => (string)this["env"];

        [ConfigurationProperty("appSecret")]
        public string AppSecret => (string)this["appSecret"];

        [ConfigurationProperty("redirectUri")]
        public string RedirectUri => (string)this["redirectUri"];
        
        public string GetByKey(string key)
        {
            return (string) this[key];
        }      

        public string ApplicationRoot => RedirectUri;

        public string ConnectionString { get;  set; }
    }
}