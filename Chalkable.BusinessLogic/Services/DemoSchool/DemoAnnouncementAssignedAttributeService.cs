using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

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

        public AnnouncementDetails Edit(AnnouncementType announcementType, int announcementId, IList<AssignedAttributeInputModel> attributes)
        {
            throw new System.NotImplementedException();
        }


        public AnnouncementDetails Edit(AnnouncementType announcementType, int announcementId, IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails Edit(int announcementType, int announcementId, IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
            throw new System.NotImplementedException();
        }

        public void Edit(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
           
        }

        public void Delete(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
           
        }

        public AnnouncementDetails Delete(AnnouncementType announcementType, int announcementId, int assignedAttributeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails Add(AnnouncementType announcementType, int announcementId, int attributeTypeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails AddAttributeAttachment(AnnouncementType announcementType, int announcementId,
            int assignedAttributeId, byte[] bin, string name, string uuid)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails Delete(int announcementType, int announcementId, int assignedAttributeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails Add(int announcementType, int announcementId, int attributeTypeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails AddAttributeAttachment(int announcementType, int announcementId, int assignedAttributeId, byte[] bin,
            string name, string uuid)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementAssignedAttribute GetAssignedAttribyteById(int assignedAttributeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementAssignedAttribute GetAssignedAttribyteByAttachmentId(int attributeAttachmentId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails RemoveAttributeAttachment(AnnouncementType announcementType, int announcementId,
            int assignedAttributeId)
        {
            throw new System.NotImplementedException();
        }

        public AttributeAttachmentContentInfo GetAttributeAttachmentContent(int assignedAttributeId, AnnouncementType announcementType)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails RemoveAttributeAttachment(int announcementType, int announcementId, int assignedAttributeId)
        {
            throw new System.NotImplementedException();
        }

        public AttributeAttachmentContentInfo GetAttributeAttachmentContent(int assignedAttributeId, int announcementType)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementDetails AddAttributeAttachment(int announcementType, int announcementId, byte[] bin, string name,
            string uuid)
        {
            throw new System.NotImplementedException();
        }
    }
}
