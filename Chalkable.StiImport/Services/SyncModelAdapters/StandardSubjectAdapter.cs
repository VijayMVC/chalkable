using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class StandardSubjectAdapter : SyncModelAdapter<StandardSubject>
    {
        public StandardSubjectAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<StandardSubject> entities)
        {
            var ss = entities.Select(x => new Data.School.Model.StandardSubject
            {
                AdoptionYear = x.AdoptionYear,
                Id = x.StandardSubjectID,
                Description = x.Description,
                IsActive = x.IsActive,
                Name = x.Name
            }).ToList();
            ServiceLocatorSchool.StandardService.AddStandardSubjects(ss);
        }

        protected override void UpdateInternal(IList<StandardSubject> entities)
        {
            var ss = entities.Select(x => new Data.School.Model.StandardSubject
            {
                AdoptionYear = x.AdoptionYear,
                Id = x.StandardSubjectID,
                Description = x.Description,
                IsActive = x.IsActive,
                Name = x.Name
            }).ToList();
            ServiceLocatorSchool.StandardService.EditStandardSubjects(ss);
        }

        protected override void DeleteInternal(IList<StandardSubject> entities)
        {
            var ids = entities.Select(x => x.StandardSubjectID).ToList();
            ServiceLocatorSchool.StandardService.DeleteStandardSubjects(ids);
        }
    }
}