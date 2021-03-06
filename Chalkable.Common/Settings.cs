﻿using System;
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

        public static string MasterConnectionString => GetConnectionString("ChalkableMaster");
        public static string ChalkableSchoolDbUser => Get("ChalkableSchoolDbUser");
        public static string ChalkableSchoolDbPassword => Get("ChalkableSchoolDbPwd");
        public static string[] ChalkableSchoolDbServers => Get("ChalkableSchoolDbServers").Split(',');

        public static string GetSchoolConnectionString(string dbServer, string catalogName)
        {
            Debug.Assert(ChalkableSchoolDbServers.Contains(dbServer), $"School DB server {dbServer} should be included in ChalkableSchoolDbServers");
            return String.Format(GetSchoolConnectionString("ChalkableSchool"), dbServer, catalogName);
        }

        public static string GetSchoolConnectionString(string dbServer, Guid catalogName)
        {
            return GetSchoolConnectionString(dbServer, catalogName.ToString());
        }

        public static string GetSchoolTemplateConnectionString(string dbServer)
        {
            Debug.Assert(ChalkableSchoolDbServers.Contains(dbServer), $"School DB server {dbServer} should be included in ChalkableSchoolDbServers");
            return String.Format(GetSchoolConnectionString("ChalkableSchool"), dbServer, SchoolTemplateDbName);
        }

        public static string SchoolTemplateDbName => Get("ChalkableSchoolTemplateDbName");

        public static int DbUpdateTimeout => int.Parse(Get("DbUpdateTimeout"));

        /* Web settings */

        public static string Domain => Get("Domain");
        public static string ScriptsRoot => Get("ScriptsRoot");

        /* API Explorer */

        public static string ApiExplorerClientId => Get("api.explorer.client-id");
        public static string ApiExplorerSecret => Get("api.explorer.secret");
        public static string ApiExplorerRedirectUri => Get("api.explorer.redirecturi");
        public static string ApiExplorerClientName => Get("api.explorer.client-name");
        public static string ApiExplorerScope => Get("api.explorer.scope");

        /* Settings from WEB.config */

        public static string MixPanelToken => Get("mixpanel-token");

        /* STI */

        public static string StiApplicationKey => ConfigurationManager.AppSettings["sti.application.key"];

        /* Academic Benchmark */
        public static string AcademicBenchmarkApiUrl => ConfigurationManager.AppSettings["AcademicBenchmark.ApiUrl"];
        public static string AcademicBenchmarkPartnerId => ConfigurationManager.AppSettings["AcademicBenchmark.Partner.Id"];
        public static string AcademicBenchmarkPartnerKey => ConfigurationManager.AppSettings["AcademicBenchmark.Partner.Key"];
        public static string AcademicBenchmarkDbConnectionString => GetConnectionString("ChalkableAcademicBenchmark");


        /* BackgroundTasks settings */

        public static int TaskProcessorDelay => int.Parse(Get("TaskProcessorDelay"));
        public static string DbBackupServiceUrl => Get("DbBackupServiceUrl");
        public static int PictureProcessorCount => int.Parse(Get("PictureProcessorCount"));
        public static int AllSchoolRunnerDistrictPerThread => int.Parse(Get("AllSchoolRunner.DistrictsPerThread"));
        public static string InstrumentationKey => Get("instrumentationKey");

        public static ProducerConfigSection GetProducerConfig(string sectionName)
        {
            return new ProducerConfigSection(
                    long.Parse(Get("TaskProducer." + sectionName + ".Interval")),
                    long.Parse(Get("TaskProducer." + sectionName + ".IntervalStart")),
                    long.Parse(Get("TaskProducer." + sectionName + ".IntervalEnd"))
                );
        }

        /* Web Config */

        public static int MinPasswordLength => int.Parse(GetWebConfig("minPasswordLength"));

        public static string HomeRedirectUrl => Get("home-redirect-url");

        private static string GetWebConfig(string field)
        {
            return ConfigurationManager.AppSettings[field];
        }

        public static int WebClientTimeout => int.Parse(Get("WebClientTimeout"));

        /* Global Cache */

        public static string RedisCacheConnectionString => Get("RedisCache.ConnectionString");
        public static int RedisCacheConcurentConnections => int.Parse(Get("RedisCache.ConcurentConnections"));

        private static Verbosity configuredVerbosity;
        private static bool verbositySet = false;

        public static Verbosity Verbosity {
            get {
                if (!verbositySet) {
                    // set the default verbosity
                    configuredVerbosity = Verbosity.Off;

                    // Get the configured setting
                    var verbositySetting = RoleEnvironment.GetConfigurationSettingValue("verbosity");

                    if (!string.IsNullOrWhiteSpace(verbositySetting)) {
                        // Ensure the case of the config setting
                        verbositySetting = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(verbositySetting);
                    }

                    // TryParse will work with properly cased values or integers
                    Enum.TryParse(verbositySetting, out configuredVerbosity);

                    // Ensure that we only rertrieve this value once
                    verbositySet = true;
                }
                return configuredVerbosity;
            }
        }

        public static string RaygunJsApiKey => "WV05DNwmIzBvTiSQ8pgNXQ==";
        public static string MasterDbName => "ChalkableMaster";

        public static int DefaultMinWorkerThreads => int.Parse(Get("Threads.DefaultMinWorkerThreads"));
        public static int DefaultMinIoThreads => int.Parse(Get("Threads.DefaultMinIoThreads"));

        public static string AesSecretKey => Get("AesSecretKey");
    }
}
