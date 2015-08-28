using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class SectionTimeSlotVariationAdapter : SyncModelAdapter<SectionTimeSlotVariation>
    {
        public SectionTimeSlotVariationAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<SectionTimeSlotVariation> entities)
        {
            var sectionTimeSlotVariations = entities.Select(x => new Data.School.Model.SectionTimeSlotVariation
            {
                ClassRef = x.SectionID,
                ScheduledTimeSlotVariationRef = x.TimeSlotVariationID
            }).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.AddSectionTimeSlotVariations(sectionTimeSlotVariations);
        }

        protected override void UpdateInternal(IList<SectionTimeSlotVariation> entities)
        {
        }

        protected override void DeleteInternal(IList<SectionTimeSlotVariation> entities)
        {
            var sectionTimeSlotVariations = entities.Select(x => new Data.School.Model.SectionTimeSlotVariation
            {
                ClassRef = x.SectionID,
                ScheduledTimeSlotVariationRef = x.TimeSlotVariationID
            }).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.DeleteSectionTimeSlotVariations(sectionTimeSlotVariations);
        }
    }
}