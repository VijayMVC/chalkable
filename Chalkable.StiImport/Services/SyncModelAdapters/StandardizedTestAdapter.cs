using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    class StandardizedTestAdapter : SyncModelAdapter<StandardizedTest>
    {
        public StandardizedTestAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.StandardizedTest Selector(StandardizedTest x)
        {
            return new Data.School.Model.StandardizedTest
            {
                Id = x.StandardizedTestID,
                Name = x.Name,
                DisplayName = x.DisplayName,
                Code = x.Code,
                Description = x.Description,
                DisplayOnTranscript = x.DisplayOnTranscript,
                GradeLevelRef = x.GradeLevelID,
                NcesCode = x.NCESCode,
                SifCode = x.SIFCode,
                StateCode = x.StateCode
            };
        }

        protected override void InsertInternal(IList<StandardizedTest> entities)
        {
            var sts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardizedTestService.AddStandardizedTests(sts);
        }

        protected override void UpdateInternal(IList<StandardizedTest> entities)
        {
            var sts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardizedTestService.EditStandardizedTests(sts);
        }

        protected override void DeleteInternal(IList<StandardizedTest> entities)
        {
            var toDelete = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardizedTestService.DeleteStandardizedTests(toDelete);
        }

        protected override void PrepareToDeleteInternal(IList<StandardizedTest> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}
