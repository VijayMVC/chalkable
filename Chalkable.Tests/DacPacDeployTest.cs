using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using Chalkable.BackgroundTaskProcessor;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Data.Master.Model;
using Microsoft.Azure.SqlDatabase.Jobs.Client;
using NUnit.Framework;

namespace Chalkable.Tests
{
    public class DacPacDeployTest
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

        [Test]
        public async void CancelJobTest()
        {
            AzureSqlJobCredentials creds = new AzureSqlJobCredentials()
            {
                DatabaseName = "edjb0d1a0ab363747abbc2ee",
                Password = "Hellowebapps1!",
                ServerName = "edjb0d1a0ab363747abbc2ee.database.windows.net",
                Username = "chalkadmin@edjb0d1a0ab363747abbc2ee"
            };
            var elasticJobs = CreateAzureSqlJobClient(creds);
            var id = Guid.Parse("ADD673E6-E46F-4103-A5FD-BBCCD9D74AA0");
            //var id = Guid.Parse("D79231A9-9F15-4B60-BC56-2F0B6D766890");
            var children = await elasticJobs.JobExecutions
                .ListJobExecutionsAsync(new JobExecutionFilter()
                {
                    ParentJobExecutionId = id
                });
            foreach (var jobExecutionInfo in children)
            {
                await elasticJobs.JobExecutions.CancelJobExecutionAsync(jobExecutionInfo.JobExecutionId);
            }
            
            //            await elasticJobs.JobExecutions.CancelJobExecutionAsync(Guid.Parse("D79231A9-9F15-4B60-BC56-2F0B6D766890"));
            await elasticJobs.JobExecutions.CancelJobExecutionAsync(id);
            
        }

        [Test]
        public async void ListPrivateJobsInProgress()
        {
            AzureSqlJobCredentials creds = new AzureSqlJobCredentials()
            {
                DatabaseName = "edjb0d1a0ab363747abbc2ee",
                Password = "Hellowebapps1!",
                ServerName = "edjb0d1a0ab363747abbc2ee.database.windows.net",
                Username = "chalkadmin@edjb0d1a0ab363747abbc2ee"
            };
            var elasticJobs = CreateAzureSqlJobClient(creds);
            var list = await elasticJobs.Jobs.ListJobsAsync();
            var privateJobs = list.ToList();
            foreach (var job in privateJobs)
            {
                //Console.WriteLine($"Job: {job}");

                var executions = await elasticJobs.JobExecutions.ListJobExecutionsAsync(new JobExecutionFilter
                {
                    JobName = job.JobName
                });

                foreach (var execution in executions)
                {
                    if (!(execution.Lifecycle == JobExecutionLifecycle.InProgress || execution.Lifecycle == JobExecutionLifecycle.WaitingForChildJobExecutions))
                        continue;

                    Console.WriteLine($"Execution: {execution} of {job}, status {execution.Lifecycle}");
                    var children = await elasticJobs.JobExecutions.ListJobExecutionsAsync(new JobExecutionFilter
                    {
                        ParentJobExecutionId = execution.JobExecutionId
                    });

                    /*foreach (var child in children)
                    {
                        Console.WriteLine($"Execution child: {child}, status {child.Lifecycle}");

                        var tasks = await elasticJobs.JobTaskExecutions.ListJobTaskExecutions(child.JobExecutionId);

                        foreach (var task in tasks)
                        {
                            Console.WriteLine($"Execution child task: {task}, status {task.Lifecycle}, {task.Message}");

                            var scripts =
                                await
                                    elasticJobs.ScriptBatchExecutions.ListScriptBatchExecutions(task.JobTaskExecutionId);
                            foreach (var script in scripts)
                            {
                                Console.WriteLine($"Execution child task script: {script}, status {script.Lifecycle}, {script.Message}");
                            }
                        }
                    }*/
                }
            }
        }

