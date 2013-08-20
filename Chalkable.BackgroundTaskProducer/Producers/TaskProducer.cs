using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public interface ITaskProducer
    {
        void Produce();
    }

    public class CompositeProducer : ITaskProducer
    {
        private Dictionary<string, ITaskProducer> producers = new Dictionary<string, ITaskProducer>();
        public void AddProducer(string name, ITaskProducer producer)
        {
            producers.Add(name, producer);
        }

        public void Produce()
        {
            foreach (var taskProducer in producers)
            {
                Trace.TraceInformation(string.Format("Start producer {0}", taskProducer.Key));
                try
                {
                    taskProducer.Value.Produce();
                    Trace.TraceInformation(string.Format("Producer {0} finishes", taskProducer.Key));
                }
                catch (Exception ex)
                {
                    Trace.TraceError(string.Format("Producer {0} throws an exception", taskProducer.Key));
                    Trace.TraceError(ex.Message);
                }
            }
        }
    }
}
