using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Common.Web;
using Chalkable.Data.Common.Storage;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
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
                var annAtt = new AnnouncementAttachment
                {
                    AnnouncementRef = ann.Id,
                    PersonRef = Context.UserLocalId.Value,
                    AttachedDate = Context.NowSchoolTime,
                    Name = name,
                    Uuid = uuid,
                    Order = ServiceLocator.AnnouncementService.GetNewAnnouncementItemOrder(ann)
                };
                IList<AnnouncementAttachment> atts;
                if (CoreRoles.TEACHER_ROLE == Context.Role)
                {
                    var stiAtts = ConnectorLocator.AttachmentConnector.DownloadAttachment(name, content);
                    atts = MapStiAttsToAnnAtts(stiAtts.ToList());
                    var lastStiAtts = stiAtts.Last();
                    lastStiAtts.CrocoDocId = Guid.Parse(uuid);
                    if (ann.SisActivityId.HasValue)
                    {
                        var activityAtt = ActivityAttachment.Create(ann.SisActivityId.Value, lastStiAtts, null);
                        activityAtt = ConnectorLocator.ActivityAttachmentsConnector
                            .CreateActivityAttachments(ann.SisActivityId.Value, activityAtt);
                    }
                    annAtt.SisAttachmentId = lastStiAtts.AttachmentId;
                }
                var da = new AnnouncementAttachmentDataAccess(uow);
                da.Insert(annAtt);
                uow.Commit();
                
                atts = da.GetList(Context.UserLocalId.Value, Context.Role.Id, name);
                if(CoreRoles.TEACHER_ROLE != Context.Role)
                    ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, atts.Last().Id.ToString(), content);
                
                if (ann.State != AnnouncementState.Draft)
                {
                    if (Context.UserLocalId == ann.PersonRef)
                        ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotification(announcementId);
                    else
                        ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotificationToPerson(announcementId, Context.UserLocalId.Value);
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
                if(!annAtt.SisAttachmentId.HasValue)
                    ServiceLocator.StorageBlobService.DeleteBlob(ATTACHMENT_CONTAINER_ADDRESS, annAtt.Id.ToString());
                else
                {
                    if(!Context.UserLocalId.HasValue)
                        throw new UnassignedUserException();
                    var ann = (new AnnouncementForTeacherDataAccess(uow, Context.SchoolLocalId))
                        .GetAnnouncement(annAtt.AnnouncementRef, Context.RoleId, Context.UserLocalId.Value);
                    if (ann.SisActivityId.HasValue)
                    {
                        var atts = ConnectorLocator.ActivityAttachmentsConnector.GetAttachments(ann.SisActivityId.Value);
                        var att = atts.FirstOrDefault(x => x.AttachmentId == annAtt.SisAttachmentId.Value);
                        if(att != null)
                            ConnectorLocator.ActivityAttachmentsConnector.Delete(ann.SisActivityId.Value, att.Id);
                    }
                    else ConnectorLocator.AttachmentConnector.DeleteAttachment(annAtt.SisAttachmentId.Value);

                    //else 
                    //    ConnectorLocator.AttachmentConnector.DeleteAttachment(annAtt.SisAttachmentId.Value);
                }
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
                    return MapStiAttsToAnnAtts(GetActivityAttachments(ann.SisActivityId.Value));
                }
                var da = new AnnouncementAttachmentDataAccess(uow);
                return da.GetPaginatedList(announcementId, Context.UserLocalId ?? 0, Context.Role.Id, start, count, needsAllAttachments).ToList();
            }
        }

        private IList<AnnouncementAttachment> MapStiAttsToAnnAtts(IEnumerable<StiAttachment> activityAtts)
        {
            var res = new List<AnnouncementAttachment>();
            foreach (var stiAttachment in activityAtts)
            {
                var atts = new AnnouncementAttachment {PersonRef = Context.UserLocalId.Value};
                MapperFactory.GetMapper<AnnouncementAttachment, StiAttachment>().Map(atts, stiAttachment);
                if (string.IsNullOrEmpty(atts.Uuid) && MimeHelper.GetTypeByName(atts.Name) == MimeHelper.AttachmenType.Document)
                {
                    var content = ConnectorLocator.AttachmentConnector.GetAttachmentContent(stiAttachment.AttachmentId);
                    atts.Uuid = ServiceLocator.CrocodocService.UploadDocument(stiAttachment.Name, content).uuid;
                    ServiceLocator.StorageBlobService.AddBlob(ATTACHMENT_CONTAINER_ADDRESS, atts.Id.ToString(), content);
                }
                res.Add(atts);
            }
            return res;
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
