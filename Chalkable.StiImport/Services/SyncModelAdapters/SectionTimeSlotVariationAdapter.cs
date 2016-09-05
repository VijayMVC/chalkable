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

        private Data.School.Model.SectionTimeSlotVariation Selector(SectionTimeSlotVariation x)
        {
            return new Data.School.Model.SectionTimeSlotVariation
            {
                ClassRef = x.SectionID,
                ScheduledTimeSlotVariationRef = x.TimeSlotVariationID
            };
        }

        protected override void InsertInternal(IList<SectionTimeSlotVariation> entities)
        {
            var sectionTimeSlotVariations = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.AddSectionTimeSlotVariations(sectionTimeSlotVariations);
        }

        protected override void UpdateInternal(IList<SectionTimeSlotVariation> entities)
        {
        }

        protected override void DeleteInternal(IList<SectionTimeSlotVariation> entities)
        {
            var sectionTimeSlotVariations = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ScheduledTimeSlotService.DeleteSectionTimeSlotVariations(sectionTimeSlotVariations);
        }

        protected override void PrepareToDeleteInternal(IList<SectionTimeSlotVariation> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}