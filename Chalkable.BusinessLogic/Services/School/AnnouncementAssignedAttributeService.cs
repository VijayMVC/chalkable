using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors;
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
            var ann = ServiceLocator.GetAnnouncementService((AnnouncementType)announcementType).GetAnnouncementDetails(announcementId);
            if (!(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue))
                throw new UnassignedUserException();

            var assignedAttribute =
                ServiceLocator.AnnouncementAssignedAttributeService.GetAssignedAttribyteById(assignedAttributeId);



            if (assignedAttribute.SisAttributeAttachmentId.HasValue)
                throw new ChalkableSisException("You can't attach more than one file to an attribute");

            var stiAttachment = ConnectorLocator.AttachmentConnector.UploadAttachment(name, bin).Last();

            using (var uow = Update())
            {
                if (!CanAttach(ann))
                    throw new ChalkableSecurityException();

                assignedAttribute.Uuid = stiAttachment.CrocoDocId != null ? stiAttachment.CrocoDocId.ToString() : "";
                assignedAttribute.SisAttachmentName = stiAttachment.Name;
                assignedAttribute.SisAttachmentMimeType = stiAttachment.MimeType;
                assignedAttribute.SisAttributeAttachmentId = stiAttachment.AttachmentId;

                var da = new DataAccessBase<AnnouncementAssignedAttribute>(uow);
                da.Update(assignedAttribute);
                uow.Commit();
                ann.AnnouncementAttributes = da.GetAll();
            }
            return ann;
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


            //check if it's sis attribute then 
            if (assignedAttribute.SisAttributeAttachmentId.HasValue)
            {
                //if local just remove attachment
                //delete attribute 

                if (!assignedAttribute.SisAttributeId.HasValue)
                {
                    ConnectorLocator.AttachmentConnector.DeleteAttachment(
                        assignedAttribute.SisAttributeAttachmentId.Value);
                }
                else
                {
                    //update attribute in inow
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

                
            }

            return ann;
            
        }
    }
}
