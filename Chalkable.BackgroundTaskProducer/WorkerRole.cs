using System.Diagnostics;
using System.Net;
using System.Threading;
using Chalkable.BackgroundTaskProducer.Producers;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Chalkable.BackgroundTaskProducer
{
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            Trace.WriteLine("BackgroundTaskProducer entry point called", "Information");
            var cp = new CompositeProducer();
            cp.AddProducer("Empty school producer", new EmptySchoolTaskProducer());
            cp.AddProducer("Demo school producer", new CreateDemoSchoolTaskProducer());
            
            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Producing cycle is starting");
                cp.Produce();
                Trace.TraceInformation("Producing cycle is ended");
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
