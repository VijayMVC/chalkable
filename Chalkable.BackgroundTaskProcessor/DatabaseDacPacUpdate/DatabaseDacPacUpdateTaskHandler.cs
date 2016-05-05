using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Microsoft.Azure.SqlDatabase.Jobs.Client;

namespace Chalkable.BackgroundTaskProcessor.DatabaseDacPacUpdate
{
    public class DatabaseDacPacUpdateTaskHandler : ITaskHandler
    {
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

        private static SecureString CreatePassword(string s)
        {
            var securePassword = new SecureString();
            foreach (var ch in s)
            {
                securePassword.AppendChar(ch);
            }
            securePassword.MakeReadOnly();
            return securePassword;
        }

        private static AzureSqlJobClient CreateAzureSqlJobClient(AzureSqlJobCredentials creds)
        {
            var securePassword = CreatePassword(creds.Password);
            var connection = new AzureSqlJobConnection(creds.ServerName, creds.DatabaseName, creds.Username, securePassword);
            return new AzureSqlJobClient(connection);
        }

        private static async Task CreateDbTarget(AzureSqlJobClient elasticJobs, CustomCollectionTargetInfo rootTarget
            , BackgroundTaskService.BackgroundTaskLog log, DatabaseTarget dbTarget)
        {
            try
            {
                var dbTargetInfo = await elasticJobs.Targets.CreateDatabaseTargetAsync(dbTarget.Server, dbTarget.Name);
                var result = await elasticJobs.Targets.AddChildTargetAsync(rootTarget.TargetId, dbTargetInfo.TargetId);
                if (!result)
                    throw new Exception("child target adding returns false");
                log.LogInfo("Targeting db " + dbTarget.Name + "@" + dbTarget.Server + " complete as " + dbTargetInfo.TargetId);
            }
            catch (Exception e)
            {
                log.LogException(e);
                throw new Exception("Targeting db " + dbTarget.Name + "@" + dbTarget.Server + " failed.", e);
            }
        }

        private static async Task CreateDbTargets(AzureSqlJobClient elasticJobs, CustomCollectionTargetInfo rootTarget, BackgroundTaskService.BackgroundTaskLog log
            , IList<DatabaseTarget> dbTargets, int pageSize, int delay)
        {
            for (int i = 0; i < dbTargets.Count; i += pageSize)
            {
                await Task.WhenAll(dbTargets.Skip(i).Take(pageSize).Select(x => CreateDbTarget(elasticJobs, rootTarget, log, x)).ToArray());
                Thread.Sleep(delay);
            }
        }

        private static async Task<JobExecutionInfo> StartJob(AzureSqlJobClient elasticJobs, CustomCollectionTargetInfo rootTarget
            , BackgroundTaskService.BackgroundTaskLog log, string dacPacName, string dacPacUri)
        {
            var dacPackDef = await elasticJobs.Content.GetContentAsync(dacPacName) ??
                             await elasticJobs.Content.CreateDacpacAsync(dacPacName, new Uri(dacPacUri));
            var pwd = CreatePassword(Settings.ChalkableSchoolDbPassword);
            var credential = await elasticJobs.Credentials.CreateCredentialAsync(Guid.NewGuid().ToString(), Settings.ChalkableSchoolDbUser, pwd);
            var job = await elasticJobs.Jobs.CreateJobAsync("Apply DACPAC " + dacPacName + " " + Guid.NewGuid(), new JobBuilder
            {
                ContentName = dacPackDef.ContentName,
                TargetId = rootTarget.TargetId,
                CredentialName = credential.CredentialName
            });
            log.LogInfo("Job created as " + job.TargetId + " " + job.JobName);
            var jobExecution = await elasticJobs.JobExecutions.StartJobExecutionAsync(job.JobName);
            log.LogInfo($"{dacPacName} execution job as {jobExecution.JobExecutionId} with lifecycle {jobExecution.Lifecycle}");
            return jobExecution;
        }

