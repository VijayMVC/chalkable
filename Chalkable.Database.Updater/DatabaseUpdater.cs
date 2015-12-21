using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Microsoft.Azure.SqlDatabase.Jobs.Client;

namespace Chalkable.Database.Updater
{
    public class DatabaseUpdater
    {
        private static void Main(string[] args)
        {
            // Server=tcp:edjb0d1a0ab363747abbc2ee.database.windows.net,1433;Database=edjb0d1a0ab363747abbc2ee;User ID=chalkadmin@edjb0d1a0ab363747abbc2ee;Password={your_password_here};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
            var serverName = "edjb0d1a0ab363747abbc2ee.database.windows.net";
            var databaseName = "edjb0d1a0ab363747abbc2ee";
            var userName = "chalkadmin@edjb0d1a0ab363747abbc2ee";
            var password = "Hellowebapps1!";
            var dacPacName = "Chalkable.Database.School.1.0.0.2267";
            var dacPacUri = "https://chalkablestat.blob.core.windows.net/artifacts/0-7-bacc5da8d5f5-2267/Chalkable.Database.School.dacpac";

            Task.Run(() => DeploySchoolDacPac(serverName, databaseName, userName, password, dacPacName, dacPacUri));
        }

        public static async Task<object> DeploySchoolDacPac(string serverName, string databaseName, string userName, string password, string dacPacName, string dacPacUri)
        {
            var securePassword = new SecureString();
            foreach (var ch in password)
            {
                securePassword.AppendChar(ch);
            }

            securePassword.MakeReadOnly();

            var connection = new AzureSqlJobConnection(serverName, databaseName, userName, securePassword);

            var elasticJobs = new AzureSqlJobClient(connection);

            var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();

            var targets = new HashSet<DatabaseTarget>();

            Settings.ChalkableSchoolDbServers.ToList().ForEach(_ =>
            {
                targets.Add(new DatabaseTarget(_, Settings.SchoolTemplateDbName));
            });

            serviceLocator.DistrictService.GetDistricts().ForEach(_ =>
            {
                targets.Add(new DatabaseTarget(_.ServerUrl, _.Name));
            });

            var dacPackDef = await elasticJobs.Content.GetContentAsync(dacPacName) ??
                             await elasticJobs.Content.CreateDacpacAsync(dacPacName, new Uri(dacPacUri));

            var target = await elasticJobs.Targets.CreateCustomCollectionTargetAsync("Targets for DACPAC " + dacPacName + " " + Guid.NewGuid());

            await Task.WhenAll(targets.Select(async _ =>
            {
                var dbTarget = await elasticJobs.Targets.GetDatabaseTargetAsync(_.Server, _.Name) ??
                               await elasticJobs.Targets.CreateDatabaseTargetAsync(_.Server, _.Name);
                return await elasticJobs.Targets.AddChildTargetAsync(target.TargetId, dbTarget.TargetId);
            }).ToArray());

            var securePassword1 = new SecureString();
            foreach (var ch1 in Settings.ChalkableSchoolDbPassword)
            {
                securePassword1.AppendChar(ch1);
            }

            securePassword1.MakeReadOnly();

            var credential = await elasticJobs.Credentials.CreateCredentialAsync(Guid.NewGuid().ToString(), Settings.ChalkableSchoolDbUser, securePassword1);

            var job = await elasticJobs.Jobs.CreateJobAsync("Apply DACPAC " + dacPacName + " " + Guid.NewGuid(), new JobBuilder
            {
                ContentName = dacPackDef.ContentName,
                TargetId = target.TargetId,
                CredentialName = credential.CredentialName
            });

            return await elasticJobs.JobExecutions.StartJobExecutionAsync(job.JobName);
        }

        private class DatabaseTarget
        {
            public string Server { get; }
            public string Name { get; }

            public DatabaseTarget(string server, string name)
            {
                Server = server;
                Name = name;
            }

            public override int GetHashCode()
            {
                return (Server + Name).GetHashCode();
            }
        }

        public static async Task<object> GetJobStatus(string serverName, string databaseName, string userName, string password, Guid id)
        {
            var securePassword = new SecureString();
            foreach (var ch in password)
            {
                securePassword.AppendChar(ch);
            }

            securePassword.MakeReadOnly();

            var connection = new AzureSqlJobConnection(serverName, databaseName, userName, securePassword);

            var elasticJobs = new AzureSqlJobClient(connection);

            var jobExecution = await elasticJobs.JobExecutions.GetJobExecutionAsync(id);

            var jjobExecutionTasks = await elasticJobs.JobTaskExecutions.ListJobTaskExecutions(id);

            return new
            {
                jobExecution,
                jjobExecutionTasks
            };
        }

        public static async Task DropJob(string serverName, string databaseName, string userName, string password, Guid id)
        {
            var securePassword = new SecureString();
            foreach (var ch in password)
            {
                securePassword.AppendChar(ch);
            }

            securePassword.MakeReadOnly();

            var connection = new AzureSqlJobConnection(serverName, databaseName, userName, securePassword);

            var elasticJobs = new AzureSqlJobClient(connection);

            await elasticJobs.JobExecutions.CancelJobExecutionAsync(id);
        }
    }
}
