using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;

using Chalkable.Data.Common;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.School.DataAccess;

using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAttachmentService
    {
        Announcement AddAttachment(int announcementId, AnnouncementType type, byte[] content, string name, string uuid);
        void AddAttachmentToBlob(IList<AttachmentContentInfo> attachmentContent);
        //Announcement AddAttachments(int announcementId, AnnouncementType type, IList<AttachmentContentInfo> attachments);
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

        public Announcement AddAttachments(int announcementId, AnnouncementType type, IList<AttachmentContentInfo> attachments)
        {

            var annDetails = ServiceLocator.GetAnnouncementService(type).GetAnnouncementDetails(announcementId);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            if (!CanAttach(annDetails))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                da.Insert(attachments.Select(x=>x.Attachment).ToList());
                uow.Commit();

                foreach (var attachmentContentInfo in attachments)
                {
                    
                }
            }

            throw new System.NotImplementedException();
        }

        public Announcement AddAttachment(int announcementId, AnnouncementType type, byte[] content, string name, string uuid)
        {
            var annDetails = ServiceLocator.GetAnnouncementService(type).GetAnnouncementDetails(announcementId);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            if (!CanAttach(annDetails))
                throw new ChalkableSecurityException();
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

                if (CoreRoles.TEACHER_ROLE == Context.Role)
                {
                    //var stiAtts = ConnectorLocator.AttachmentConnector.UploadAttachment(name, content).ToList();
                    //var lastStiAtts = stiAtts.Last();
                    //if(!string.IsNullOrEmpty(uuid))
                    //    lastStiAtts.CrocoDocId = Guid.Parse(uuid);
                    //if (ann.SisActivityId.HasValue)
                    //{
                    //    var activityAtt = ActivityAttachment.Create(ann.SisActivityId.Value, lastStiAtts, null);
                    //    ConnectorLocator.ActivityAttachmentsConnector.CreateActivityAttachments(ann.SisActivityId.Value, activityAtt);
                    //}
                    //annAtt.SisAttachmentId = lastStiAtts.AttachmentId;
                }
                var da = new AnnouncementAttachmentDataAccess(uow);
                da.Insert(annAtt);
                uow.Commit();
                
                var lastAtt = da.GetList(Context.PersonId.Value, Context.Role.Id, name).Last();
                AddAttachmentToBlob(new List<AttachmentContentInfo> { new AttachmentContentInfo {Attachment = lastAtt, Content = content}});

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
                //if (CoreRoles.TEACHER_ROLE == Context.Role)
                //{
                //    var ann = new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId.Value)
                //        .GetAnnouncement(announcementId, Context.PersonId.Value);
                //    Trace.Assert(ann.SisActivityId.HasValue);
                //    return MapStiAttsToAnnAtts(GetActivityAttachments(ann.SisActivityId.Value));
                //}
                var da = new AnnouncementAttachmentDataAccess(uow);
                return da.GetPaginatedList(announcementId, Context.PersonId.Value, Context.Role.Id, start, count, needsAllAttachments).ToList();
            }
        }

        //private IList<AnnouncementAttachment> MapStiAttsToAnnAtts(IEnumerable<StiAttachment> activityAtts)
        //{
        //    Trace.Assert(Context.PersonId.HasValue);
        //    var res = new List<AnnouncementAttachment>();
        //    foreach (var stiAttachment in activityAtts)
        //    {
        //        var atts = new AnnouncementAttachment {PersonRef = Context.PersonId.Value};
        //        MapperFactory.GetMapper<AnnouncementAttachment, StiAttachment>().Map(atts, stiAttachment);
        //        if (string.IsNullOrEmpty(atts.Uuid) && MimeHelper.GetTypeByName(atts.Name) == MimeHelper.AttachmenType.Document)
        //        {
        //            var content = ConnectorLocator.AttachmentConnector.GetAttachmentContent(stiAttachment.AttachmentId);
        //            atts.Uuid = ServiceLocator.CrocodocService.UploadDocument(stiAttachment.Name, content).uuid;
        //        }
        //        res.Add(atts);
        //    }
        //    return res;
        //} 

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

            //var content = att.SisAttachmentId.HasValue 
            //                     ? ConnectorLocator.AttachmentConnector.GetAttachmentContent(att.SisAttachmentId.Value)
            //                     : ServiceLocator.StorageBlobService.GetBlobContent(ATTACHMENT_CONTAINER_ADDRESS, GenerateKeyForBlob(att));

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

        //private IList<ActivityAttachment> GetActivityAttachments(int activityId)
        //{
        //    return ConnectorLocator.ActivityAttachmentsConnector.GetAttachments(activityId);
        //} 

    }
}
