using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Microsoft.Azure.SqlDatabase.Jobs.Client;

namespace Chalkable.BackgroundTaskProcessor
{
    class AzureSqlJobCredentials
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    class DatabaseTarget
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

    public class DatabaseDacPacUpdateTaskHandler : ITaskHandler
    {
        private static AzureSqlJobClient CreateAzureSqlJobClient(AzureSqlJobCredentials creds)
        {
            var securePassword = new SecureString();
            foreach (var ch in creds.Password)
            {
                securePassword.AppendChar(ch);
            }

            securePassword.MakeReadOnly();

            var connection = new AzureSqlJobConnection(creds.ServerName, creds.DatabaseName, creds.Username, securePassword);

            return new AzureSqlJobClient(connection);
        }

        private static async Task<JobExecutionInfo> DeployDacPac(AzureSqlJobClient elasticJobs, string dacPacName, string dacPacUri, IEnumerable<DatabaseTarget> targets)
        {
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

        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            bool res;
            try
            {
                var data = DatabaseDacPacUpdateTaskData.FromString(task.Data);

                log.LogInfo("DACPAC: " + data.DacPacName);

                var azureJobsCreds = new AzureSqlJobCredentials
                {
                    ServerName = data.ServerName,
                    DatabaseName = data.DatabaseName,
                    Username = data.Username,
                    Password = data.Password
                };

                var work = Do(azureJobsCreds, log, data);

                Task.WhenAll(work);

                res = work.Result;
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    log.LogError("Error during database update");
                    log.LogError(ex.Message);
                    log.LogError(ex.StackTrace);
                    ex = ex.InnerException;
                }
                res = false;
            }
            return res;
        }

        private async static Task<bool> Do(AzureSqlJobCredentials azureJobsCreds, BackgroundTaskService.BackgroundTaskLog log, DatabaseDacPacUpdateTaskData data)
        {
            var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();

            var masterTargets = new HashSet<DatabaseTarget>
                {
                    new DatabaseTarget(Settings.ChalkableSchoolDbServers.First(), Settings.MasterDbName)
                };

            var schoolTargets = new HashSet<DatabaseTarget>();

            Settings.ChalkableSchoolDbServers.ToList().ForEach(_ =>
            {
                schoolTargets.Add(new DatabaseTarget(_, Settings.SchoolTemplateDbName));
            });

            serviceLocator.DistrictService.GetDistricts().ForEach(_ =>
            {
                schoolTargets.Add(new DatabaseTarget(_.ServerUrl, _.Name));
            });

            var elasticJobs = CreateAzureSqlJobClient(azureJobsCreds);

            log.LogInfo("DacPac deployment initated");

            var masterJob = await DeployDacPac(elasticJobs, data.DacPacName + "-master", data.MasterDacPacUri, masterTargets);
            var schoolsJob = await DeployDacPac(elasticJobs, data.DacPacName + "-school", data.SchoolDacPacUri, schoolTargets);

            log.LogInfo("DacPac deployment started");

            while (true)
            {
                var masterJobStatus = await elasticJobs.JobExecutions.GetJobExecutionAsync(masterJob.JobExecutionId);
                var schoolJobStatus = await elasticJobs.JobExecutions.GetJobExecutionAsync(schoolsJob.JobExecutionId);

                if (masterJobStatus.Lifecycle == JobExecutionLifecycle.Failed
                    || schoolJobStatus.Lifecycle == JobExecutionLifecycle.Failed)
                {
                    if (masterJob.Lifecycle == JobExecutionLifecycle.Failed)
                    {
                        await LogErrors(log, elasticJobs, masterJob);
                    }

                    if (schoolJobStatus.Lifecycle == JobExecutionLifecycle.Failed)
                    {
                        await LogErrors(log, elasticJobs, schoolJobStatus);
                    }

                    log.LogError("Deploy failed");

                    return false;
                }

                if (masterJob.Lifecycle == JobExecutionLifecycle.Succeeded
                    || schoolJobStatus.Lifecycle == JobExecutionLifecycle.Succeeded)
                {
                    log.LogInfo("Deploy success");

                    return true;
                }

                await Task.Delay(500);
            }
        }

        private static async Task LogErrors(BackgroundTaskService.BackgroundTaskLog log, AzureSqlJobClient elasticJobs, JobExecutionInfo masterJob)
        {
            var taskExecutions =
                await elasticJobs.JobTaskExecutions.ListJobTaskExecutions(masterJob.JobExecutionId);

            foreach (
                var taskExtension in
                    taskExecutions.Where(taskExtension => taskExtension.Lifecycle == JobTaskExecutionLifecycle.Failed))
            {
                var failedJob = await elasticJobs.ScriptBatchExecutions.ListScriptBatchExecutions(
                    taskExtension.JobTaskExecutionId);

                foreach (
                    var scriptBatchExecutionInfo in
                        failedJob.Where(
                            scriptBatchExecutionInfo =>
                                scriptBatchExecutionInfo.Lifecycle == ScriptBatchExecutionLifecycle.Failed))
                {
                    log.LogError(scriptBatchExecutionInfo.Message);
                }
            }
        }
    }
}