using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class ActivityAttributeAdapter : SyncModelAdapter<ActivityAttribute>
    {
        public ActivityAttributeAdapter(AdapterLocator locator) : base(locator)
        {
        }
        private AnnouncementAttribute Selector(ActivityAttribute x)
        {
            return new AnnouncementAttribute
            {
                Id = x.ActivityAttributeID,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                NCESCode = x.NCESCode,
                SIFCode = x.SIFCode,
                StateCode = x.StateCode,
                IsActive = x.IsActive,
                IsSystem = x.IsSystem
            };
        }

        protected override void InsertInternal(IList<ActivityAttribute> entities)
        {
            var res = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AnnouncementAttributeService.Add(res);
        }
        
        protected override void UpdateInternal(IList<ActivityAttribute> entities)
        {
            var announcementAttributes = entities.Select(Selector).ToList();
            ServiceLocatorSchool.AnnouncementAttributeService.Edit(announcementAttributes);
        }

        protected override void DeleteInternal(IList<ActivityAttribute> entities)
        {
            var annAttributes = entities.Select(x => new Data.School.Model.AnnouncementAttribute { Id = x.ActivityAttributeID }).ToList();
            ServiceLocatorSchool.AnnouncementAttributeService.Delete(annAttributes);
        }

        protected override void PrepareToDeleteInternal(IList<ActivityAttribute> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}