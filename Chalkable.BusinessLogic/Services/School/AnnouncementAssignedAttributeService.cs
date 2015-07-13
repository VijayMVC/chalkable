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
        AnnouncementDetails Edit(AnnouncementType announcementType, int announcementId, IList<AssignedAttributeInputModel> attributes);
        AnnouncementDetails Delete(AnnouncementType announcementType, int announcementId, int assignedAttributeId);
        AnnouncementDetails Add(AnnouncementType announcementType, int announcementId, int attributeTypeId);
        AnnouncementDetails AddAttributeAttachment(AnnouncementType announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name, string uuid);
        AnnouncementAssignedAttribute GetAssignedAttribyteById(int assignedAttributeId);
        AnnouncementAssignedAttribute GetAssignedAttribyteByAttachmentId(int attributeAttachmentId);
        AnnouncementDetails RemoveAttributeAttachment(AnnouncementType announcementType, int announcementId, int attributeAttachmentId);
        AttributeAttachmentContentInfo GetAttributeAttachmentContent(int assignedAttributeId, AnnouncementType announcementType);
        IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, int toAnnouncementId);
    }

    public class AnnouncementAssignedAttributeService : SisConnectedService, IAnnouncementAssignedAttributeService
    {
        public AnnouncementAssignedAttributeService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public AnnouncementDetails Edit(AnnouncementType announcementType, int announcementId, IList<AssignedAttributeInputModel> attributes)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
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
                    ann.AnnouncementAttributes = da.GetAll(new AndQueryCondition { { AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD, announcementId } });
                }
            }
            
            return ann;
        }

        public AnnouncementDetails Delete(AnnouncementType announcementType, int announcementId, int assignedAttributeId)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var attribute =
                ServiceLocator.AnnouncementAssignedAttributeService.GetAssignedAttribyteById(assignedAttributeId);
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
                ann.AnnouncementAttributes = da.GetAll(new AndQueryCondition { { AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD, announcementId } });
            }
            return ann;
        }

        private bool CanAttach(Announcement ann)
        {
            return AnnouncementSecurity.CanModifyAnnouncement(ann, Context);
        }

        public AnnouncementDetails Add(AnnouncementType announcementType, int announcementId, int attributeTypeId)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var attributeType = ServiceLocator.AnnouncementAttributeService.GetAttributeById(attributeTypeId, true);

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
                da.Insert(annAttribute);
                uow.Commit();
                ann.AnnouncementAttributes = da.GetAll(new AndQueryCondition { { AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD, announcementId } });
            }
            return ann;
        }

        public AnnouncementDetails AddAttributeAttachment(AnnouncementType announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name,
            string uuid)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var assignedAttribute =
                ServiceLocator.AnnouncementAssignedAttributeService.GetAssignedAttribyteById(assignedAttributeId);

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
                ann.AnnouncementAttributes = da.GetAll(new AndQueryCondition { { AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD, announcementId } });
            }

           
            return ann;
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

        public AnnouncementAssignedAttribute GetAssignedAttribyteById(int assignedAttributeId)
        {
            return DoRead(u => new DataAccessBase<AnnouncementAssignedAttribute>(u).GetAll(new AndQueryCondition{{AnnouncementAssignedAttribute.ID_FIELD, assignedAttributeId}}).First());
        }

        public AnnouncementAssignedAttribute GetAssignedAttribyteByAttachmentId(int attributeAttachmentId)
        {
            return DoRead(u => new DataAccessBase<AnnouncementAssignedAttribute>(u).GetAll(new AndQueryCondition { { AnnouncementAssignedAttribute.SIS_ATTRIBUTE_ATTACHMENT_ID, attributeAttachmentId} }).First());
        }

        public AnnouncementDetails RemoveAttributeAttachment(AnnouncementType announcementType, int announcementId, int attributeAttachmentId)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var attribute =
                ServiceLocator.AnnouncementAssignedAttributeService.GetAssignedAttribyteByAttachmentId(attributeAttachmentId);
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
                ann.AnnouncementAttributes = da.GetAll(new AndQueryCondition { { AnnouncementAssignedAttribute.ANNOUNCEMENT_REF_FIELD, announcementId } });
            }
            return ann;
            
        }

        public AttributeAttachmentContentInfo GetAttributeAttachmentContent(int assignedAttributeId, AnnouncementType announcementType)
        {
            var attribute = GetAssignedAttribyteById(assignedAttributeId);
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
