using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
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
            while (true)
            {
                Thread.Sleep(10000);
                try
                {
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
