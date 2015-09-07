﻿using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Data.School.Model.Announcements.Sis;
using Chalkable.Data.School.Model.Chlk;

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

        public void Edit(AnnouncementType announcementType, int announcementId, IList<AssignedAttributeInputModel> attributes)
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

        public void Delete(AnnouncementType announcementType, int announcementId, int assignedAttributeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementAssignedAttribute Add(AnnouncementType announcementType, int announcementId, int attributeTypeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementAssignedAttribute AddAttributeAttachment(AnnouncementType announcementType, int announcementId,
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

        public AnnouncementAssignedAttribute GetAssignedAttributeById(int assignedAttributeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementAssignedAttribute GetAssignedAttributeByAttachmentId(int attributeAttachmentId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementAssignedAttribute RemoveAttributeAttachment(AnnouncementType announcementType, int announcementId,
            int assignedAttributeId)
        {
            throw new System.NotImplementedException();
        }

        public AttributeAttachmentContentInfo GetAttributeAttachmentContent(int assignedAttributeId, AnnouncementType announcementType)
        {
            throw new System.NotImplementedException();
        }

        public IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, int toAnnouncementId)
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


        public void Delete(int announcementAssignedAttributeId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementAssignedAttribute UploadAttachment(AnnouncementType announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementAssignedAttribute AddAttachment(AnnouncementType announcementType, int announcementId, int assignedAttributeId, int attachmentId)
        {
            throw new System.NotImplementedException();
        }

        public AnnouncementAssignedAttribute RemoveAttachment(int announcementAssignedAttributeId)
        {
            throw new System.NotImplementedException();
        }
    }
}
