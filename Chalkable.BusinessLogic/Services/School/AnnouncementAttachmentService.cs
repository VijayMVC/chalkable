using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAttachmentService
    {
        Announcement AddAttachment(Guid announcementId, byte[] content, string name, string uuid);
        void DeleteAttachment(Guid announcementAttachmentId);
        PaginatedList<AnnouncementAttachment> GetAttachments(Guid announcementId, int start, int count, bool needsAllAttachments = true);
        PaginatedList<AnnouncementAttachment> GetAttachments(Guid[] announcementIds, int start, int count, bool needsAllAttachments = true);
        IList<AnnouncementAttachment> GetAnnouncementsAttachments(IEnumerable<Guid> announcementIds);
        AnnouncementAttachment GetAttachmentById(Guid announcementAttachmentId);
    }

    public class AnnouncementAttachmentService : SchoolServiceBase, IAnnouncementAttachmentService
    {
        public AnnouncementAttachmentService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        private const string ATTACHMENT_CONTAINER_ADDRESS = "attachmentscontainer";

        public Announcement AddAttachment(Guid announcementId, byte[] content, string name, string uuid)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementId);
            if(!AnnouncementSecurity.CanModifyAnnouncement(ann, Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                var id = Guid.NewGuid();
                da.Create(new AnnouncementAttachment
                    {
                        Id = id,
                        AnnouncementRef = ann.Id,
                        PersonRef = Context.UserId,
                        AttachedDate = Context.NowSchoolTime,
                        Name = name,
                        Uuid = uuid,
                        Order = ServiceLocator.AnnouncementService.GetNewAnnouncementItemOrder(ann)
                    });
                ServiceLocator.StorageMonitorService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, id.ToString(), content);
                uow.Commit();
            }
            return ann;
        }

        public void DeleteAttachment(Guid announcementAttachmentId)
        {
            using (var uow = Update())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                var annAtt = da.GetById(announcementAttachmentId);
                da.Delete(annAtt);

               // ServiceLocator.StorageMonitorService.DeleteBlob(annAtt.Id.ToString()); Delete blob 
                uow.Commit();
            }
        }

        public PaginatedList<AnnouncementAttachment> GetAttachments(Guid announcementId, int start, int count, bool needsAllAttachments = true)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<AnnouncementAttachment> GetAttachments(Guid[] announcementIds, int start, int count, bool needsAllAttachments = true)
        {
            throw new NotImplementedException();
        }

        public IList<AnnouncementAttachment> GetAnnouncementsAttachments(IEnumerable<Guid> announcementIds)
        {
            throw new NotImplementedException();
        }

        public AnnouncementAttachment GetAttachmentById(Guid announcementAttachmentId)
        {
            throw new NotImplementedException();
        }
    }
}
