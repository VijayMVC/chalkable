using System.Configuration;

namespace Chalkable.BackgroundTaskProducer
{
    class ProducerConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("interval")]
        public long Interval
        {
            get { return (long)this["interval"]; }
        }

        [ConfigurationProperty("intervalStart")]
        public long IntervalStart
        {
            get { return (long)this["intervalStart"]; }
        }

        [ConfigurationProperty("intervalEnd")]
        public long IntervalEnd
        {
            get { return (long)this["intervalEnd"]; }
        }
    }
}
