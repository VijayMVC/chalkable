using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    class AnnouncementAttachmentQuery
    {
        public int? AnnouncementId { get; set; }
        public int? Start { get; set; }
        public int? Count { get; set; }
        public int? CallerId { get; set; }
        public int? RoleId { get; set; }
        public bool NeedsAllAttachments { get; set; }
        public string Filter { get; set; }
        public string Name { get; set; }
        public int? Id { get; set; }
    }

    public class DemoAnnouncementAttachmentStorage : BaseDemoIntStorage<AnnouncementAttachment>
    {
        public DemoAnnouncementAttachmentStorage()
            : base(x => x.Id, true)
        {
        }
    }

    public class DemoAnnouncementAttachmentService : DemoSchoolServiceBase, IAnnouncementAttachmentService
    {
        private DemoAnnouncementAttachmentStorage AnnouncementAttachmentStorage { get; set; }
        public DemoAnnouncementAttachmentService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
            AnnouncementAttachmentStorage = new DemoAnnouncementAttachmentStorage();
        }

        private bool CanAttach(Announcement ann)
        {
            return AnnouncementSecurity.CanModifyAnnouncement(ann, Context)
                || ((DemoClassService)ServiceLocator.ClassService).ClassPersonExists(ann.ClassRef, Context.PersonId);
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

            var attachment = AnnouncementAttachmentStorage.Add(annAtt);
            ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, attachment.Id.ToString(CultureInfo.InvariantCulture), content);

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
            var annAtt = AnnouncementAttachmentStorage.GetById(announcementAttachmentId);
            if (!AnnouncementSecurity.CanDeleteAttachment(annAtt, Context))
                throw new ChalkableSecurityException();

            AnnouncementAttachmentStorage.Delete(annAtt.Id);
            if (!annAtt.SisAttachmentId.HasValue)
                ServiceLocator.StorageBlobService.DeleteBlob(ATTACHMENT_CONTAINER_ADDRESS, annAtt.Id.ToString(CultureInfo.InvariantCulture));
        }

        private IList<AnnouncementAttachment> GetAttachmentsQuery(AnnouncementAttachmentQuery query)
        {
            query.CallerId = Context.PersonId;
            query.RoleId = Context.Role.Id;

            var attachments = AnnouncementAttachmentStorage.GetData().Select(x => x.Value);

            if (query.AnnouncementId.HasValue)
            {
                attachments = attachments.Where(x => x.AnnouncementRef == query.AnnouncementId);
            }

            if (CoreRoles.SUPER_ADMIN_ROLE.Id == query.RoleId)
            {
                return attachments.ToList();
            }
            if (CoreRoles.ADMIN_EDIT_ROLE.Id == query.RoleId || CoreRoles.ADMIN_GRADE_ROLE.Id == query.RoleId ||
                CoreRoles.ADMIN_VIEW_ROLE.Id == query.RoleId)
            {

                attachments = attachments.Where(x => x.PersonRef == query.CallerId);
                return attachments.ToList();
            }
            if (CoreRoles.TEACHER_ROLE.Id == query.RoleId)
            {

                var announcementRefs =
                    StorageLocator.AnnouncementRecipientStorage.GetAll()
                        .Where(x => x.RoleRef == query.RoleId || x.PersonRef == query.CallerId)
                        .Select(x => x.AnnouncementRef);

                attachments =
                    attachments.Where(
                        x =>
                        {
                            var announcement = StorageLocator.AnnouncementStorage.GetById(x.AnnouncementRef);
                            return x.PersonRef == query.CallerId || x.PersonRef == announcement.PrimaryTeacherRef || announcementRefs.Contains(x.AnnouncementRef);
                        });

            }
            if (CoreRoles.STUDENT_ROLE.Id == query.RoleId)
            {
                var classRefs = StorageLocator.ClassPersonStorage.GetAll().Where(x => x.PersonRef == query.CallerId).Select(x => x.ClassRef).ToList();

                attachments = attachments.Where(x =>
                {
                    var classRef = ServiceLocator.AnnouncementService.GetAnnouncementById(x.AnnouncementRef).ClassRef;
                    return (x.PersonRef == query.CallerId || classRefs.Contains(classRef));
                });
                return attachments.ToList();
            }


            if (!query.NeedsAllAttachments)
                attachments = attachments.Where(x => x.PersonRef == query.CallerId);

            if (!string.IsNullOrEmpty(query.Filter))
            {
                var filters =
                    query.Filter.Trim()
                        .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.ToLower())
                        .ToList();

                attachments = attachments.Where(x => filters.Contains(x.Name.ToLower()));
            }

            if (query.Start.HasValue)
                attachments = attachments.Skip(query.Start.Value);
            if (query.Count.HasValue)
                attachments = attachments.Take(query.Count.Value);
            return attachments.ToList();
        }

        public IList<AnnouncementAttachment> GetAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true)
        {
             var attachments = GetAttachmentsQuery(new AnnouncementAttachmentQuery
            {
                AnnouncementId = announcementId,
                Start = start,
                Count = count,
                NeedsAllAttachments = needsAllAttachments,
                CallerId = Context.PersonId ?? 0,
                RoleId = Context.Role.Id
            });
            return new PaginatedList<AnnouncementAttachment>(attachments, start / count, count, AnnouncementAttachmentStorage.GetData().Count);
        }

        public AnnouncementAttachment GetAttachmentById(int announcementAttachmentId)
        {
            return GetAttachmentsQuery(new AnnouncementAttachmentQuery
            {
                Id = announcementAttachmentId,
                CallerId = Context.PersonId ?? 0,
                RoleId = Context.Role.Id
            }).First();
        }

        public AttachmentContentInfo GetAttachmentContent(int announcementAttachmentId)
        {
            var att = GetAttachmentById(announcementAttachmentId);
            var content = ServiceLocator.StorageBlobService.GetBlobContent(ATTACHMENT_CONTAINER_ADDRESS, announcementAttachmentId.ToString());
            return AttachmentContentInfo.Create(att, content);
            
        }
        public IList<AnnouncementAttachment> GetAttachments(string filter)
        {
            return GetAttachmentsQuery(new AnnouncementAttachmentQuery
            {
                CallerId = Context.PersonId ?? 0,
                RoleId = Context.Role.Id,
                Name = filter
            });
        }

        private const string ATTACHMENT_CONTAINER_ADDRESS = "attachmentscontainer";

        public static string GetAttachmentRelativeAddress()
        {
            return (new BlobHelper()).GetBlobsRelativeAddress(ATTACHMENT_CONTAINER_ADDRESS);
        }

        public IList<AnnouncementAttachment> GetAll(int announcementId)
        {
            return GetAttachmentsQuery(new AnnouncementAttachmentQuery
            {
                AnnouncementId = announcementId
            });
        }

        public IList<AnnouncementAttachment> GetList(int userId, int roleId, string name)
        {
            return GetAttachmentsQuery(new AnnouncementAttachmentQuery
            {
                CallerId = userId,
                RoleId = roleId,
                Name = name
            });
        }

        public IList<AnnouncementAttachment> DuplicateFrom(int fromAnnouncementId, int toAnnouncementId)
        {
            var sourceAnnouncement = ServiceLocator.AnnouncementService.GetAnnouncementDetails(fromAnnouncementId);
            foreach (var sourceAnnouncementAttachment in sourceAnnouncement.AnnouncementAttachments)
            {
                var content = ServiceLocator.StorageBlobService.GetBlobContent(ATTACHMENT_CONTAINER_ADDRESS,
                    sourceAnnouncementAttachment.Id.ToString(CultureInfo.InvariantCulture));
                AddAttachment(toAnnouncementId, content, sourceAnnouncementAttachment.Name, "");
            }
            return GetAttachments(toAnnouncementId);
        }
    }
}
