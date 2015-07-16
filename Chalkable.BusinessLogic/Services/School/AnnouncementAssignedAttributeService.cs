using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
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


            if (attachment != null)
            {
                if (announcementType == AnnouncementType.Class && attachment.StiAttachment)
                {
                    ConnectorLocator.AttachmentConnector.DeleteAttachment(attachment.Id);
                }
                else
                {
                    RemoveAttributeAttachmentFromBlob(attachment.Id);//same id as attribute id
                }
            }

            using (var uow = Update())
            {
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
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

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
                var da = new DataAccessBase<AnnouncementAssignedAttribute>(uow);
                id = da.InsertWithEntityId(annAttribute);
                uow.Commit();
            }
            return GetAssignedAttributeById(id);
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
                var da = new DataAccessBase<AnnouncementAssignedAttribute>(uow);
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
            return DoRead(u => new DataAccessBase<AnnouncementAssignedAttribute>(u).GetAll(new AndQueryCondition{{AnnouncementAssignedAttribute.ID_FIELD, assignedAttributeId}}).First());
        }

        public AnnouncementAssignedAttribute GetAssignedAttributeByAttachmentId(int attributeAttachmentId)
        {
            return DoRead(u => new DataAccessBase<AnnouncementAssignedAttribute>(u).GetAll(new AndQueryCondition { { AnnouncementAssignedAttribute.SIS_ATTRIBUTE_ATTACHMENT_ID, attributeAttachmentId} }).First());
        }

        public AnnouncementAssignedAttribute RemoveAttributeAttachment(AnnouncementType announcementType, int announcementId, int attributeAttachmentId)
        {
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var attribute =
                ServiceLocator.AnnouncementAssignedAttributeService.GetAssignedAttributeByAttachmentId(attributeAttachmentId);
            var attachment = attribute.Attachment;


            if (attachment != null)
            {
                if (announcementType == AnnouncementType.Class && attachment.StiAttachment)
                {
                    ConnectorLocator.AttachmentConnector.DeleteAttachment(attachment.Id);
                }
                else
                {
                    RemoveAttributeAttachmentFromBlob(attachment.Id);//same id as attribute id
                }
            }
            
            using (var uow = Update())
            {
                attribute.Uuid = "";
                attribute.SisAttachmentName = "";
                attribute.SisAttachmentMimeType = "";
                attribute.SisAttributeAttachmentId = null;
                var da = new DataAccessBase<AnnouncementAssignedAttribute>(uow);
                da.Update(attribute);
                uow.Commit();
            }
            return GetAssignedAttributeById(attribute.Id);
            
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

        private AttributeAttachmentContentInfo GetAttributeAttachmentContent(AnnouncementAssignedAttribute attribute)
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

        public IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, int toAnnouncementId, UnitOfWork unitOfWork)
        {

            var da = new DataAccessBase<AnnouncementAssignedAttribute>(unitOfWork);
            var attributesForCopying = da.GetAll(new AndQueryCondition {{AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD, fromAnnouncementId}});
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

            var attribues = da.GetAll(new AndQueryCondition { {AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD, toAnnouncementId}})
                              .OrderByDescending(x => x.Id).Take(atributesInfo.Count).OrderBy(x => x.Id).ToList();
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
