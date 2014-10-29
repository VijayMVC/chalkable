using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Chalkable.Common
{
    public static class Settings
    {
        public class ProducerConfigSection
        {
            public long Interval { get; private set; }
            public long IntervalStart { get; private set; }
            public long IntervalEnd { get; private set; }

            public ProducerConfigSection(long interval, long intervalStart, long intervalEnd)
            {
                Interval = interval;
                IntervalStart = intervalStart;
                IntervalEnd = intervalEnd;
            }
        }

        private static string Get(string key)
        {
            Debug.Assert(RoleEnvironment.IsAvailable, "RoleEnvironment is unavailable");

            return RoleEnvironment.GetConfigurationSettingValue(key);
        }

        private static string GetConnectionString(string key)
        {
            Debug.Assert(RoleEnvironment.IsAvailable, "RoleEnvironment is unavailable");

            return RoleEnvironment.GetConfigurationSettingValue(key);
        }

        private static string GetSchoolConnectionString(string key)
        {
            return String.Format(GetConnectionString(key), "{0}", "{1}", ChalkableSchoolDbUser, ChalkableSchoolDbPassword);
        }

        /* Web DB settings */

        public static string MasterConnectionString { get { return GetConnectionString("ChalkableMaster"); } }
        public static string ChalkableSchoolDbUser { get { return Get("ChalkableSchoolDbUser"); } }
        public static string ChalkableSchoolDbPassword { get { return Get("ChalkableSchoolDbPwd"); } }
        public static string[] ChalkableSchoolDbServers { get { return Get("ChalkableSchoolDbServers").Split(','); } }

        public static string GetSchoolConnectionString(string dbServer, string catalogName)
        {
            Debug.Assert(ChalkableSchoolDbServers.Contains(dbServer), string.Format("School DB server {0} should be included in ChalkableSchoolDbServers", dbServer));
            return String.Format(GetSchoolConnectionString("ChalkableSchool"), dbServer, catalogName);
        }

        public static string GetSchoolConnectionString(string dbServer, Guid catalogName)
        {
            return GetSchoolConnectionString(dbServer, catalogName.ToString());
        }

        public static string GetSchoolTemplateConnectionString(string dbServer)
        {
            Debug.Assert(ChalkableSchoolDbServers.Contains(dbServer), string.Format("School DB server {0} should be included in ChalkableSchoolDbServers", dbServer));
            return String.Format(GetSchoolConnectionString("ChalkableSchool"), dbServer, SchoolTemplateDbName);
        }

        public static string SchoolTemplateDbName
        {
            get { return Get("ChalkableSchoolTemplateDbName"); }
        }

        /* Web settings */

        public static string Domain { get { return Get("Domain"); } }
        public static string ScriptsRoot { get { return Get("ScriptsRoot"); } }

        /* API Explorer */

        public static string ApiExplorerClientId { get { return Get("api.explorer.client-id"); } }
        public static string ApiExplorerSecret { get { return Get("api.explorer.secret"); } }
        public static string ApiExplorerRedirectUri { get { return Get("api.explorer.redirecturi"); } }
        public static string ApiExplorerClientName { get { return Get("api.explorer.client-name"); } }
        public static string ApiExplorerScope { get { return Get("api.explorer.scope"); } }

        /* Settings from WEB.config */

        public static string MixPanelToken { get { return Get("mixpanel-token"); } }

        /* WindowsAzure.OAuth */

        public static string WindowsAzureOAuthSwtSigningKey { get { return Get("WindowsAzure.OAuth.SwtSigningKey"); } }
        public static string WindowsAzureOAuthRelyingPartyRealm { get { return Get("WindowsAzure.OAuth.RelyingPartyRealm"); } }
        public static string WindowsAzureOAuthRelyingPartyName { get { return Get("WindowsAzure.OAuth.RelyingPartyName"); } }
        public static string WindowsAzureOAuthServiceNamespace { get { return Get("WindowsAzure.OAuth.ServiceNamespace"); } }
        public static string WindowsAzureOAuthServiceNamespaceManagementUserName { get { return Get("WindowsAzure.OAuth.ServiceNamespaceManagementUserName"); } }
        public static string WindowsAzureOAuthServiceNamespaceManagementUserKey { get { return Get("WindowsAzure.OAuth.ServiceNamespaceManagementUserKey"); } }

        /* STI */

        public static string StiApplicationKey { get { return ConfigurationManager.AppSettings["sti.application.key"]; } }

        /* BackgroundTasks settings */

        public static int TaskProcessorDelay { get { return Int32.Parse(Get("TaskProcessorDelay")); } }
        public static string DbBackupServiceUrl { get { return Get("DbBackupServiceUrl"); } }

        public static ProducerConfigSection GetProducerConfig(string sectionName)
        {
            return new ProducerConfigSection(
                    long.Parse(Get("TaskProducer." + sectionName + ".Interval")),
                    long.Parse(Get("TaskProducer." + sectionName + ".IntervalStart")),
                    long.Parse(Get("TaskProducer." + sectionName + ".IntervalEnd"))
                );
        }
    }
}
