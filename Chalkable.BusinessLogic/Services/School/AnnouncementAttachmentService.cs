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
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAttachmentService
    {
        Announcement AddAttachment(int announcementId, byte[] content, string name, string uuid);
        void DeleteAttachment(int announcementAttachmentId);
        IList<AnnouncementAttachment> GetAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true);
        IList<AnnouncementAttachment> GetAttachments(string filter);
        AnnouncementAttachment GetAttachmentById(int announcementAttachmentId);
        AttachmentContentInfo GetAttachmentContent(int announcementAttachmentId);
    }

    public class AnnouncementAttachmentService : SisConnectedService, IAnnouncementAttachmentService
    {
        public AnnouncementAttachmentService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        private const string ATTACHMENT_CONTAINER_ADDRESS = "attachmentscontainer";

        public Announcement AddAttachment(int announcementId, byte[] content, string name, string uuid)
        {
            var ann = ServiceLocator.AnnouncementService.GetAnnouncementDetails(announcementId);
            if (!(Context.UserLocalId.HasValue && Context.SchoolId.HasValue))
                throw new UnassignedUserException();
            if(!AnnouncementSecurity.CanAttach(ann, Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {

                IList<AnnouncementAttachment> atts;
                if (CoreRoles.TEACHER_ROLE == Context.Role && ann.SisActivityId.HasValue)
                {
                    var activityAtts = ConnectorLocator.ActivityAttachmentsConnector.CreateAttachments(ann.SisActivityId.Value, name, content);
                    atts = MapActivityAttsToAnnAtts(activityAtts);
                }
                else
                {
                    var da = new AnnouncementAttachmentDataAccess(uow);
                    da.Insert(new AnnouncementAttachment
                    {
                        AnnouncementRef = ann.Id,
                        PersonRef = Context.UserLocalId.Value,
                        AttachedDate = Context.NowSchoolTime,
                        Name = name,
                        Uuid = uuid,
                        Order = ServiceLocator.AnnouncementService.GetNewAnnouncementItemOrder(ann)
                    });
                    atts = da.GetList(Context.UserLocalId.Value, Context.Role.Id, name);
                }
                ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, atts.Last().Id.ToString(), content);
                uow.Commit();
                
                if (ann.State != AnnouncementState.Draft)
                {
                    if (Context.UserLocalId == ann.PersonRef)
                    {
                        ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotification(announcementId);
                    }
                    else
                    {
                        ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotificationToPerson(announcementId, Context.UserLocalId.Value);
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

        public IList<AnnouncementAttachment> GetAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true)
        {
            using (var uow = Read())
            {
                if (CoreRoles.TEACHER_ROLE == Context.Role)
                {
                    var ann = new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId)
                        .GetAnnouncement(announcementId, Context.RoleId, Context.UserLocalId.Value);
                    return MapActivityAttsToAnnAtts(GetActivityAttachments(ann.SisActivityId.Value));
                }
                var da = new AnnouncementAttachmentDataAccess(uow);
                return da.GetPaginatedList(announcementId, Context.UserLocalId ?? 0, Context.Role.Id, start, count, needsAllAttachments).ToList();
            }
        }

        private IList<AnnouncementAttachment> MapActivityAttsToAnnAtts(IEnumerable<ActivityAttachment> activityAtts)
        {
            var res = new List<AnnouncementAttachment>();
            foreach (var activityAttachment in activityAtts)
            {
                var atts = MapActivityAttachmentToAnnAttachment(new AnnouncementAttachment(), activityAttachment);
                if (atts.Uuid == null)
                {
                 //   ConnectorLocator.ActivityAttachmentsConnector.
                }
                res.Add(atts);
            }
            return res;
        } 

        private AnnouncementAttachment MapActivityAttachmentToAnnAttachment(
            AnnouncementAttachment announcementAttachment, ActivityAttachment activityAttachment)
        {
            announcementAttachment.Name = activityAttachment.Name;
            announcementAttachment.Uuid = activityAttachment.Uuid;
            announcementAttachment.Id = activityAttachment.Id;
            announcementAttachment.SisActivityId = activityAttachment.ActivityId;
            return announcementAttachment;
        }

        public AnnouncementAttachment GetAttachmentById(int announcementAttachmentId)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                return da.GetById(announcementAttachmentId, Context.UserLocalId ?? 0, Context.Role.Id);
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
                var res = new AnnouncementAttachmentDataAccess(uow).GetList(Context.UserLocalId ?? 0, Context.Role.Id, filter);
                //if (CoreRoles.TEACHER_ROLE == Context.Role)
                //{
                //    var atts = GetActivityAttachments()
                //}
                return res;
            }
        }
        public static string GetAttachmentRelativeAddress()
        {
            return (new BlobHelper()).GetBlobsRelativeAddress(ATTACHMENT_CONTAINER_ADDRESS);
        }

        private IList<ActivityAttachment> GetActivityAttachments(int activityId)
        {
            return ConnectorLocator.ActivityAttachmentsConnector.GetAttachments(activityId);
        } 
    }
}
