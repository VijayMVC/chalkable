using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAttachmentService
    {
        Announcement AddAttachment(int announcementId, byte[] content, string name, string uuid);
        void DeleteAttachment(int announcementAttachmentId);
        PaginatedList<AnnouncementAttachment> GetAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true);
        IList<AnnouncementAttachment> GetAttachments(string filter);
        AnnouncementAttachment GetAttachmentById(int announcementAttachmentId);
        AttachmentContentInfo GetAttachmentContent(int announcementAttachmentId);
    }

    public class AnnouncementAttachmentService : SchoolServiceBase, IAnnouncementAttachmentService
    {
        public AnnouncementAttachmentService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        private const string ATTACHMENT_CONTAINER_ADDRESS = "attachmentscontainer";

        public Announcement AddAttachment(int announcementId, byte[] content, string name, string uuid)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            if (!(Context.LocalId.HasValue && Context.SchoolId.HasValue))
                throw new UnassignedUserException();
            if(!AnnouncementSecurity.CanAttach(ann, Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                da.Insert(new AnnouncementAttachment
                    {
                        AnnouncementRef = ann.Id,
                        PersonRef = Context.LocalId.Value,
                        AttachedDate = Context.NowSchoolTime,
                        Name = name,
                        Uuid = uuid,
                        Order = ServiceLocator.AnnouncementService.GetNewAnnouncementItemOrder(ann)
                    });
                var atts =  da.GetList(Context.LocalId.Value, Context.Role.Id, name);
                ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, atts.Last().Id.ToString(), content);
                uow.Commit();
                if (ann.State != AnnouncementState.Draft)
                {
                    if (Context.LocalId == ann.PersonRef)
                    {
                        ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotification(announcementId);
                    }
                    else
                    {
                        ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotificationToPerson(announcementId, Context.LocalId.Value);
                    }
                }

            }
            return ann;
        }

        public void DeleteAttachment(int announcementAttachmentId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                var annAtt = GetAttachmentById(announcementAttachmentId);
                if(!AnnouncementSecurity.CanDeleteAttachment(annAtt, Context))
                    throw new ChalkableSecurityException();
              
                da.Delete(annAtt.Id);
                ServiceLocator.StorageBlobService.DeleteBlob(ATTACHMENT_CONTAINER_ADDRESS, annAtt.Id.ToString()); 
                uow.Commit();
            }
        }

        public PaginatedList<AnnouncementAttachment> GetAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                return da.GetPaginatedList(announcementId, Context.LocalId ?? 0, Context.Role.Id, start, count, needsAllAttachments);
            }
        }

        public AnnouncementAttachment GetAttachmentById(int announcementAttachmentId)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                return da.GetById(announcementAttachmentId, Context.LocalId ?? 0, Context.Role.Id);
            }
        }

        public AttachmentContentInfo GetAttachmentContent(int announcementAttachmentId)
        {
            var att = GetAttachmentById(announcementAttachmentId);
            var content =  ServiceLocator.StorageBlobService.GetBlobContent(ATTACHMENT_CONTAINER_ADDRESS, announcementAttachmentId.ToString());
            return AttachmentContentInfo.Create(att, content);
        }
        public IList<AnnouncementAttachment> GetAttachments(string filter)
        {
            using (var uow = Read())
            {
                return new AnnouncementAttachmentDataAccess(uow).GetList(Context.LocalId ?? 0, Context.Role.Id, filter);
            }
        }
        public static string GetAttachmentRelativeAddress()
        {
            return (new BlobHelper()).GetBlobsRelativeAddress(ATTACHMENT_CONTAINER_ADDRESS);
        }
    }
}
