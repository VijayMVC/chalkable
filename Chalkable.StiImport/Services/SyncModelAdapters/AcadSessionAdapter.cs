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

        protected override void InsertInternal(IList<AcadSession> entities)
        {
            var schoolYears = entities.
                Select(x => new SchoolYear
                {
                    Description = x.Description,
                    EndDate = x.EndDate,
                    Id = x.AcadSessionID,
                    Name = x.Name,
                    SchoolRef = x.SchoolID,
                    StartDate = x.StartDate,
                    ArchiveDate = x.ArchiveDate
                }).ToList();
            SchoolLocator.SchoolYearService.Add(schoolYears);
        }

        protected override void UpdateInternal(IList<AcadSession> entities)
        {
            var schoolYears = entities.Select(x => new SchoolYear
            {
                Id = x.AcadSessionID,
                Description = x.Description,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Name = x.Name,
                SchoolRef = x.SchoolID,
                ArchiveDate = x.ArchiveDate
            }).ToList();
            SchoolLocator.SchoolYearService.Edit(schoolYears);
        }

        protected override void DeleteInternal(IList<AcadSession> entities)
        {
            var ids = entities.Select(x => x.AcadSessionID).ToList();
            SchoolLocator.SchoolYearService.Delete(ids);
        }
    }
}