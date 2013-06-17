﻿using System;
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
                var annAtt = GetAttachmentById(announcementAttachmentId);
                if(!AnnouncementSecurity.CanDeleteAttachment(annAtt, Context))
                    throw new ChalkableSecurityException();
              
                da.Delete(annAtt);
                ServiceLocator.StorageMonitorService.DeleteBlob(ATTACHMENT_CONTAINER_ADDRESS, annAtt.Id.ToString()); 
                uow.Commit();
            }
        }

        public PaginatedList<AnnouncementAttachment> GetAttachments(Guid announcementId, int start, int count, bool needsAllAttachments = true)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                return da.GetPaginatedList(announcementId, Context.UserId, Context.Role.Id, start, count, needsAllAttachments);
            }
        }

        public AnnouncementAttachment GetAttachmentById(Guid announcementAttachmentId)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                return da.GetById(announcementAttachmentId, Context.UserId, Context.Role.Id);
            }
        }
    }
}
