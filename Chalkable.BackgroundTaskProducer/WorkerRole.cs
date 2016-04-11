using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Chalkable.BackgroundTaskProducer.Producers;
using Chalkable.Common;
using Chalkable.Web.Tools;
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
            
            cp.AddProducer("Sis Import Producer", new SisImportProducer());
            cp.AddProducer("Cleanup Manager", new CleanupManager("CleanupManager"));
            cp.AddProducer("Db backup Producer", new DbBackupProducer("DbBackupProducer"));
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
                        raygunClient.SendInBackground(ex, new []{ Settings.WindowsAzureOAuthRelyingPartyName, "task-producer" });
                    }
                    catch (Exception) { }
#else
                    throw;
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
            return base.OnStart();
        }

    }
}