        [Test]
        public async void CancelPrivateJobsInProgress()
        {
            AzureSqlJobCredentials creds = new AzureSqlJobCredentials()
            {
                DatabaseName = "edjb0d1a0ab363747abbc2ee",
                Password = "Hellowebapps1!",
                ServerName = "edjb0d1a0ab363747abbc2ee.database.windows.net",
                Username = "chalkadmin@edjb0d1a0ab363747abbc2ee"
            };
            var elasticJobs = CreateAzureSqlJobClient(creds);
            var list = await elasticJobs.Jobs.ListJobsAsync();
            var privateJobs = list.Where(x => x.JobName.Contains("private-build")).ToList();
            foreach (var job in privateJobs)
            {
                Console.WriteLine($"Job: {job}");

                var executions = await elasticJobs.JobExecutions.ListJobExecutionsAsync(new JobExecutionFilter
                {
                    JobName = job.JobName
                });

                foreach (var execution in executions)
                {
                    Console.WriteLine($"Execution: {execution}, status {execution.Lifecycle}");

                    await elasticJobs.JobExecutions.CancelJobExecutionAsync(execution.JobExecutionId);

                    Console.WriteLine($"Cancelled {execution.JobExecutionId}");
                }
            }
        }
        
        public void Test()
        {
            AzureSqlJobCredentials creds = new AzureSqlJobCredentials()
            {
                DatabaseName = "edjb0d1a0ab363747abbc2ee",
                Password = "Hellowebapps1!",
                ServerName = "edjb0d1a0ab363747abbc2ee.database.windows.net",
                Username = "chalkadmin@edjb0d1a0ab363747abbc2ee"
            };

            var id = Guid.NewGuid();
            Debug.WriteLine(id);
            BackgroundTaskService.BackgroundTaskLog log = new BackgroundTaskService.BackgroundTaskLog(id, 10);
            var data = DatabaseDacPacUpdateTaskData.FromString("{\"DacPacName\":\"0-7-1d38ca879106-2321\",\"MasterDacPacUri\":\"https://chalkablestat.blob.core.windows.net/artifacts-db/0-7-1d38ca879106-2321/Chalkable.Database.Master.dacpac\",\"SchoolDacPacUri\":\"https://chalkablestat.blob.core.windows.net/artifacts-db/0-7-1d38ca879106-2321/Chalkable.Database.School.dacpac\",\"ServerName\":\"edjb0d1a0ab363747abbc2ee.database.windows.net\",\"DatabaseName\":\"edjb0d1a0ab363747abbc2ee\",\"Username\":\"chalkadmin@edjb0d1a0ab363747abbc2ee\",\"Password\":\"Hellowebapps1!\"}");
            DatabaseDacPacUpdateTaskHandler.Test(creds, log, data).Wait();
        }

        [Test]
        public async void TestJobExecutionTaskList()
        {
            AzureSqlJobCredentials creds = new AzureSqlJobCredentials()
            {
                DatabaseName = "edjb0d1a0ab363747abbc2ee",
                Password = "Hellowebapps1!",
                ServerName = "edjb0d1a0ab363747abbc2ee.database.windows.net",
                Username = "chalkadmin@edjb0d1a0ab363747abbc2ee"
            };
            var elasticJobs = CreateAzureSqlJobClient(creds);
            var id = Guid.Parse("ADD673E6-E46F-4103-A5FD-BBCCD9D74AA0");
            /*var children = await elasticJobs.JobExecutions
                .ListJobExecutionsAsync(new JobExecutionFilter()
                {
                    ParentJobExecutionId = id
                });*/
            var te = await elasticJobs.JobTaskExecutions.ListJobTaskExecutions(id);
            foreach (var info in te)
            {
                Debug.WriteLine($"{info.JobTaskExecutionId} {info.Lifecycle}");
            }

            /*foreach (var jobExecutionInfo in children)
            {
                
            }*/
        }
    }
}