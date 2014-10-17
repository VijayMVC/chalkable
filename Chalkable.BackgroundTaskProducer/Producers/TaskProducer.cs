using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Chalkable.Common;

namespace Chalkable.BackgroundTaskProducer.Producers
{
    public interface ITaskProducer
    {
        void Produce();
    }

    public abstract class BaseProducer : ITaskProducer
    {
        private DateTime startTime;
        private long count;
        private long interval;
        private long intervalStart, intervalEnd;

        protected BaseProducer(string configSectionName)
        {
            var config = Settings.GetProducerConfig(configSectionName);
            startTime = DateTime.UtcNow;
            count = 0;
            interval = config.Interval;
            intervalStart = config.IntervalStart;
            intervalEnd = config.IntervalEnd;
        }

        public void Produce()
        {
            var now = DateTime.UtcNow;
            var secondsOfDay = (now - now.Date).TotalSeconds;
            if (secondsOfDay >= intervalStart && secondsOfDay <= intervalEnd)
            {
                var currentTime = startTime.AddSeconds(interval * count);//TODO: this logic doesn't take into account working interval 2014-10-16
                if (currentTime <= now)
                {
                    Trace.TraceWarning(string.Format(ChlkResources.BG_PROCESSING_FOR_TIME, currentTime));
                    ProduceInternal(currentTime);
                    count++;
                }
            }
        }

        protected abstract void ProduceInternal(DateTime currentTimeUtc);

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
