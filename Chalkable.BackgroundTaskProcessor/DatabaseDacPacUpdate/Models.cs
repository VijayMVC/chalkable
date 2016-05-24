using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.BackgroundTaskProcessor.DatabaseDacPacUpdate
{

    public class AzureSqlJobCredentials
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class JobExecutionStat
    {
        public string Lifecycle { get; set; }

        public int Count { get; set; }
    }

    public class JobTaskExecution
    {
        public Guid JobTaskExecutionId { get; set; }
        public DateTime? EndTime { get; set; }
        public string Message { get; set; }
    }


}
