using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAssignedAttributeService
    {
        void Edit(AnnouncementType announcementType, int announcementId, IList<AssignedAttributeInputModel> attributes);
        void Delete(AnnouncementType announcementType, int announcementId, int assignedAttributeId);
        AnnouncementAssignedAttribute Add(AnnouncementType announcementType, int announcementId, int attributeTypeId);
        AnnouncementAssignedAttribute AddAttributeAttachment(AnnouncementType announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name, string uuid);
        AnnouncementAssignedAttribute GetAssignedAttributeById(int assignedAttributeId);
        AnnouncementAssignedAttribute GetAssignedAttributeByAttachmentId(int attributeAttachmentId);
        AnnouncementAssignedAttribute RemoveAttributeAttachment(AnnouncementType announcementType, int announcementId, int attributeAttachmentId);
        AttributeAttachmentContentInfo GetAttributeAttachmentContent(int assignedAttributeId, AnnouncementType announcementType);
        IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, int toAnnouncementId);
    }

    public class AnnouncementAssignedAttributeService : SisConnectedService, IAnnouncementAssignedAttributeService
    {
        public AnnouncementAssignedAttributeService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public void Edit(AnnouncementType announcementType, int announcementId, IList<AssignedAttributeInputModel> attributes)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var announcementAttributes = AnnouncementAssignedAttribute.Create(attributes);

            if (attributes != null)
            {
                using (var uow = Update())
                {
                    var da = new DataAccessBase<AnnouncementAssignedAttribute>(uow);
                    da.Update(announcementAttributes);
                    uow.Commit();
                }
            }
        }

        public void Delete(AnnouncementType announcementType, int announcementId, int assignedAttributeId)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var attribute =
                ServiceLocator.AnnouncementAssignedAttributeService.GetAssignedAttributeById(assignedAttributeId);
            var attachment = attribute.Attachment;

            using (var uow = Update())
            {
                if (announcementType == AnnouncementType.Class)
                {
                    var announcement = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);
                    if (announcement.SisActivityId.HasValue && attribute.SisActivityAssignedAttributeId.HasValue)
                    {
                        ConnectorLocator.ActivityAssignedAttributeConnector.Delete(announcement.SisActivityId.Value, attribute.SisActivityAssignedAttributeId.Value);
                    }
                    if (attachment != null)
                    {
                        RemoveAttributeAttachment(announcementType, announcementId, attachment.Id);
                    }
                }
                var da = new DataAccessBase<AnnouncementAssignedAttribute, int>(uow);
                da.Delete(assignedAttributeId);
                uow.Commit();
            }
        }

        private bool CanAttach(Announcement ann)
        {
            return AnnouncementSecurity.CanModifyAnnouncement(ann, Context);
        }

        public AnnouncementAssignedAttribute Add(AnnouncementType announcementType, int announcementId, int attributeTypeId)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(announcementId);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            var attributeType = ServiceLocator.AnnouncementAttributeService.GetAttributeById(attributeTypeId, true);

            var id = -1;

            using (var uow = Update())
            {
                if (!CanAttach(ann))
                    throw new ChalkableSecurityException();

                var annAttribute = new AnnouncementAssignedAttribute
                {
                    AnnouncementRef = ann.Id,
                    AttributeTypeId = attributeType.Id,
                    Name = attributeType.Name,
                    Text = " ",
                    VisibleForStudents = true
                };
                var da = new AnnouncementAssignedAttributeDataAccess(uow);
                id = da.InsertWithEntityId(annAttribute);
                uow.Commit();
                return da.GetById(id);
            }
        }

        public AnnouncementAssignedAttribute AddAttributeAttachment(AnnouncementType announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name,
            string uuid)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var assignedAttribute =
                ServiceLocator.AnnouncementAssignedAttributeService.GetAssignedAttributeById(assignedAttributeId);

            if (assignedAttribute.Attachment != null && assignedAttribute.Attachment.Id > 0)
            {
                throw new ChalkableSisException("You can't attach more than one file to an attribute");
            }
                
            using (var uow = Update())
            {
                if (!CanAttach(ann))
                    throw new ChalkableSecurityException();

                if (announcementType == AnnouncementType.Class)
                {
                    var stiAttachment = ConnectorLocator.AttachmentConnector.UploadAttachment(name, bin).Last();
                    assignedAttribute.Uuid = stiAttachment.CrocoDocId != null ? stiAttachment.CrocoDocId.ToString() : "";
                    assignedAttribute.SisAttachmentName = stiAttachment.Name;
                    assignedAttribute.SisAttachmentMimeType = stiAttachment.MimeType;
                    assignedAttribute.SisAttributeAttachmentId = stiAttachment.AttachmentId;
                }
                else
                {
                    assignedAttribute.Uuid = uuid;
                    assignedAttribute.SisAttachmentName = name;
                    assignedAttribute.SisAttributeAttachmentId = assignedAttributeId;
                    AddAttributeAttachmentToBlob(assignedAttributeId, bin);
                }
                var da = new AnnouncementAssignedAttributeDataAccess(uow);
                da.Update(assignedAttribute);
                uow.Commit();
            }


            return GetAssignedAttributeById(assignedAttributeId);
        }

        private const string ATTACHMENT_CONTAINER_ADDRESS = "attachmentscontainer";

        public void AddAttributeAttachmentToBlob(int assignedAttributeId, byte[] content)
        {
            ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, GenerateKeyForBlob(assignedAttributeId), content);
        }

        public void RemoveAttributeAttachmentFromBlob(int assignedAttributeid)
        {
            ServiceLocator.StorageBlobService.DeleteBlob(ATTACHMENT_CONTAINER_ADDRESS, GenerateKeyForBlob(assignedAttributeid));
        }

        public AttributeAttachmentContentInfo GetAttributeAttachmentFromBlob(string attachmentName, int assignedAttributeid)
        {
            var content = ServiceLocator.StorageBlobService.GetBlobContent(ATTACHMENT_CONTAINER_ADDRESS, GenerateKeyForBlob(assignedAttributeid));
            return AttributeAttachmentContentInfo.Create(attachmentName, content);
        }

        private string GenerateKeyForBlob(int assignedAttributeId)
        {
            var res = assignedAttributeId.ToString(CultureInfo.InvariantCulture);
            if (Context.DistrictId.HasValue)
                return string.Format("{0}_{1}", res, Context.DistrictId.Value);
            return res;
        }

        public AnnouncementAssignedAttribute GetAssignedAttributeById(int assignedAttributeId)
        {
            return DoRead(u => new AnnouncementAssignedAttributeDataAccess(u).GetById(assignedAttributeId));
        }

        public AnnouncementAssignedAttribute GetAssignedAttributeByAttachmentId(int attributeAttachmentId)
        {
            return DoRead(u => new AnnouncementAssignedAttributeDataAccess(u).GetByAttachmentId(attributeAttachmentId));
        }

        public AnnouncementAssignedAttribute RemoveAttributeAttachment(AnnouncementType announcementType, int announcementId, int attributeAttachmentId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            var attribute = ServiceLocator.AnnouncementAssignedAttributeService.GetAssignedAttributeByAttachmentId(attributeAttachmentId);
            var attachment = attribute.Attachment;
            
            using (var uow = Update())
            {
                attribute.Uuid = "";
                attribute.SisAttachmentName = "";
                attribute.SisAttachmentMimeType = "";
                attribute.SisAttributeAttachmentId = null;
                var da = new AnnouncementAssignedAttributeDataAccess(uow);
                if (attachment != null)
                {
                    if (announcementType == AnnouncementType.Class && attachment.StiAttachment)
                    {
                        if (attribute.SisActivityAssignedAttributeId.HasValue)
                        {
                            var classAnn = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);
                            var activityAssignedAttribute = ConnectorLocator.ActivityAssignedAttributeConnector.GetAttribute(classAnn.SisActivityId.Value, attribute.SisActivityAssignedAttributeId.Value);
                            activityAssignedAttribute.Attachment = null;
                            ConnectorLocator.ActivityAssignedAttributeConnector.Update(classAnn.SisActivityId.Value, attribute.SisActivityAssignedAttributeId.Value, activityAssignedAttribute);    
                        }
                        ConnectorLocator.AttachmentConnector.DeleteAttachment(attachment.Id);
                    }
                    else
                    {
                        RemoveAttributeAttachmentFromBlob(attachment.Id);//same id as attribute id
                    }
                }
                
                da.Update(attribute);
                uow.Commit();
                return da.GetById(attribute.Id);
            }
        }

        public AttributeAttachmentContentInfo GetAttributeAttachmentContent(int assignedAttributeId, AnnouncementType announcementType)
        {
            var attribute = GetAssignedAttributeById(assignedAttributeId);
            return GetAttributeAttachmentContent(attribute);
        }

        private string UploadToCrocodoc(string name, byte[] content)
        {
            if (ServiceLocator.CrocodocService.IsDocument(name))
                return ServiceLocator.CrocodocService.UploadDocument(name, content).uuid;
            return null;
        }

        public AttributeAttachmentContentInfo GetAttributeAttachmentContent(AnnouncementAssignedAttribute attribute)
        {
            AttributeAttachmentContentInfo result = null;
            if (attribute.Attachment != null)
            {
                if (attribute.SisActivityAssignedAttributeId.HasValue)
                {
                    var content = ConnectorLocator.AttachmentConnector.GetAttachmentContent(attribute.Attachment.Id);
                    result = AttributeAttachmentContentInfo.Create(attribute.Attachment.Name, content);
                }
                else result = GetAttributeAttachmentFromBlob(attribute.Attachment.Name, attribute.Attachment.Id);
            }
            return result;
        }

        public void AddMissingSisAttributes(IList<AnnouncementAssignedAttribute> attributes, UnitOfWork u)
        {
            var missingAttributes = attributes.Where(a => a.Id <= 0).ToList();
            foreach (var attribute in missingAttributes)
            {
                if (attribute.Attachment != null && ServiceLocator.CrocodocService.IsDocument(attribute.Attachment.Name))
                {
                    var content = ConnectorLocator.AttachmentConnector.GetAttachmentContent(attribute.Attachment.Id);
                    attribute.Uuid = UploadToCrocodoc(attribute.Attachment.Name, content);
                }
            }
            new AnnouncementAssignedAttributeDataAccess(u).Insert(missingAttributes);
        }

        public void UploadMissingAttributeAttachments(IList<AnnouncementAssignedAttribute> attributes, UnitOfWork u)
        {
            var attributesForUpdate = attributes.Where(x => x.Id > 0 && x.Attachment != null && string.IsNullOrEmpty(x.Uuid) && ServiceLocator.CrocodocService.IsDocument(x.Attachment.Name)).ToList();
            foreach (var attribute in attributesForUpdate)
            {
                var content = ConnectorLocator.AttachmentConnector.GetAttachmentContent(attribute.Attachment.Id);
                attribute.Uuid = UploadToCrocodoc(attribute.Attachment.Name, content);
            }
            new AnnouncementAssignedAttributeDataAccess(u).Update(attributesForUpdate);
        }


        public IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, int toAnnouncementId, UnitOfWork unitOfWork)
        {
            var da = new AnnouncementAssignedAttributeDataAccess(unitOfWork);
            var attributesForCopying = da.GetListByAnntId(fromAnnouncementId);
            attributesForCopying = attributesForCopying.Where(x => !x.SisActivityAssignedAttributeId.HasValue).ToList();
            var attributsWithContents = attributesForCopying.Select(x => AnnouncementAssignedAttributeInfo.Create(x, GetAttributeAttachmentContent(x)));

            var atributesInfo = new List<AnnouncementAssignedAttributeInfo>();
            foreach (var attributeContent in attributsWithContents)
            {
                var attribute = new AnnouncementAssignedAttribute
                    {
                        AnnouncementRef = toAnnouncementId,
                        AttributeTypeId = attributeContent.Attribute.AttributeTypeId,
                        Name = attributeContent.Attribute.Name,
                        Text = attributeContent.Attribute.Text,
                        VisibleForStudents = attributeContent.Attribute.VisibleForStudents,
                    };
                if (attributeContent.AttachmentContentInfo != null)
                    attribute.Uuid = UploadToCrocodoc(attributeContent.Attribute.Name, attributeContent.AttachmentContentInfo.Content);
                atributesInfo.Add(AnnouncementAssignedAttributeInfo.Create(attribute, attributeContent.AttachmentContentInfo));
            }
            da.Insert(atributesInfo.Select(x => x.Attribute).ToList());

            var attribues = da.GetLastListByAnnId(toAnnouncementId, atributesInfo.Count);
            for (var i = 0; i < attribues.Count; i++)
                if(atributesInfo[i].AttachmentContentInfo != null)
                    AddAttributeAttachmentToBlob(attribues[i].Id, atributesInfo[i].AttachmentContentInfo.Content);
            return attribues;
        }

        public IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, int toAnnouncementId)
        {
            using (var u = Update())
            {
                var res = CopyNonStiAttributes(fromAnnouncementId, toAnnouncementId, u);
                u.Commit();
                return res;
            }
        }
    }
}
