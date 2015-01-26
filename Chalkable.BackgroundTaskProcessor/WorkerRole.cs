using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Chalkable.Common;
using Chalkable.Web.Tools;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;
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
            var raygunClient = new RaygunClient();
            raygunClient.ApplicationVersion = CompilerHelper.Version;

            if (delay <= 0)
            {
                Trace.TraceError("Task processing is turned off");
            }

            while (true)
            {
                if (delay <= 0)
                {
                    Thread.Sleep(10000);                    
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
                        raygunClient.SendInBackground(ex);
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
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            ConfigureDiagnostics();
            
            return base.OnStart();
        }

        private void ConfigureDiagnostics()
        {
            /*String wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(wadConnectionString));

            RoleInstanceDiagnosticManager roleInstanceDiagnosticManager =
                cloudStorageAccount.CreateRoleInstanceDiagnosticManager(RoleEnvironment.DeploymentId, RoleEnvironment.CurrentRoleInstance.Role.Name, RoleEnvironment.CurrentRoleInstance.Id);
            DiagnosticMonitorConfiguration diagnosticMonitorConfiguration = roleInstanceDiagnosticManager.GetCurrentConfiguration();
            diagnosticMonitorConfiguration.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(5d);
            diagnosticMonitorConfiguration.Logs.ScheduledTransferPeriod = TimeSpan.FromSeconds(30);
            diagnosticMonitorConfiguration.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("Application!*");
            diagnosticMonitorConfiguration.WindowsEventLog.DataSources.Add("System!*");
            diagnosticMonitorConfiguration.WindowsEventLog.ScheduledTransferPeriod = TimeSpan.FromMinutes(5d);
            PerformanceCounterConfiguration performanceCounterConfiguration = new PerformanceCounterConfiguration();
            performanceCounterConfiguration.CounterSpecifier = @"\Processor(_Total)\% Processor Time";
            performanceCounterConfiguration.SampleRate = TimeSpan.FromSeconds(10d);
            diagnosticMonitorConfiguration.PerformanceCounters.DataSources.Add(performanceCounterConfiguration);
            diagnosticMonitorConfiguration.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(1d);
            roleInstanceDiagnosticManager.SetCurrentConfiguration(diagnosticMonitorConfiguration);*/
        }
    }
}
