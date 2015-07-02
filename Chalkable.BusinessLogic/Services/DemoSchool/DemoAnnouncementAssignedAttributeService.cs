using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoAnnouncementAssignedAttributeService : SchoolServiceBase, IAnnouncementAssignedAttributeService
    {
        public DemoAnnouncementAssignedAttributeService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public void Add(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
           
        }

        public void Edit(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
           
        }

        public void Delete(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
           
        }

        public AnnouncementDetails Delete(int announcementId, int assignedAttributeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails Add(int announcementId, int attributeTypeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails Add(int announcementAttributes)
        {
            throw new System.NotImplementedException();
        }
    }
}
