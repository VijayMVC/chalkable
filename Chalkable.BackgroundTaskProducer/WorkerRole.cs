using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Chalkable.BackgroundTaskProducer.Producers;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Tools;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.ServiceRuntime;
using Mindscape.Raygun4Net;

namespace Chalkable.BackgroundTaskProducer
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            Trace.WriteLine("BackgroundTaskProducer entry point called", "Information");
            var cp = new CompositeProducer();
            //cp.AddProducer("Process Reminders Producer", new AllDistrictsProducer("ProcessReminderProducer", BackgroundTaskTypeEnum.ProcessReminder));
            //cp.AddProducer("Attendance Notification Producer", new AllDistrictsProducer("AttendanceNotificationProducer", BackgroundTaskTypeEnum.AttendanceNotification));
            //cp.AddProducer("Teacher Attendance Notification Producer", new AllDistrictsProducer("TeacherAttendanceNotification", BackgroundTaskTypeEnum.TeacherAttendanceNotification));
            
            cp.AddProducer("Sis Import Producer", new AllDistrictsProducer("SisImportProducer", BackgroundTaskTypeEnum.SisDataImport));
            cp.AddProducer("Cleanup Manager", new CleanupManager("CleanupManager"));
            var raygunClient = new RaygunClient();
            raygunClient.ApplicationVersion = CompilerHelper.Version;      
            while (true)
            {
                try
                {
                    Thread.Sleep(20000);
                    Trace.TraceInformation("Producing cycle is starting");
                    cp.Produce();
                    Trace.TraceInformation("Producing cycle is ended");
                }
                catch (Exception ex)
                {
#if !DEBUG
                    try
                    {
                        raygunClient.SendInBackground(ex);
                    }
                    catch (Exception) { }
#endif
                    while (ex != null)
                    {
                        Trace.TraceInformation("Exception during task producing");
                        Trace.TraceInformation(ex.Message);
                        Trace.TraceInformation(ex.StackTrace);
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
            String wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
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
            roleInstanceDiagnosticManager.SetCurrentConfiguration(diagnosticMonitorConfiguration);
        }
    }
}
