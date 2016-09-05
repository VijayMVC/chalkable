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
        private Data.School.Model.School Selector(School x)
        {
            return new Data.School.Model.School
            {
                Id = x.SchoolID,
                IsActive = x.IsActive,
                IsPrivate = x.IsPrivate,
                Name = x.Name,
                IsChalkableEnabled = x.IsChalkableEnabled,
                IsLEEnabled = x.IsLEEnabled,
                IsLESyncComplete = x.IsLESyncComplete
            };
        }

        protected override void InsertInternal(IList<School> entities)
        {
            ServiceLocatorSchool.SchoolService.Add(entities.Select(Selector).ToList());
            
        }

        protected override void UpdateInternal(IList<School> entities)
        {
            var schools = entities.Select(Selector).ToList();
            ServiceLocatorSchool.SchoolService.Edit(schools);
        }

        protected override void DeleteInternal(IList<School> entities)
        {
            var ids = entities.Select(x => new Data.School.Model.School { Id = x.SchoolID }).ToList();
            ServiceLocatorSchool.SchoolService.Delete(ids);
        }

        protected override void PrepareToDeleteInternal(IList<School> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}