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
            throw new NotImplementedException();
        }

        public void Edit(IList<AnnouncementAttribute> announcementAttributes)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<AnnouncementAttribute> announcementAttributes)
        {
            throw new NotImplementedException();
        }

        public IList<AnnouncementAttribute> GetList(bool? activeOnly)
        {
            var res = AnnouncementAttributeStorage.GetAll();
            if (activeOnly.HasValue && activeOnly.Value)
                res = res.Where(x => x.IsActive).ToList();
            return res;
        }
    }
}
