using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class ScheduledTimeSlotAdapter : SyncModelAdapter<ScheduledTimeSlot>
    {
        public ScheduledTimeSlotAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.ScheduledTimeSlot Selector(ScheduledTimeSlot x)
        {
            return new Data.School.Model.ScheduledTimeSlot
            {
                BellScheduleRef = x.BellScheduleID,
                Description = x.Description,
                EndTime = x.EndTime,
                IsDailyAttendancePeriod = x.IsDailyAttendancePeriod,
                StartTime = x.StartTime,
                PeriodRef = x.TimeSlotID
            };
        }

        protected override void InsertInternal(IList<ScheduledTimeSlot> entities)
        {
            var allSts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.Add(allSts);
        }

        protected override void UpdateInternal(IList<ScheduledTimeSlot> entities)
        {
            var allSts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.Edit(allSts);
        }

        protected override void DeleteInternal(IList<ScheduledTimeSlot> entities)
        {
            IList<Data.School.Model.ScheduledTimeSlot> sts = entities.Select(x => new Data.School.Model.ScheduledTimeSlot
            {
                BellScheduleRef = x.BellScheduleID,
                PeriodRef = x.TimeSlotID
            }).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.Delete(sts);
        }

        protected override void PrepareToDeleteInternal(IList<ScheduledTimeSlot> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}