using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Chalkable.Common;
using Chalkable.Web.Tools;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.WindowsAzure.ServiceRuntime;
using Mindscape.Raygun4Net;


namespace Chalkable.BackgroundTaskProcessor
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("Chalkable.BackgroundTaskProcessor entry point called", "Information");
            var processor = new TaskProcessor();
            var delay = Settings.TaskProcessorDelay;
            var raygunClient = new RaygunClient {ApplicationVersion = CompilerHelper.Version};

            if (delay <= 0)
            {
                Trace.TraceError("Task processing is turned off");
            }

            while (true)
            {
                if (delay <= 0)
                {
                    Thread.Sleep(1000);                    
                    continue;
                }
                try
                {
                    Thread.Sleep(delay);
                    processor.Process();
                }
                catch (Exception ex)
                {
#if !DEBUG
                    try
                    {
                        raygunClient.SendInBackground(ex, new []{ Settings.Domain, "task-processor" });
                        Telemetry.TrackException(ex);                        
                    }
                    catch (Exception){}
#else
                    Debug.Fail(ex.Message);
#endif                    

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
            TelemetryConfiguration.Active.InstrumentationKey = Settings.InstrumentationKey;
          
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            
            return base.OnStart();
        }
    }
}
