using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAnnouncementAttributeStorage : BaseDemoIntStorage<AnnouncementAttribute>
    {
        public DemoAnnouncementAttributeStorage()
            : base(x => x.Id, true)
        {
        }

    }

    public class DemoAnnouncementAttributeService : DemoSchoolServiceBase, IAnnouncementAttributeService
    {
        private DemoAnnouncementAttributeStorage AnnouncementAttributeStorage { get; set; }
        public DemoAnnouncementAttributeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            AnnouncementAttributeStorage = new DemoAnnouncementAttributeStorage();
        }

        public void Add(IList<AnnouncementAttribute> announcementAttributes)
        {
            AnnouncementAttributeStorage.Add(announcementAttributes);
        }

        public void Edit(IList<AnnouncementAttribute> announcementAttributes)
        {
            AnnouncementAttributeStorage.Update(announcementAttributes);
        }

        public void Delete(IList<AnnouncementAttribute> announcementAttributes)
        {
            AnnouncementAttributeStorage.Delete(announcementAttributes);
        }

        public IList<AnnouncementAttribute> GetList(bool? activeOnly)
        {
            var res = AnnouncementAttributeStorage.GetAll();
            if (activeOnly.HasValue && activeOnly.Value)
                res = res.Where(x => x.IsActive).ToList();
            return res;
        }

        public AnnouncementAttribute GetAttributeById(int attributeId, bool? activeOnly)
        {
            throw new NotImplementedException();
        }
    }
}
