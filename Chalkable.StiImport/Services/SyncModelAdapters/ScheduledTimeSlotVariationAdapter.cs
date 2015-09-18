using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class ScheduledTimeSlotVariationAdapter : SyncModelAdapter<ScheduledTimeSlotVariation>
    {
        public ScheduledTimeSlotVariationAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.ScheduledTimeSlotVariation Selector(ScheduledTimeSlotVariation x)
        {
            return new Data.School.Model.ScheduledTimeSlotVariation
            {
                Id = x.TimeSlotVariationId,
                BellScheduleRef = x.BellScheduleId,
                PeriodRef = x.TimeSlotId,
                Name = x.Name,
                Description = x.Description,
                StartTime = x.StartTime,
                EndTime = x.EndTime
            };
        }

        protected override void InsertInternal(IList<ScheduledTimeSlotVariation> entities)
        {
            var scheduledTimeSlotVariations = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.AddScheduledTimeSlotVariations(scheduledTimeSlotVariations);
        }

        protected override void UpdateInternal(IList<ScheduledTimeSlotVariation> entities)
        {
            var scheduledTimeSlotVariations = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.EditScheduledTimeSlotVariations(scheduledTimeSlotVariations);
        }

        protected override void DeleteInternal(IList<ScheduledTimeSlotVariation> entities)
        {
            var sectionTimeSlotVariations = entities.Select(x => new Data.School.Model.ScheduledTimeSlotVariation
            { Id = x.TimeSlotVariationId }).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.DeleteScheduledTimeSlotVariations(sectionTimeSlotVariations);
        }
    }
}