using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    class StandardizedTestScoreTypeAdapter : SyncModelAdapter<StandardizedTestScoreType>
    {
        public StandardizedTestScoreTypeAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.StandardizedTestScoreType Selector(StandardizedTestScoreType x)
        {
            return new Data.School.Model.StandardizedTestScoreType
            {
                Id = x.StandardizedTestScoreTypeID,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description,
                NcesCode = x.NCESCode,
                SifCode = x.SIFCode,
                StateCode = x.StateCode,
                StandardizedTestRef = x.StandardizedTestID
            };
        }

        protected override void InsertInternal(IList<StandardizedTestScoreType> entities)
        {
            var sts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardizedTestService.AddStandardizedTestScoreTypes(sts);
        }

        protected override void UpdateInternal(IList<StandardizedTestScoreType> entities)
        {
            var sts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardizedTestService.EditStandardizedTestScoreTypes(sts);
        }

        protected override void DeleteInternal(IList<StandardizedTestScoreType> entities)
        {
            var toDelete = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardizedTestService.DeleteStandardizedTestScoreTypes(toDelete);
        }

        protected override void PrepareToDeleteInternal(IList<StandardizedTestScoreType> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}