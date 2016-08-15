using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class ScheduledSectionAdapter : SyncModelAdapter<ScheduledSection>
    {
        public ScheduledSectionAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private ClassPeriod Selector(ScheduledSection x)
        {
            return new ClassPeriod
            {
                ClassRef = x.SectionID,
                DayTypeRef = x.DayTypeID,
                PeriodRef = x.TimeSlotID,
            };
        }

        protected override void InsertInternal(IList<ScheduledSection> entities)
        {
            var classPeriods = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ClassPeriodService.Add(classPeriods);
        }

        protected override void UpdateInternal(IList<ScheduledSection> entities)
        {
            //Nothing here
        }

        protected override void DeleteInternal(IList<ScheduledSection> entities)
        {
            var classPeriods = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ClassPeriodService.Delete(classPeriods);
        }

        protected override void PrepareToDeleteInternal(IList<ScheduledSection> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}