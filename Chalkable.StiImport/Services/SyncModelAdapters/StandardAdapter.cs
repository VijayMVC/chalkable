using System.Collections.Generic;
using System.Linq;
using Standard = Chalkable.StiConnector.SyncModel.Standard;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StandardAdapter : SyncModelAdapter<Standard>
    {
        public StandardAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private Data.School.Model.Standard Selector(Standard x)
        {
            return new Data.School.Model.Standard
            {
                Description = x.Description,
                Id = x.StandardID,
                IsActive = x.IsActive,
                LowerGradeLevelRef = x.LowerGradeLevelID,
                Name = x.Name,
                ParentStandardRef = x.ParentStandardID,
                StandardSubjectRef = x.StandardSubjectID,
                UpperGradeLevelRef = x.UpperGradeLevelID,
                AcademicBenchmarkId = x.AcademicBenchmarkId
            };
        }

        protected override void InsertInternal(IList<Standard> entities)
        {
            var sts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardService.AddStandards(sts);
        }

        protected override void UpdateInternal(IList<Standard> entities)
        {
            var sts = entities.Select(Selector).ToList();
            ServiceLocatorSchool.StandardService.EditStandard(sts);
        }

        protected override void DeleteInternal(IList<Standard> entities)
        {
            var toDelete = entities.Select(x => new Data.School.Model.Standard
            {
                Id = x.StandardID
            }).ToList();
            ServiceLocatorSchool.StandardService.DeleteStandards(toDelete);
        }

        protected override void PrepareToDeleteInternal(IList<Standard> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}