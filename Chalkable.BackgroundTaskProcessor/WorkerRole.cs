using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Chalkable.Common;
using Microsoft.WindowsAzure.ServiceRuntime;


namespace Chalkable.BackgroundTaskProcessor
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("Chalkable.BackgroundTaskProcessor entry point called", "Information");
            var processor = new TaskProcessor();
            var delay = Settings.Configuration.TaskProcessorDelay;
            while (true)
            {
                if (delay <= 0)
                {
                    Thread.Sleep(7200000);
                    Trace.TraceError("Task processing is turned off");
                    continue;
                }
                try
                {
                    Thread.Sleep(delay);
                    processor.Process();
                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        Trace.TraceError(ex.Message);
                        Trace.TraceError(ex.StackTrace);
                        ex = ex.InnerException;
                    }
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
