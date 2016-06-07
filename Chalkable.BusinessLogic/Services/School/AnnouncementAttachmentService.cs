using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAttachmentService
    {
        IList<AnnouncementAttachment> CopyAttachments(int fromAnnouncementId, IList<int> attachmentOwnersIds, int toAnnouncementId);
        AnnouncementAttachment UploadAttachment(int announcementId, AnnouncementTypeEnum type, byte[] content, string name);
        Announcement Add(int announcementId, AnnouncementTypeEnum type, int attachmentId);
        void Delete(int announcementAttachmentId);
        IList<AnnouncementAttachment> GetAnnouncementAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true);
        IList<AnnouncementAttachment> GetAnnouncementAttachments(string filter);
        AnnouncementAttachment GetAnnouncementAttachmentById(int announcementAttachmentId);
        IList<AnnouncementAttachmentInfo> TransformToAttachmentsInfo(IList<AnnouncementAttachment> announcementAttachments, IList<int> teacherIds);

    }

    public class AnnouncementAttachmentService : SisConnectedService, IAnnouncementAttachmentService
    {
        public static IList<AnnouncementAttachment> CopyAnnouncementAttachments(int fromAnnouncementId, IList<int> attachmentsOwners, IList<int> toAnnouncemenIds, UnitOfWork unitOfWork, IServiceLocatorSchool serviceLocator, ConnectorLocator connectorLocator)
        {
            Trace.Assert(serviceLocator.Context.PersonId.HasValue);
            var da = new AnnouncementAttachmentDataAccess(unitOfWork);
            var annAttachmentsForCopying = da.GetLastAttachments(fromAnnouncementId)
                .Where(x => attachmentsOwners.Contains(x.Attachment.PersonRef)).ToList();

            var annAtts = new List<AnnouncementAttachment>();
            foreach (var annAttForCopy in annAttachmentsForCopying)
            {
                foreach (var toAnnouncemenId in toAnnouncemenIds)
                {
                    var attForCopy = annAttForCopy.Attachment;
                    var content = serviceLocator.AttachementService.GetAttachmentContent(attForCopy).Content;
                    if (content != null)
                    {
                        var att = AttachmentService.Upload(attForCopy.Name, content, attForCopy.IsStiAttachment, unitOfWork, serviceLocator, connectorLocator);
                        var annAtt = new AnnouncementAttachment
                        {
                            AnnouncementRef = toAnnouncemenId,
                            AttachedDate = annAttForCopy.AttachedDate,
                            Order = annAttForCopy.Order,
                            AttachmentRef = att.Id,
                            Attachment = att
                        };
                        annAtts.Add(annAtt);
                    }

                }
            }
            da.Insert(annAtts);
            return da.GetLastAttachments(toAnnouncemenIds, annAtts.Count);
        }

        /// <summary>
        /// </summary>
        /// <returns>Source annAttahcment and new annAttachment</returns>
        public static IList<Pair<AnnouncementAttachment, AnnouncementAttachment>> CopyAnnouncementAttachments(IDictionary<int, int> fromToAnnouncementIds, 
            IList<int> attachmentsOwners, UnitOfWork unitOfWork, IServiceLocatorSchool serviceLocator, ConnectorLocator connectorLocator)
        {
            Trace.Assert(serviceLocator.Context.PersonId.HasValue);

            var attachmentDA = new AttachmentDataAccess(unitOfWork);

            var annoncementAttachmentDA = new AnnouncementAttachmentDataAccess(unitOfWork);
            var annAttachmentsToCopy = annoncementAttachmentDA.GetByAnnouncementIds(fromToAnnouncementIds.Select(x=>x.Key).ToList(), attachmentsOwners);

            var fromToAnnAttachments = new List<Pair<AnnouncementAttachment, AnnouncementAttachment>>();

            foreach (var announcementPair in fromToAnnouncementIds)
            {
                var announcementAttachmentsToCopy = annAttachmentsToCopy.Where(x => x.AnnouncementRef == announcementPair.Key).ToList();

                foreach (var annAttachmentToCopy in announcementAttachmentsToCopy)
                {
                    var newAttachment = new Attachment
                    {
                        Name = annAttachmentToCopy.Attachment.Name,
                        PersonRef = serviceLocator.Context.PersonId.Value,
                        Uuid = null,
                        UploadedDate = serviceLocator.Context.NowSchoolTime,
                        LastAttachedDate = serviceLocator.Context.NowSchoolTime,
                    };

                    newAttachment.Id = attachmentDA.InsertWithEntityId(newAttachment);

                    var newAnnAttachment = new AnnouncementAttachment
                    {
                        AnnouncementRef = announcementPair.Value,
                        AttachedDate = annAttachmentToCopy.AttachedDate,
                        Order = annAttachmentToCopy.Order,
                        AttachmentRef = newAttachment.Id,
                        Attachment = newAttachment
                    };

                    fromToAnnAttachments.Add(new Pair<AnnouncementAttachment, AnnouncementAttachment>(annAttachmentToCopy, newAnnAttachment));
                }
            }

            annoncementAttachmentDA.Insert(fromToAnnAttachments.Select(x => x.Second).ToList());

            return fromToAnnAttachments;
        }

        public AnnouncementAttachmentService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        private bool CanAttach(Announcement ann)
        {
            var recipients = ServiceLocator.GetAnnouncementService(ann.Type).GetAnnouncementRecipientPersons(ann.Id);
            return AnnouncementSecurity.CanModifyAnnouncement(ann, Context) || recipients.Any(p => p.Id == Context.PersonId);
        }

        public IList<AnnouncementAttachment> CopyAttachments(int fromAnnouncementId, IList<int> attachmentOwnersIds, int toAnnouncementId)
        {
            IList<AnnouncementAttachment> res = null;
            DoUpdate(u=> { res = CopyAnnouncementAttachments(fromAnnouncementId, attachmentOwnersIds, new List<int> {toAnnouncementId}, u, ServiceLocator, ConnectorLocator);});
            return res;
        }

        public Announcement Add(int announcementId, AnnouncementTypeEnum type, int attachmentId)
        {
            var annDetails = ServiceLocator.GetAnnouncementService(type).GetAnnouncementDetails(announcementId);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            if (!CanAttach(annDetails))
                throw new ChalkableSecurityException();

            using (var u = Update())
            {
                var attDa = new AttachmentDataAccess(u);
                var att = attDa.GetById(attachmentId);
                if(att.PersonRef != Context.PersonId)
                    throw new ChalkableSecurityException();

                var annAtt = new AnnouncementAttachment
                {
                    AnnouncementRef = annDetails.Id,
                    AttachedDate = Context.NowSchoolTime,
                    Order = ServiceLocator.GetAnnouncementService(type).GetNewAnnouncementItemOrder(annDetails),
                    AttachmentRef = attachmentId
                };
                new AnnouncementAttachmentDataAccess(u).Insert(annAtt);
                att.LastAttachedDate = annAtt.AttachedDate;
                attDa.Update(att);
                u.Commit();
                NotifyUsers(annDetails, type);
            }
            return annDetails;
        }

        public AnnouncementAttachment UploadAttachment(int announcementId, AnnouncementTypeEnum type, byte[] content, string name)
        {
            var annDetails = ServiceLocator.GetAnnouncementService(type).GetAnnouncementDetails(announcementId);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            if (!CanAttach(annDetails))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                var att = AttachmentService.Upload(name, content, false, uow, ServiceLocator, ConnectorLocator);
                var annAtt = new AnnouncementAttachment
                {
                    AnnouncementRef = annDetails.Id,
                    AttachedDate = Context.NowSchoolTime,
                    Order = ServiceLocator.GetAnnouncementService(type).GetNewAnnouncementItemOrder(annDetails),
                    AttachmentRef = att.Id,
                };
                var da = new AnnouncementAttachmentDataAccess(uow);
                var attId = da.InsertWithEntityId(annAtt);
                uow.Commit();
                bool notifyUsers = !annDetails.IsOwner
                                   || (annDetails.ClassAnnouncementData != null && annDetails.ClassAnnouncementData.VisibleForStudent)
                                   || (annDetails.LessonPlanData != null && annDetails.LessonPlanData.VisibleForStudent)
                                   || annDetails.AdminAnnouncementData != null;
                if (notifyUsers)
                    NotifyUsers(annDetails, type);

                return da.GetById(attId, Context.PersonId.Value, Context.RoleId);
            }
        }

        private void NotifyUsers(Announcement announcement, AnnouncementTypeEnum type)
        {
            if (!announcement.IsDraft)
            {
                if (announcement.IsOwner)
                    ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotification(announcement.Id, type);
                else
                    ServiceLocator.NotificationService.AddAnnouncementNewAttachmentNotificationToOwner(announcement.Id, type, Context.PersonId.Value);
            }
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


        public void Delete(int announcementAttachmentId)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            using (var uow = Update())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                var annAtt = GetAnnouncementAttachmentById(announcementAttachmentId);
                if (!CanDeleteAttachment(annAtt))
                    throw new ChalkableSecurityException();
                
                da.Delete(annAtt.Id);
                uow.Commit();
            }
        }

        public IList<AnnouncementAttachment> GetAnnouncementAttachments(int announcementId, int start = 0, int count = int.MaxValue, bool needsAllAttachments = true)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            using (var uow = Read())
            {
                var da = new AnnouncementAttachmentDataAccess(uow);
                return da.GetPaginatedList(announcementId, Context.PersonId.Value, Context.Role.Id, start, count, needsAllAttachments).ToList();
            }
        }

        public AnnouncementAttachment GetAnnouncementAttachmentById(int announcementAttachmentId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var hadAdmiClassPermission = Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN)
                                         || Context.Claims.HasPermission(ClaimInfo.MAINTAIN_CLASSROOM_ADMIN);
            return DoRead(u => new AnnouncementAttachmentDataAccess(u).GetById(announcementAttachmentId, Context.PersonId.Value, Context.Role.Id, hadAdmiClassPermission));
        }

        public IList<AnnouncementAttachment> GetAnnouncementAttachments(string filter)
        {
            Trace.Assert(Context.PersonId.HasValue);
            return DoRead(u => new AnnouncementAttachmentDataAccess(u).GetList(Context.PersonId.Value, Context.Role.Id, filter));
        }

        public IList<AnnouncementAttachmentInfo> TransformToAttachmentsInfo(IList<AnnouncementAttachment> announcementAttachments, IList<int> teacherIds)
        {
            return announcementAttachments.Select(x => new AnnouncementAttachmentInfo
                {
                    AttachmentInfo = ServiceLocator.AttachementService.TransformToAttachmentInfo(x.Attachment, teacherIds),
                    AnnouncementAttachment = x
                }).ToList();
        }
    }


    public class AnnouncementAttachmentInfo
    {
        public AnnouncementAttachment AnnouncementAttachment { get; set; }
        public AttachmentInfo AttachmentInfo { get; set; }
    }
}
