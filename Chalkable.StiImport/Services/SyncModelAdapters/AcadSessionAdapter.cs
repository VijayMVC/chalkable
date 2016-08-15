using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class AcadSessionAdapter : SyncModelAdapter<AcadSession>
    {
        public AcadSessionAdapter(AdapterLocator locator) : base(locator)
        {
        }

        private SchoolYear Selector(AcadSession x)
        {
            return new SchoolYear
            {
                Description = x.Description,
                EndDate = x.EndDate,
                Id = x.AcadSessionID,
                Name = x.Name,
                SchoolRef = x.SchoolID,
                StartDate = x.StartDate,
                ArchiveDate = x.ArchiveDate,
                AcadYear = x.AcadYear
            };
        }

        protected override void InsertInternal(IList<AcadSession> entities)
        {
            var schoolYears = entities.
                Select(Selector
                ).ToList();
            ServiceLocatorSchool.SchoolYearService.Add(schoolYears);
        }

        protected override void UpdateInternal(IList<AcadSession> entities)
        {
            var schoolYears = entities.Select(Selector).ToList();
            ServiceLocatorSchool.SchoolYearService.Edit(schoolYears);
        }

        protected override void DeleteInternal(IList<AcadSession> entities)
        {
            var ids = entities.Select(x => x.AcadSessionID).ToList();
            ServiceLocatorSchool.SchoolYearService.Delete(ids);
        }

        protected override void PrepareToDeleteInternal(IList<AcadSession> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}