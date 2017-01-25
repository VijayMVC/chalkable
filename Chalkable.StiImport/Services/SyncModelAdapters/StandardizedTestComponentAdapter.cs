using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    class StandardizedTestComponentAdapter : SyncModelAdapter<StandardizedTestComponent>
    {
        public StandardizedTestComponentAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.StandardizedTestComponent Selector(StandardizedTestComponent x)
        {
            return new Data.School.Model.StandardizedTestComponent
            {
                Id = x.StandardizedTestComponentID,
                Name = x.Name,
                Code = x.Code,
                DisplayOnTranscript = x.DisplayOnTranscript,
                NcesCode = x.NCESCode,
                SifCode = x.SIFCode,
                StateCode = x.StateCode,
                StandardizedTestRef = x.StandardizedTestID
            };
        }

        protected override void InsertInternal(IList<StandardizedTestComponent> entities)
        {
            var sts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardizedTestService.AddStandardizedTestComponents(sts);
        }

        protected override void UpdateInternal(IList<StandardizedTestComponent> entities)
        {
            var sts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardizedTestService.EditStandardizedTestComponents(sts);
        }

        protected override void DeleteInternal(IList<StandardizedTestComponent> entities)
        {
            var toDelete = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardizedTestService.DeleteStandardizedTestComponents(toDelete);
        }

        protected override void PrepareToDeleteInternal(IList<StandardizedTestComponent> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}