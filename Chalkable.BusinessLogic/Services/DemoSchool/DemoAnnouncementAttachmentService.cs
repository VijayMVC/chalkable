using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoAnnouncementAttachmentService : DemoSchoolServiceBase, IAnnouncementAttachmentService
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
                                  PersonId = Context.PersonId
                              });
        }

        public Announcement AddAttachment(int announcementId, byte[] content, string name, string uuid)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            if (!CanAttach(ann))
                    throw new ChalkableSecurityException();

            var annAtt = new AnnouncementAttachment
            {
                AnnouncementRef = ann.Id,
                PersonRef = Context.PersonId.Value,
                AttachedDate = Context.NowSchoolTime,
                Name = name,
                Uuid = uuid,
                Order = ServiceLocator.AnnouncementService.GetNewAnnouncementItemOrder(ann)
            };


            Storage.AnnouncementAttachmentStorage.Add(annAtt);

            var atts = Storage.AnnouncementAttachmentStorage.GetList(Context.PersonId.Value, Context.Role.Id, name);

            
            ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, atts.Last().Id.ToString(), content);

            if (ann.State != AnnouncementState.Draft)
            {
                if (ann.IsOwner)
                    ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotification(announcementId);
                else
                    ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotificationToTeachers(announcementId, Context.PersonId.Value);
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
            return Storage.AnnouncementAttachmentStorage.GetPaginatedList(announcementId, Context.PersonId ?? 0, Context.Role.Id, start, count, needsAllAttachments).ToList();
        }

        public AnnouncementAttachment GetAttachmentById(int announcementAttachmentId)
        {
            return Storage.AnnouncementAttachmentStorage.GetById(announcementAttachmentId, Context.PersonId ?? 0, Context.Role.Id);
        }

        public AttachmentContentInfo GetAttachmentContent(int announcementAttachmentId)
        {
            var att = GetAttachmentById(announcementAttachmentId);
            var content = ServiceLocator.StorageBlobService.GetBlobContent(ATTACHMENT_CONTAINER_ADDRESS, announcementAttachmentId.ToString());
            return AttachmentContentInfo.Create(att, content);
        }
        public IList<AnnouncementAttachment> GetAttachments(string filter)
        {
            return Storage.AnnouncementAttachmentStorage.GetList(Context.PersonId ?? 0, Context.Role.Id, filter);
        }
        public static string GetAttachmentRelativeAddress()
        {
            return (new BlobHelper()).GetBlobsRelativeAddress(ATTACHMENT_CONTAINER_ADDRESS);
        }
    }
}
