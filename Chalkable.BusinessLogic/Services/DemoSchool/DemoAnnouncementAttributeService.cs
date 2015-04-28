using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAnnouncementAttributeService : DemoSchoolServiceBase, IAnnouncementAttributeService
    {
        public DemoAnnouncementAttributeService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
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
            var res = Storage.AnnouncementAttributeStorage.GetAll();
            if (activeOnly.HasValue && activeOnly.Value)
                res = res.Where(x => x.IsActive).ToList();
            return res;
        }
    }
}
