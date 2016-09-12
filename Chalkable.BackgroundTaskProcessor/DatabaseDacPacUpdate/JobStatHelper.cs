using System;
using System.Collections.Generic;
using System.Text;
using Chalkable.Data.Common;

namespace Chalkable.BackgroundTaskProcessor.DatabaseDacPacUpdate
{
    public class JobStatHelper
    {
        private string dbName;
        private string serverUrl;
        private string userName;
        private string password;

        public JobStatHelper(AzureSqlJobCredentials creds)
        {
            dbName = creds.DatabaseName;
            serverUrl = creds.ServerName;
            userName = creds.Username;
            password = creds.Password;
        }

        private string ConnectionString
        {
            get { return $"Data Source={serverUrl};Initial Catalog={dbName};UID={userName};Pwd={password}"; }
        }
        public IList<JobExecutionStat> GetChilderJobExecutionStat(string jobName)
        {
            string sql = $@"select 
	__ElasticDatabaseJob.JobExecution.Lifecycle, count(*) as [Count] 
from 
	__ElasticDatabaseJob.Job 
	join __ElasticDatabaseJob.JobExecution on __ElasticDatabaseJob.Job.LastJobExecution_JobExecutionId = __ElasticDatabaseJob.JobExecution.ParentJobExecutionId
	--join __ElasticDatabaseJob.JobTaskExecution on __ElasticDatabaseJob.JobExecution.JobExecutionId = __ElasticDatabaseJob.JobTaskExecution.JobExecutionId
where 
	name = '{jobName}'
group by
	__ElasticDatabaseJob.JobExecution.Lifecycle
";
            using (var uow = new UnitOfWork(ConnectionString, false))
            {
                var cmd = uow.GetTextCommand(sql);
                using (var reader = cmd.ExecuteReader())
                {
                    var result = reader.ReadList<JobExecutionStat>();
                    return result;
                }
            }
        }

        public IList<JobTaskExecution> GetJobTaskExecutions(string jobName, DateTime? endAfter, DateTime? endBefore)
        {
            var sql = new StringBuilder();
            sql.Append(@"select 
	__ElasticDatabaseJob.JobTaskExecution.JobTaskExecutionId,
	__ElasticDatabaseJob.JobTaskExecution.EndTime,
	__ElasticDatabaseJob.JobTaskExecution.[Message]
from 
	__ElasticDatabaseJob.Job 
	join __ElasticDatabaseJob.JobExecution on __ElasticDatabaseJob.Job.LastJobExecution_JobExecutionId = __ElasticDatabaseJob.JobExecution.ParentJobExecutionId
	join __ElasticDatabaseJob.JobTaskExecution on __ElasticDatabaseJob.JobExecution.JobExecutionId = __ElasticDatabaseJob.JobTaskExecution.JobExecutionId
where 
	name = @name	
	and __ElasticDatabaseJob.JobTaskExecution.[Message] is not null ");
            if (endAfter.HasValue)
                sql.Append("and __ElasticDatabaseJob.JobTaskExecution.EndTime > @endAfter ");
            if (endBefore.HasValue)
                sql.Append("and __ElasticDatabaseJob.JobTaskExecution.EndTime < @endBefore ");

            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"name", jobName},
                {"endAfter", endAfter},
                {"endBefore", endBefore} 
            };
            using (var uow = new UnitOfWork(ConnectionString, false))
            {
                var cmd = uow.GetTextCommandWithParams(sql.ToString(), ps);
                using (var reader = cmd.ExecuteReader())
                {
                    var result = reader.ReadList<JobTaskExecution>();
                    return result;
                }
            }
        } 
    }
}