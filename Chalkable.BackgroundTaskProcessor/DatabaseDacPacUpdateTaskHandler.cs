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
    public class AzureSqlJobCredentials
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

        private static async Task<bool> DeployDacPac(AzureSqlJobClient elasticJobs, string dacPacName, string dacPacUri, 
            IEnumerable<DatabaseTarget> targets, BackgroundTaskService.BackgroundTaskLog log)
        {
            Memoization.Clear();

            var dacPackDef = await elasticJobs.Content.GetContentAsync(dacPacName) ??
                             await elasticJobs.Content.CreateDacpacAsync(dacPacName, new Uri(dacPacUri));

            var target = await elasticJobs.Targets.CreateCustomCollectionTargetAsync("Targets for DACPAC " + dacPacName + " " + Guid.NewGuid());

            log.LogInfo("Job targets created as " + target.TargetId + " " + target.CustomCollectionName);

            var databaseTargets = targets as IList<DatabaseTarget> ?? targets.ToList();

            var count = databaseTargets.Count();
            const int pageSize = 10;
            for (var i = 0; i < count; i += pageSize)
            {
                await Task.WhenAll(databaseTargets.Skip(i).Take(pageSize).Select(async _ =>
                {
                    try
                    {
                        var dbTarget = await elasticJobs.Targets.GetDatabaseTargetAsync(_.Server, _.Name) ??
                                       await elasticJobs.Targets.CreateDatabaseTargetAsync(_.Server, _.Name);
                        var result = await elasticJobs.Targets.AddChildTargetAsync(target.TargetId, dbTarget.TargetId);
                        log.LogInfo("Targeting db " + _.Name + "@" + _.Server + " complete as " + dbTarget.TargetId);
                        return result;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Targeting db " + _.Name + "@" + _.Server + " failed.", e);
                    }
                }).ToArray());
            }

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

            log.LogInfo("Job created as " + job.TargetId + " " + job.JobName);

            var masterJob = await elasticJobs.JobExecutions.StartJobExecutionAsync(job.JobName);

            log.LogInfo($"{dacPacName} execution job as " + masterJob.JobExecutionId);
            log.Flush();

            JobExecutionLifecycle masterJobStatusLifecycle = JobExecutionLifecycle.Created;
            while (true)
            {
                var masterJobStatus = await elasticJobs.JobExecutions.GetJobExecutionAsync(masterJob.JobExecutionId);

                if (masterJobStatus.Lifecycle != masterJobStatusLifecycle)
                    log.LogInfo($"{dacPacName} job is " + masterJobStatus.Lifecycle);

                masterJobStatusLifecycle = masterJobStatus.Lifecycle;
                
                var stats = await ProcessChildJobExcutions(log, elasticJobs, masterJobStatus);

                log.LogInfo("Task stats");
                foreach (var k in stats)
                {
                    log.LogInfo($"{k.Key}: {k.Value}");
                }

                switch (masterJobStatus.Lifecycle)
                {
                    case JobExecutionLifecycle.Failed:
                    case JobExecutionLifecycle.Canceled:
                    case JobExecutionLifecycle.Skipped:
                    case JobExecutionLifecycle.TimedOut:
                        log.LogError("Deploy " + masterJobStatus.Lifecycle);
                        log.Flush();

                        return false;

                    case JobExecutionLifecycle.Succeeded:
                        log.Flush();
                        return true;
                }

                log.Flush();
                await Task.Delay(10000);
            }
        }

        public bool Handle(BackgroundTask task, BackgroundTaskService.BackgroundTaskLog log)
        {
            bool res;
            try
            {
                var data = DatabaseDacPacUpdateTaskData.FromString(task.Data);

                log.LogInfo("DACPAC: " + data.DacPacName);
                log.Flush();

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
                log.Flush();
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


            var elasticJobs = CreateAzureSqlJobClient(azureJobsCreds);

            log.LogInfo("Master DacPac deployment initated");
            log.Flush();

            if (!(await DeployDacPac(elasticJobs, data.DacPacName + "-master", data.MasterDacPacUri, masterTargets, log)))
                return false;            

            log.LogInfo("Preparing schools DacPac targets");
            log.Flush();

            var schoolTargets = new HashSet<DatabaseTarget>();

            Settings.ChalkableSchoolDbServers.ToList().ForEach(_ =>
            {
                schoolTargets.Add(new DatabaseTarget(_, Settings.SchoolTemplateDbName));
            });

            serviceLocator.DistrictService.GetDistricts().ForEach(_ =>
            {
                schoolTargets.Add(new DatabaseTarget(_.ServerUrl, _.Id.ToString()));
            });

            log.LogInfo("Schools DacPac deployment initated");
            log.Flush();

            if (!(await DeployDacPac(elasticJobs, data.DacPacName + "-school", data.SchoolDacPacUri, schoolTargets, log)))
                return false;

            log.LogInfo("Deploy success");
            log.Flush();

            return true;
        }

        private static readonly HashSet<string> Memoization = new HashSet<string>();

        private static async Task<Dictionary<string, int>> ProcessChildJobExcutions(BackgroundTaskService.BackgroundTaskLog log, AzureSqlJobClient elasticJobs, JobExecutionInfo execution)
        {            
            var stats = new Dictionary<string, int>();

            var children = await elasticJobs.JobExecutions.ListJobExecutionsAsync(new JobExecutionFilter
            {
                ParentJobExecutionId = execution.JobExecutionId
            });

            foreach (var child in children)
            {
                var tasks = (await elasticJobs.JobTaskExecutions.ListJobTaskExecutions(child.JobExecutionId))
                    .OrderByDescending(x => x.CreatedTime);

                foreach(var task in tasks)
                {
                    var key = task.Lifecycle.ToString();
                    if (!stats.ContainsKey(key))
                        stats.Add(key, 1);
                    else 
                        stats[key]++;

                    if (string.IsNullOrWhiteSpace(task?.Message))
                        continue;
                    
                    var id = $"{task.JobTaskExecutionId}--{task.Lifecycle}";
                    if (Memoization.Contains(id))
                        continue;

                    Memoization.Add(id);

                    log.LogError($"Execution task: {task}, target {child.TargetDescription} status {task.Lifecycle}, {task.Message}");

                    var scripts = await elasticJobs.ScriptBatchExecutions.ListScriptBatchExecutions(task.JobTaskExecutionId);
                    foreach (var script in scripts)
                    {
                        log.LogError($"Execution task script: {script}, status {script.Lifecycle}, {script.Message}");
                    }
                }
            }

            return stats;
        }

        public static Task<bool> Test(AzureSqlJobCredentials creds, BackgroundTaskService.BackgroundTaskLog log, DatabaseDacPacUpdateTaskData data)
        {
            return Do(creds, log, data);
        }
    }
}