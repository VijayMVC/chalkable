using System.Configuration;

namespace Chalkable.API.Configuration
{
    public class ApplicationEnvironment : ConfigurationSection
    {
        [ConfigurationProperty("env")]
        public string Environment => (string)this["env"];

        [ConfigurationProperty("appSecret")]
        public string AppSecret => (string)this["appSecret"];

        [ConfigurationProperty("acsUri")]
        public string AcsUri => (string)this["acsUri"];

        [ConfigurationProperty("redirectUri")]
        public string RedirectUri => (string)this["redirectUri"];

        [ConfigurationProperty("client_id")]
        public string ClientId => (string)this["client_id"];

        [ConfigurationProperty("scope")]
        public string Scope => (string)this["scope"];

        public string GetByKey(string key)
        {
            return (string) this[key];
        }      

        public string ApplicationRoot => RedirectUri;

        public string ConnectionString { get;  set; }
    }
}