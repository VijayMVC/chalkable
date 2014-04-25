using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Chalkable.BackgroundTaskProducer.Producers;
using Chalkable.Data.Master.Model;
using Microsoft.WindowsAzure.ServiceRuntime;

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
