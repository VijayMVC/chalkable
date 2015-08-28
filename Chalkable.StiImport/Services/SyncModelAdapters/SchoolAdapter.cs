using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class SchoolAdapter : SyncModelAdapter<School>
    {
        public SchoolAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<School> entities)
        {
            ServiceLocatorSchool.SchoolService.Add(entities.Select(x=>new Data.School.Model.School
            {
                Id = x.SchoolID,
                IsActive = x.IsActive,
                IsPrivate = x.IsPrivate,
                Name = x.Name,
                IsChalkableEnabled = x.IsChalkableEnabled,
                IsLEEnabled = x.IsLEEnabled,
                IsLESyncComplete = x.IsLESyncComplete
            }).ToList());
            
        }

        protected override void UpdateInternal(IList<School> entities)
        {
            var schools = entities.Select(school => new Data.School.Model.School
            {
                Id = school.SchoolID,
                IsActive = school.IsActive,
                IsPrivate = school.IsPrivate,
                Name = school.Name,
                IsChalkableEnabled = school.IsChalkableEnabled,
                IsLEEnabled = school.IsLEEnabled,
                IsLESyncComplete = school.IsLESyncComplete
            }).ToList();
            ServiceLocatorSchool.SchoolService.Edit(schools);
        }

        protected override void DeleteInternal(IList<School> entities)
        {
            var ids = entities.Select(x => new Data.School.Model.School { Id = x.SchoolID }).ToList();
            ServiceLocatorSchool.SchoolService.Delete(ids);
        }
    }
}