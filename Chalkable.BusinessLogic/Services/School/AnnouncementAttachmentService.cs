using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.School.DataAccess;

using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAttachmentService
    {
        IList<AnnouncementAttachment> CopyAttachments(int fromAnnouncementId, int toAnnouncementId);
        Announcement AddAttachment(int announcementId, AnnouncementType type, byte[] content, string name);
        void AddAttachmentToBlob(IList<AttachmentContentInfo> attachmentContent);
        void DeleteAttachment(int announcementAttachmentId);
        IList<AnnouncementAttachment> GetAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true);
        IList<AnnouncementAttachment> GetAttachments(string filter);
        AnnouncementAttachment GetAttachmentById(int announcementAttachmentId);
        AttachmentContentInfo GetAttachmentContent(int announcementAttachmentId);
        AttachmentContentInfo GetAttachmentContent(AnnouncementAttachment announcementAttachment);
    }

    public class AnnouncementAttachmentService : SisConnectedService, IAnnouncementAttachmentService
    {
        public AnnouncementAttachmentService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        private const string ATTACHMENT_CONTAINER_ADDRESS = "attachmentscontainer";

        private bool CanAttach(Announcement ann)
        {
            var recipients = ServiceLocator.GetAnnouncementService(ann.Type).GetAnnouncementRecipientPersons(ann.Id);
            return AnnouncementSecurity.CanModifyAnnouncement(ann, Context) || recipients.Any(p => p.Id == Context.PersonId);
        }

        private string UploadToCrocodoc(string name, byte[] content)
        {
            if (ServiceLocator.CrocodocService.IsDocument(name))
                return ServiceLocator.CrocodocService.UploadDocument(name, content).uuid;
            return null;
        }

        public IList<AnnouncementAttachment> CopyAttachments(int fromAnnouncementId, IList<int> toAnnouncemenIds, UnitOfWork unitOfWork)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var da = new AnnouncementAttachmentDataAccess(unitOfWork);
            var attachmentsForCopying = da.GetLastAttachments(fromAnnouncementId);
            var attContentsForCopy = attachmentsForCopying.Select(ServiceLocator.AnnouncementAttachmentService.GetAttachmentContent).ToList();

            var attContents = new List<AttachmentContentInfo>();
            foreach (var attWithContent in attContentsForCopy)
            {
                foreach (var toAnnouncemenId in toAnnouncemenIds)
                {
                    var uuid = UploadToCrocodoc(attWithContent.Attachment.Name, attWithContent.Content);
                    var att = new AnnouncementAttachment
                    {
                        AnnouncementRef = toAnnouncemenId,
                        AttachedDate = attWithContent.Attachment.AttachedDate,
                        Name = attWithContent.Attachment.Name,
                        PersonRef = Context.PersonId.Value,
                        Uuid = uuid,
                        Order = attWithContent.Attachment.Order
                    };
                    attContents.Add(AttachmentContentInfo.Create(att, attWithContent.Content));
                }
            }
            da.Insert(attContents.Select(x => x.Attachment).ToList());
            var atts = da.GetLastAttachments(toAnnouncemenIds, attContents.Count);
            
            for (var i = 0; i < atts.Count; i++) 
                attContents[i].Attachment = atts[i];
            
            AddAttachmentToBlob(attContents);
            return atts;
        }


        public IList<AnnouncementAttachment> CopyAttachments(int fromAnnouncementId, int toAnnouncementId)
        {
            using (var u = Update())
            {
                var res = CopyAttachments(fromAnnouncementId, new List<int>{toAnnouncementId}, u);
                u.Commit();
                return res;
            }
        }


        public Announcement AddAttachment(int announcementId, AnnouncementType type, byte[] content, string name)
        {
            var annDetails = ServiceLocator.GetAnnouncementService(type).GetAnnouncementDetails(announcementId);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            if (!CanAttach(annDetails))
                throw new ChalkableSecurityException();
            
            var uuid = UploadToCrocodoc(name, content);
            using (var uow = Update())
            {               
                var annAtt = new AnnouncementAttachment
                {
                    AnnouncementRef = annDetails.Id,
                    PersonRef = Context.PersonId.Value,
                    AttachedDate = Context.NowSchoolTime,
                    Name = name,
                    Uuid = uuid,
                    Order = ServiceLocator.GetAnnouncementService(type).GetNewAnnouncementItemOrder(annDetails)
                };
                var da = new AnnouncementAttachmentDataAccess(uow);
                da.Insert(annAtt);
                uow.Commit();
                
                var lastAtt = da.GetList(Context.PersonId.Value, Context.Role.Id, name).Last();
                AddAttachmentToBlob(new List<AttachmentContentInfo> {AttachmentContentInfo.Create(lastAtt, content)});

                if (!annDetails.IsDraft)
                {
                    if (annDetails.IsOwner)
                        ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotification(announcementId, type);
                    else
                        ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotificationToTeachers(announcementId, type, Context.PersonId.Value);
                }
            }
            return annDetails;
        }


        public void AddAttachmentToBlob(IList<AttachmentContentInfo> attachmentContent)
        {
            foreach (var attachmentContentInfo in attachmentContent)
            {
                ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, GenerateKeyForBlob(attachmentContentInfo.Attachment), attachmentContentInfo.Content);                
            }
        }


        private string GenerateKeyForBlob(AnnouncementAttachment announcementAttachment)
        {
            var res = announcementAttachment.Id.ToString(CultureInfo.InvariantCulture);
            if (Context.DistrictId.HasValue)
                return string.Format("{0}_{1}", res, Context.DistrictId.Value);
            return res;
        }

        private bool CanDeleteAttachment(AnnouncementAttachment announcementAttachment)
        {
            //TODO: impl method get announcement owners 
            //var ann = ServiceLocator.AnnouncementService.GetAnnouncementById(announcementAttachment.AnnouncementRef);
            //int? classId = null;
            //if (ann is LessonPlan) classId = (ann as LessonPlan).ClassRef;
            //if (ann is ClassAnnouncement) classId = (ann as ClassAnnouncement).ClassRef;
            //var teachers =  ServiceLocator.ClassService.GetClassTeachers(classId, null);
            //return teachers.Any(x => x.PersonRef == announcementAttachment.PersonRef) || announcementAttachment.PersonRef == Context.PersonId;
            return true;
        }


        public void DeleteAttachment(int announcementAttachmentId)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            using (var uow = Update())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                var annAtt = GetAttachmentById(announcementAttachmentId);
                if (!CanDeleteAttachment(annAtt))
                    throw new ChalkableSecurityException();
                
                da.Delete(annAtt.Id);
                ServiceLocator.StorageBlobService.DeleteBlob(ATTACHMENT_CONTAINER_ADDRESS, GenerateKeyForBlob(annAtt));
                uow.Commit();
            }
        }

        public IList<AnnouncementAttachment> GetAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            using (var uow = Read())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                return da.GetPaginatedList(announcementId, Context.PersonId.Value, Context.Role.Id, start, count, needsAllAttachments).ToList();
            }
        }

        public AnnouncementAttachment GetAttachmentById(int announcementAttachmentId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => new AnnouncementAttachmentDataAccess(u).GetById(announcementAttachmentId, Context.PersonId.Value, Context.Role.Id));
        }

        public AttachmentContentInfo GetAttachmentContent(int announcementAttachmentId)
        {
            var att = GetAttachmentById(announcementAttachmentId);
            if (att == null)
                return null;

            return GetAttachmentContent(att);
        }

        public AttachmentContentInfo GetAttachmentContent(AnnouncementAttachment announcementAttachment)
        {
            var content = ServiceLocator.StorageBlobService.GetBlobContent(ATTACHMENT_CONTAINER_ADDRESS, GenerateKeyForBlob(announcementAttachment));
            return AttachmentContentInfo.Create(announcementAttachment, content);
        }
       
        public IList<AnnouncementAttachment> GetAttachments(string filter)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => new AnnouncementAttachmentDataAccess(u).GetList(Context.PersonId.Value, Context.Role.Id, filter));
        }
        
        public static string GetAttachmentRelativeAddress()
        {
            return (new BlobHelper()).GetBlobsRelativeAddress(ATTACHMENT_CONTAINER_ADDRESS);
        }

        
    }
}