        private static async Task<bool> DeployDacPac(AzureSqlJobCredentials azureJobsCreds, string dacPacName, string dacPacUri, 
            IList<DatabaseTarget> targets, BackgroundTaskService.BackgroundTaskLog log)
        {
            var elasticJobs = CreateAzureSqlJobClient(azureJobsCreds);

            var target = await elasticJobs.Targets.CreateCustomCollectionTargetAsync("Targets for DACPAC " + dacPacName + " " + Guid.NewGuid());
            log.LogInfo("Job targets created as " + target.TargetId + " " + target.CustomCollectionName);
            await CreateDbTargets(elasticJobs, target, log, targets, 20, 2000);
            var jobExecution = await StartJob(elasticJobs, target, log, dacPacName, dacPacUri);
            JobStatHelper helper = new JobStatHelper(azureJobsCreds);

            DateTime? since = null;
            var lifecycle = jobExecution.Lifecycle;
            while (true)
            {
                var status = await elasticJobs.JobExecutions.GetJobExecutionAsync(jobExecution.JobExecutionId);
                if (status.Lifecycle != lifecycle)
                    log.LogInfo($"{dacPacName} job is {status.Lifecycle}");
                lifecycle = status.Lifecycle;

                var stats = helper.GetChilderJobExecutionStat(dacPacName);
                log.LogInfo("Task stats:\n" + stats.Select(x=>x.Lifecycle + ": " + x.Count).JoinString("\n"));

                var taskExecutions = helper.GetJobTaskExecutions(dacPacName, since, null);
                log.LogInfo(taskExecutions.Select(x => x.Message));
                since = taskExecutions.Max(x => x.EndTime);

                switch (status.Lifecycle)
                {
                    case JobExecutionLifecycle.Failed:
                    case JobExecutionLifecycle.Canceled:
                    case JobExecutionLifecycle.Skipped:
                    case JobExecutionLifecycle.TimedOut:
                        log.LogError("Deploy " + status.Lifecycle);
                        return false;

                    case JobExecutionLifecycle.Succeeded:
                        log.Flush();
                        return true;
                }
                await Task.Delay(30000);
            }
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
                work.Wait();
                res = work.Result;
            }
            catch (Exception ex)
            {
                log.LogError("Error during database update");
                log.LogException(ex);
                res = false;
            }
            return res;
        }

        private async static Task<bool> Do(AzureSqlJobCredentials azureJobsCreds, BackgroundTaskService.BackgroundTaskLog log, DatabaseDacPacUpdateTaskData data)
        {
            var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();

            var masterTargets = new List<DatabaseTarget>
                {
                    new DatabaseTarget(Settings.ChalkableSchoolDbServers.First(), Settings.MasterDbName)
                };
            log.LogInfo("Master DacPac deployment initated");

            if (!(await DeployDacPac(azureJobsCreds, data.DacPacName + "-master", data.MasterDacPacUri, masterTargets, log)))
                return false;            

            log.LogInfo("Preparing schools DacPac targets");

            var schoolTargets = new List<DatabaseTarget>(
                    Settings.ChalkableSchoolDbServers.Select(x => new DatabaseTarget(x, Settings.SchoolTemplateDbName))
                    .Concat(serviceLocator.DistrictService.GetDistricts().Select(x => new DatabaseTarget(x.ServerUrl, x.Id.ToString())))
                );

            log.LogInfo("Schools DacPac deployment initated");

            if (!(await DeployDacPac(azureJobsCreds, data.DacPacName + "-school", data.SchoolDacPacUri, schoolTargets, log)))
                return false;

            log.LogInfo("Deploy success");

            return true;
        }
        
        public static Task<bool> Test(AzureSqlJobCredentials creds, BackgroundTaskService.BackgroundTaskLog log, DatabaseDacPacUpdateTaskData data)
        {
            return Do(creds, log, data);
        }
    }
}