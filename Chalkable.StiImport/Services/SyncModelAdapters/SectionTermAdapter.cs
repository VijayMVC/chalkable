using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class SectionTermAdapter : SyncModelAdapter<SectionTerm>
    {
        public SectionTermAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private MarkingPeriodClass Selector(SectionTerm x)
        {
            return new MarkingPeriodClass
            {
                ClassRef = x.SectionID,
                MarkingPeriodRef = x.TermID,
            };
        }

        protected override void InsertInternal(IList<SectionTerm> entities)
        {
            var cts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ClassService.AssignClassToMarkingPeriod(cts);
        }

        protected override void UpdateInternal(IList<SectionTerm> entities)
        {
            //Nothing here
        }

        protected override void DeleteInternal(IList<SectionTerm> entities)
        {
            var mps = entities.Select(Selector).ToList();
            ServiceLocatorSchool.ClassService.DeleteMarkingPeriodClasses(mps);
        }

        protected override void PrepareToDeleteInternal(IList<SectionTerm> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}