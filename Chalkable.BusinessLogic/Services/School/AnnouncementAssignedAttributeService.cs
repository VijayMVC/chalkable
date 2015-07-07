using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAssignedAttributeService
    {
        void Add(IList<AnnouncementAssignedAttribute> announcementAttributes);
        AnnouncementDetails Edit(int announcementType, int announcementId, IList<AnnouncementAssignedAttribute> announcementAttributes);
        void Delete(IList<AnnouncementAssignedAttribute> announcementAttributes);
        AnnouncementDetails Delete(int announcementType, int announcementId, int assignedAttributeId);
        AnnouncementDetails Add(int announcementType, int announcementId, int attributeTypeId);
        AnnouncementDetails AddAttributeAttachment(int announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name, string uuid);
        AnnouncementAssignedAttribute GetAssignedAttribyteById(int assignedAttributeId);
        AnnouncementDetails RemoveAttributeAttachment(int announcementType, int announcementId, int assignedAttributeId);
        AttributeAttachmentContentInfo GetAttributeAttachmentContent(int assignedAttributeId, int announcementType);
    }

    public class AnnouncementAssignedAttributeService : SisConnectedService, IAnnouncementAssignedAttributeService
    {
        public AnnouncementAssignedAttributeService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public void Add(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            DoUpdate(u => new DataAccessBase<AnnouncementAssignedAttribute>(u).Insert(announcementAttributes));
        }

        public AnnouncementDetails Edit(int announcementType, int announcementId, IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var ann = ServiceLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            if (announcementAttributes != null)
            {
                using (var uow = Update())
                {
                    var da = new DataAccessBase<AnnouncementAssignedAttribute, int>(uow);

                    da.Update(announcementAttributes);
                    uow.Commit();
                    ann.AnnouncementAttributes = da.GetAll();
                }
            }
            
            return ann;
        }

        public void Delete(IList<AnnouncementAssignedAttribute> announcementAttributes)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            DoUpdate(u => new DataAccessBase<AnnouncementAssignedAttribute>(u).Delete(announcementAttributes));
        }

        public AnnouncementDetails Delete(int announcementType, int announcementId, int assignedAttributeId)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            var ann = ServiceLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            using (var uow = Update())
            {
                var da = new DataAccessBase<AnnouncementAssignedAttribute, int>(uow);


                ///delete sti attribute and sti attachment
                //var stiAttachment = ConnectorLocator.AttachmentConnector.UploadAttachment(name, bin).Last();

                //

                da.Delete(assignedAttributeId);


                ///delete attachments
                uow.Commit();
                ann.AnnouncementAttributes = da.GetAll();
            }
            return ann;
        }

        private bool CanAttach(Announcement ann)
        {
            return AnnouncementSecurity.CanModifyAnnouncement(ann, Context);
        }

        public AnnouncementDetails Add(int announcementType, int announcementId, int attributeTypeId)
        {
            var ann = ServiceLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementDetails(announcementId);
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
                var da = new DataAccessBase<AnnouncementAssignedAttribute,int>(uow);
                da.Insert(annAttribute);
                uow.Commit();
                ann.AnnouncementAttributes = da.GetAll();
            }
            return ann;
        }

        public AnnouncementDetails AddAttributeAttachment(int announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name,
            string uuid)
        {

            var typ = (AnnouncementType) announcementType;


            var ann = ServiceLocator.GetAnnouncementService(typ).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var assignedAttribute =
                ServiceLocator.AnnouncementAssignedAttributeService.GetAssignedAttribyteById(assignedAttributeId);



            if (assignedAttribute.SisAttributeAttachmentId.HasValue)
                throw new ChalkableSisException("You can't attach more than one file to an attribute");



            using (var uow = Update())
            {
                if (!CanAttach(ann))
                    throw new ChalkableSecurityException();


                if (typ == AnnouncementType.Class)
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
                    AddAttributeAttachmentToBlob(assignedAttributeId, bin);
                }

                

                var da = new DataAccessBase<AnnouncementAssignedAttribute>(uow);
                da.Update(assignedAttribute);
                uow.Commit();
                ann.AnnouncementAttributes = da.GetAll();
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

        public AnnouncementDetails RemoveAttributeAttachment(int announcementType, int announcementId, int assignedAttributeId)
        {
            var ann = ServiceLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var assignedAttribute =
                ServiceLocator.AnnouncementAssignedAttributeService.GetAssignedAttribyteById(assignedAttributeId);


            var typ = (AnnouncementType)announcementType;


            if (typ == AnnouncementType.Class)
            {
                if (assignedAttribute.SisAttributeAttachmentId.HasValue)
                {
                    if (!assignedAttribute.SisAttributeId.HasValue)
                    {
                        ConnectorLocator.AttachmentConnector.DeleteAttachment(
                            assignedAttribute.SisAttributeAttachmentId.Value);
                    }
                }
            }
            else
            {
                RemoveAttributeAttachmentFromBlob(assignedAttribute.Id);
            }
            using (var uow = Update())
            {
                assignedAttribute.Uuid = "";
                assignedAttribute.SisAttachmentName = "";
                assignedAttribute.SisAttachmentMimeType = "";
                assignedAttribute.SisAttributeAttachmentId = null;
                var da = new DataAccessBase<AnnouncementAssignedAttribute>(uow);
                da.Update(assignedAttribute);
                uow.Commit();
                ann.AnnouncementAttributes = da.GetAll();
            }
            return ann;
            
        }

    

        public AttributeAttachmentContentInfo GetAttributeAttachmentContent(int assignedAttributeId, int announcementType)
        {
            var attribute = GetAssignedAttribyteById(assignedAttributeId);
            AttributeAttachmentContentInfo result = null;
            var typ = (AnnouncementType)announcementType;

            if (typ == AnnouncementType.Class)
            {
                if (attribute.SisAttributeAttachmentId.HasValue)
                {
                    var content =
                        ConnectorLocator.AttachmentConnector.GetAttachmentContent(
                            attribute.SisAttributeAttachmentId.Value);
                    result = AttributeAttachmentContentInfo.Create(attribute.SisAttachmentName, content);
                }

            }
            else
                result = GetAttributeAttachmentFromBlob(attribute.SisAttachmentName, attribute.Id);
            return result;
        }
    }
}
