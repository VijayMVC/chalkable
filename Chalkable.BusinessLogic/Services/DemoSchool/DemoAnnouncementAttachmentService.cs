using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

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

        private bool CanAttach(AnnouncementComplex ann)
        {
            //TODO: rewrite this method later
            int? classId = null;
            if (ann.ClassAnnouncementData != null) 
                classId = ann.ClassAnnouncementData.ClassRef;
            if (ann.LessonPlanData != null)
                classId = ann.LessonPlanData.ClassRef;
            return AnnouncementSecurity.CanModifyAnnouncement(ann, Context)
                || (classId.HasValue && ((DemoClassService)ServiceLocator.ClassService).ClassPersonExists(classId.Value, Context.PersonId));
        }

        private string UploadToCrocodoc(string name, byte[] content)
        {
            if (ServiceLocator.CrocodocService.IsDocument(name))
                return ServiceLocator.CrocodocService.UploadDocument(name, content).uuid;
            return null;
        }

        public IList<AnnouncementAttachment> CopyAttachments(int fromAnnouncementId, IList<int> attachmentOwnersIds, int toAnnouncementId)
        {
            throw new NotImplementedException();
        }

        public AnnouncementAttachment UploadAttachment(int announcementId, AnnouncementTypeEnum type, byte[] content, string name)
        {
            throw new NotImplementedException();
        }

        public Announcement Add(int announcementId, AnnouncementTypeEnum type, int attachmentId)
        {
            throw new NotImplementedException();
        }

        public Announcement AddAttachment(int announcementId, AnnouncementTypeEnum annType, byte[] content, string name)
        {
            var ann = ServiceLocator.GetAnnouncementService(annType).GetAnnouncementDetails(announcementId);
            if (!Context.PersonId.HasValue)
                throw new UnassignedUserException();
            if (!CanAttach(ann))
                throw new ChalkableSecurityException();

            var uuid = UploadToCrocodoc(name, content);
            var annAtt = new AnnouncementAttachment
            {
                AnnouncementRef = ann.Id,
                AttachedDate = Context.NowSchoolTime,
                Order = ServiceLocator.GetAnnouncementService(annType).GetNewAnnouncementItemOrder(ann)
            };

            var attachment = AnnouncementAttachmentStorage.Add(annAtt);
            ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, attachment.Id.ToString(CultureInfo.InvariantCulture), content);

            if (ann.State != AnnouncementState.Draft)
            {
                if (ann.IsOwner)
                    ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotification(announcementId, annType);
                else
                    ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotificationToOwner(announcementId, annType, Context.PersonId.Value);
            }
            return ann;
        }

        public void AddAttachmentToBlob(IList<AttachmentContentInfo> attachmentContent)
        {
            throw new NotImplementedException();
        }

        public void Delete(int announcementAttachmentId)
        {
            var annAtt = AnnouncementAttachmentStorage.GetById(announcementAttachmentId);
            if (!AnnouncementSecurity.CanDeleteAttachment(annAtt, Context))
                throw new ChalkableSecurityException();

            AnnouncementAttachmentStorage.Delete(annAtt.Id);
            if (!annAtt.Attachment.SisAttachmentId.HasValue)
                ServiceLocator.StorageBlobService.DeleteBlob(ATTACHMENT_CONTAINER_ADDRESS, annAtt.Id.ToString(CultureInfo.InvariantCulture));
        }

        private IList<AnnouncementAttachment> GetAttachmentsQuery(AnnouncementAttachmentQuery query)
        {
            throw new NotImplementedException();
            //query.CallerId = Context.PersonId;
            //query.RoleId = Context.Role.Id;

            //var attachments = AnnouncementAttachmentStorage.GetData().Select(x => x.Value);

            //if (query.AnnouncementId.HasValue)
            //{
            //    attachments = attachments.Where(x => x.AnnouncementRef == query.AnnouncementId);
            //}

            //if (CoreRoles.SUPER_ADMIN_ROLE.Id == query.RoleId)
            //{
            //    return attachments.ToList();
            //}
            //if (CoreRoles.DISTRICT_ADMIN_ROLE.Id == query.RoleId)
            //{

            //    attachments = attachments.Where(x => x.PersonRef == query.CallerId);
            //    return attachments.ToList();
            //}
            //if (CoreRoles.TEACHER_ROLE.Id == query.RoleId)
            //{
            //    var announcementRefs = ((DemoAnnouncementService)ServiceLocator.AnnouncementService)
            //        .GetAnnouncementRecipients(null)
            //            .Select(x => x.AnnouncementRef).ToList();

            //    if (announcementRefs.Count > 0)
            //        attachments = attachments.Where(x =>
            //            {
            //                var announcement = ServiceLocator.AnnouncementService.GetAnnouncementById(x.AnnouncementRef);
            //                return x.PersonRef == query.CallerId || x.PersonRef == announcement.PrimaryTeacherRef || announcementRefs.Contains(x.AnnouncementRef);
            //            });

            //}
            //if (CoreRoles.STUDENT_ROLE.Id == query.RoleId)
            //{
            //    var classRefs = ServiceLocator.ClassService.GetClassPersons(query.CallerId.Value, null).Select(x => x.ClassRef).ToList();

            //    attachments = attachments.Where(x =>
            //    {
            //        var classRef = ServiceLocator.AnnouncementService.GetAnnouncementById(x.AnnouncementRef).ClassRef;
            //        return (x.PersonRef == query.CallerId || classRefs.Contains(classRef.Value));
            //    });
            //    return attachments.ToList();
            //}


            //if (!query.NeedsAllAttachments)
            //    attachments = attachments.Where(x => x.PersonRef == query.CallerId);

            //if (!string.IsNullOrEmpty(query.Filter))
            //{
            //    var filters =
            //        query.Filter.Trim()
            //            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            //            .Select(x => x.ToLower())
            //            .ToList();

            //    attachments = attachments.Where(x => filters.Contains(x.Name.ToLower()));
            //}

            //if (query.Start.HasValue)
            //    attachments = attachments.Skip(query.Start.Value);
            //if (query.Count.HasValue)
            //    attachments = attachments.Take(query.Count.Value);
            //return attachments.ToList();
        }

        public IList<AnnouncementAttachment> GetAnnouncementAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true)
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

        public AnnouncementAttachment GetAnnouncementAttachmentById(int announcementAttachmentId)
        {
            return GetAttachmentsQuery(new AnnouncementAttachmentQuery
            {
                Id = announcementAttachmentId,
                CallerId = Context.PersonId ?? 0,
                RoleId = Context.Role.Id
            }).First();
        }

        public IList<AnnouncementAttachmentInfo> TransformToAttachmentsInfo(IList<AnnouncementAttachment> announcementAttachments, IList<int> teacherIds)
        {
            throw new NotImplementedException();
        }

        public IList<AnnouncementAttachment> GetAnnouncementAttachments(string filter)
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
            var sourceAnnouncement = ServiceLocator.ClassAnnouncementService.GetAnnouncementDetails(fromAnnouncementId);
            foreach (var sourceAnnouncementAttachment in sourceAnnouncement.AnnouncementAttachments)
            {
                var content = ServiceLocator.StorageBlobService.GetBlobContent(ATTACHMENT_CONTAINER_ADDRESS,
                    sourceAnnouncementAttachment.Id.ToString(CultureInfo.InvariantCulture));
                AddAttachment(toAnnouncementId, AnnouncementTypeEnum.Class, content, sourceAnnouncementAttachment.Attachment.Name);
            }
            return GetAnnouncementAttachments(toAnnouncementId);
        }

        public void DeleteAttachments(int id)
        {
            var attachments = ServiceLocator.AnnouncementAttachmentService.GetAnnouncementAttachments(id);
            foreach (var announcementAttachment in attachments)
            {
                ServiceLocator.AnnouncementAttachmentService.Delete(announcementAttachment.Id);
            }
        }
    }
}
