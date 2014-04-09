using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoAnnouncementAttachmentService : DemoSisConnectedService, IAnnouncementAttachmentService
    {
        public DemoAnnouncementAttachmentService(IServiceLocatorSchool serviceLocator, DemoStorage storage)
            : base(serviceLocator, storage)
        {
        }

        private const string ATTACHMENT_CONTAINER_ADDRESS = "attachmentscontainer";

        private bool CanAttach(Announcement ann)
        {
            return AnnouncementSecurity.CanModifyAnnouncement(ann, Context)
                   || Storage.ClassPersonStorage.Exists(new ClassPersonQuery
                              {
                                  ClassId = ann.ClassRef,
                                  PersonId = Context.UserLocalId
                              });
        }

        public Announcement AddAttachment(int announcementId, byte[] content, string name, string uuid)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            if (!(Context.UserLocalId.HasValue && Context.SchoolId.HasValue))
                throw new UnassignedUserException();
            if (!CanAttach(ann))
                    throw new ChalkableSecurityException();

            var annAtt = new AnnouncementAttachment
            {
                AnnouncementRef = ann.Id,
                PersonRef = Context.UserLocalId.Value,
                AttachedDate = Context.NowSchoolTime,
                Name = name,
                Uuid = uuid,
                Order = ServiceLocator.AnnouncementService.GetNewAnnouncementItemOrder(ann)
            };


            Storage.AnnouncementAttachmentStorage.Add(annAtt);

            var atts = Storage.AnnouncementAttachmentStorage.GetList(Context.UserLocalId.Value, Context.Role.Id, name);

            if (CoreRoles.TEACHER_ROLE != Context.Role)
                ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, atts.Last().Id.ToString(), content);

            if (ann.State != AnnouncementState.Draft)
            {
                if (Context.UserLocalId == ann.PersonRef)
                    ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotification(announcementId);
                else
                    ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotificationToPerson(announcementId, Context.UserLocalId.Value);
            }
            return ann;
        }

        public void DeleteAttachment(int announcementAttachmentId)
        {
            var annAtt = Storage.AnnouncementAttachmentStorage.GetById(announcementAttachmentId);
            if (!AnnouncementSecurity.CanDeleteAttachment(annAtt, Context))
                throw new ChalkableSecurityException();

            Storage.AnnouncementAttachmentStorage.Delete(annAtt.Id);
            if (!annAtt.SisAttachmentId.HasValue)
                ServiceLocator.StorageBlobService.DeleteBlob(ATTACHMENT_CONTAINER_ADDRESS, annAtt.Id.ToString());
        }

        public IList<AnnouncementAttachment> GetAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true)
        {
            return Storage.AnnouncementAttachmentStorage.GetPaginatedList(announcementId, Context.UserLocalId ?? 0, Context.Role.Id, start, count, needsAllAttachments).ToList();
        }

        public AnnouncementAttachment GetAttachmentById(int announcementAttachmentId)
        {
            return Storage.AnnouncementAttachmentStorage.GetById(announcementAttachmentId, Context.UserLocalId ?? 0, Context.Role.Id);
        }

        public AttachmentContentInfo GetAttachmentContent(int announcementAttachmentId)
        {
            var att = GetAttachmentById(announcementAttachmentId);
            var content = ServiceLocator.StorageBlobService.GetBlobContent(ATTACHMENT_CONTAINER_ADDRESS, announcementAttachmentId.ToString());
            return AttachmentContentInfo.Create(att, content);
        }
        public IList<AnnouncementAttachment> GetAttachments(string filter)
        {
            return Storage.AnnouncementAttachmentStorage.GetList(Context.UserLocalId ?? 0, Context.Role.Id, filter);
        }
        public static string GetAttachmentRelativeAddress()
        {
            return (new BlobHelper()).GetBlobsRelativeAddress(ATTACHMENT_CONTAINER_ADDRESS);
        }
    }
}
