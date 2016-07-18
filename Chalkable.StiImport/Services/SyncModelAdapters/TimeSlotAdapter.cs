using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class TimeSlotAdapter : SyncModelAdapter<TimeSlot>
    {
        public TimeSlotAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Period Selector(TimeSlot x)
        {
            return new Period
            {
                Id = x.TimeSlotID,
                Order = x.Sequence,
                SchoolYearRef = x.AcadSessionID,
                Name = x.Name
            };
        }

        protected override void InsertInternal(IList<TimeSlot> entities)
        {
            var periods = entities.Select(Selector).ToList();
            ServiceLocatorSchool.PeriodService.AddPeriods(periods);
        }

        protected override void UpdateInternal(IList<TimeSlot> entities)
        {
            var periods = entities.Select(Selector).ToList();
            ServiceLocatorSchool.PeriodService.Edit(periods);
        }

        protected override void DeleteInternal(IList<TimeSlot> entities)
        {
            var ids = entities.Select(x => x.TimeSlotID).ToList();
            ServiceLocatorSchool.PeriodService.Delete(ids);
        }
    }
}