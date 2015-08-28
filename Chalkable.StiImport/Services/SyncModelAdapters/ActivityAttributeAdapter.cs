using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class ActivityAttributeAdapter : SyncModelAdapter<ActivityAttribute>
    {
        public ActivityAttributeAdapter(AdapterLocator locator) : base(locator)
        {
        }

        protected override void InsertInternal(IList<ActivityAttribute> entities)
        {
            var res = entities.Select(x => new Data.School.Model.AnnouncementAttribute
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
            }).ToList();
            ServiceLocatorSchool.AnnouncementAttributeService.Add(res);
        }

        protected override void UpdateInternal(IList<ActivityAttribute> entities)
        {
            var announcementAttributes = entities.Select(x => new Data.School.Model.AnnouncementAttribute
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
            }).ToList();
            ServiceLocatorSchool.AnnouncementAttributeService.Edit(announcementAttributes);
        }

        protected override void DeleteInternal(IList<ActivityAttribute> entities)
        {
            var annAttributes = entities.Select(x => new Data.School.Model.AnnouncementAttribute() { Id = x.ActivityAttributeID }).ToList();
            ServiceLocatorSchool.AnnouncementAttributeService.Delete(annAttributes);
        }
    }
}