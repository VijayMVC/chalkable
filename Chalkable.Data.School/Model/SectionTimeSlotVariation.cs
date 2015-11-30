using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class SectionTimeSlotVariation
    {
        [PrimaryKeyFieldAttr]
        public int ClassRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int ScheduledTimeSlotVariationRef { get; set; }
    }
}