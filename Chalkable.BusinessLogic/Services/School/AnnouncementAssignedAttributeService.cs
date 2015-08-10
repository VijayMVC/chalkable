using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAssignedAttributeService
    {
        void Edit(AnnouncementType announcementType, int announcementId, IList<AssignedAttributeInputModel> attributes);
        void Delete(int announcementAssignedAttributeId);
        AnnouncementAssignedAttribute Add(AnnouncementType announcementType, int announcementId, int attributeTypeId);
        AnnouncementAssignedAttribute UploadAttachment(AnnouncementType announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name);
        AnnouncementAssignedAttribute AddAttachment(AnnouncementType announcementType, int announcementId, int assignedAttributeId, int attachmentId);
        AnnouncementAssignedAttribute GetAssignedAttributeById(int announcementAssignedAttributeId);
        AnnouncementAssignedAttribute RemoveAttachment(int announcementAssignedAttributeId);
        IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, int toAnnouncementId);
    }

    public class AnnouncementAssignedAttributeService : SisConnectedService, IAnnouncementAssignedAttributeService
    {
        public AnnouncementAssignedAttributeService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public void Edit(AnnouncementType announcementType, int announcementId, IList<AssignedAttributeInputModel> attributes)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(announcementId); // security check
            if (attributes != null)
            {
                var annAttributes = attributes.Select(a => new AnnouncementAssignedAttribute
                {
                    Id = a.Id,
                    AnnouncementRef = a.AnnouncementId,
                    AttachmentRef = a.AttachmentId,
                    AttributeTypeId = a.AttributeTypeId,
                    Name = a.Name,
                    SisActivityAssignedAttributeId = a.SisActivityAssignedAttributeId,
                    Text = a.Text,
                    SisActivityId = a.SisActivityId,
                    VisibleForStudents = a.VisibleForStudents,
                }).ToList();

                DoUpdate(u => new AnnouncementAssignedAttributeDataAccess(u).Update(annAttributes));
            }
        }

        public void Delete(int assignedAttributeId)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            DoUpdate(u =>
                {
                    var da = new AnnouncementAssignedAttributeDataAccess(u);
                    var attribute = da.GetById(assignedAttributeId);
                    if (attribute.SisActivityId.HasValue && attribute.SisActivityAssignedAttributeId.HasValue)
                    {
                        ConnectorLocator.ActivityAssignedAttributeConnector.Delete(attribute.SisActivityId.Value, attribute.SisActivityAssignedAttributeId.Value);
                    }
                    da.Delete(assignedAttributeId);       
                });
        }

        private bool CanAttach(Announcement ann)
        {
            return AnnouncementSecurity.CanModifyAnnouncement(ann, Context);
        }

        public AnnouncementAssignedAttribute Add(AnnouncementType announcementType, int announcementId, int attributeTypeId)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(announcementId);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            var attributeType = ServiceLocator.AnnouncementAttributeService.GetAttributeById(attributeTypeId, true);

            var id = -1;

            using (var uow = Update())
            {
                if (!CanAttach(ann))
                    throw new ChalkableSecurityException();

                var annAttribute = new AnnouncementAssignedAttribute
                {
                    AnnouncementRef = ann.Id,
                    AttributeTypeId = attributeType.Id,
                    Text = " ",
                    VisibleForStudents = false,
                    Name = attributeType.Name
                };

                if (announcementType == AnnouncementType.Class)
                {
                    var announcement = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);
                    if (announcement.SisActivityId.HasValue)
                    {
                        var activityAssignedAttr = new ActivityAssignedAttribute();
                        MapperFactory.GetMapper<ActivityAssignedAttribute, AnnouncementAssignedAttribute>().Map(activityAssignedAttr, annAttribute);
                        activityAssignedAttr = ConnectorLocator.ActivityAssignedAttributeConnector.CreateActivityAttribute(announcement.SisActivityId.Value, activityAssignedAttr);
                        MapperFactory.GetMapper<AnnouncementAssignedAttribute, ActivityAssignedAttribute>().Map(annAttribute, activityAssignedAttr);
                        annAttribute.Name = attributeType.Name;//activity attr returns null name
                    }
                }
                
                var da = new AnnouncementAssignedAttributeDataAccess(uow);
                id = da.InsertWithEntityId(annAttribute);
                uow.Commit();
                return da.GetById(id);
            }
        }

        public AnnouncementAssignedAttribute UploadAttachment(AnnouncementType announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name)
        {
            Trace.Assert(Context.PersonId.HasValue && Context.SchoolLocalId.HasValue);
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(announcementId);
            
            using (var uow = Update())
            {
                var da = new AnnouncementAssignedAttributeDataAccess(uow);
                var assignedAttribute = da.GetById(assignedAttributeId);
                if (!CanAttach(ann))
                    throw new ChalkableSecurityException();
            
                if (assignedAttribute.AttachmentRef > 0)
                    throw new ChalkableSisException("You can't attach more than one file to an attribute");

                var isStiAttribute = assignedAttribute.IsStiAttribute || announcementType == AnnouncementType.Class;
                var attachment = ((AttachmentService)ServiceLocator.AttachementService).Upload(name, bin, isStiAttribute, uow);
                assignedAttribute.AttachmentRef = attachment.Id;
                assignedAttribute.Attachment = attachment;
                da.Update(assignedAttribute);
                
                if (assignedAttribute.IsStiAttribute)
                {
                    var stiAttribute = ConnectorLocator.ActivityAssignedAttributeConnector.GetAttribute(
                            assignedAttribute.SisActivityId.Value,
                            assignedAttribute.SisActivityAssignedAttributeId.Value);
     
                    MapperFactory.GetMapper<ActivityAssignedAttribute, AnnouncementAssignedAttribute>().Map(stiAttribute, assignedAttribute);
                    ConnectorLocator.ActivityAssignedAttributeConnector.Update(stiAttribute.ActivityId, stiAttribute.Id, stiAttribute);
                }
                uow.Commit();
                return assignedAttribute;
            }
        }

        public AnnouncementAssignedAttribute AddAttachment(AnnouncementType announcementType, int announcementId, int assignedAttributeId, int attachmentId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(announcementId);
            if (!CanAttach(ann))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                var da = new AnnouncementAssignedAttributeDataAccess(uow);
                var attribute = da.GetById(assignedAttributeId);
                if(attribute.AttachmentRef > 0)
                    throw new ChalkableSisException("You can't attach more than one file to an attribute");

                var attDa = new AttachmentDataAccess(uow);
                var attachment = attDa.GetById(attachmentId);
                if(attachment.PersonRef != Context.PersonId)
                    throw new ChalkableSecurityException();
                if (attribute.SisActivityAssignedAttributeId.HasValue)
                {
                    if (!attachment.SisAttachmentId.HasValue)
                    {
                        var attContent = ServiceLocator.AttachementService.GetAttachmentContent(attachment);
                        var stiAtt = ConnectorLocator.AttachmentConnector.UploadAttachment(attachment.Name, attContent.Content).Last();
                        MapperFactory.GetMapper<Attachment, StiAttachment>().Map(attachment, stiAtt);
                        attDa.Update(attachment);
                    }
                    attribute.AttachmentRef = attachment.Id;
                    attribute.Attachment = attachment;
                    da.Update(attribute);
                    var stiAttribute = ConnectorLocator.ActivityAssignedAttributeConnector.GetAttribute(attribute.SisActivityId.Value, attribute.SisActivityAssignedAttributeId.Value);
                    MapperFactory.GetMapper<ActivityAssignedAttribute, AnnouncementAssignedAttribute>().Map(stiAttribute, attribute);
                    ConnectorLocator.ActivityAssignedAttributeConnector.Update(stiAttribute.ActivityId, stiAttribute.Id, stiAttribute);
                }
                uow.Commit();
                return attribute;
            }
        }

       
        public AnnouncementAssignedAttribute GetAssignedAttributeById(int assignedAttributeId)
        {
            return DoRead(u => new AnnouncementAssignedAttributeDataAccess(u).GetById(assignedAttributeId));
        }

        public AnnouncementAssignedAttribute RemoveAttachment(int announcementAssignedAttributeId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            
            using (var uow = Update())
            {
                var da = new AnnouncementAssignedAttributeDataAccess(uow);
                var attribute = da.GetById(announcementAssignedAttributeId);
                if(!attribute.AttachmentRef.HasValue)
                    throw new ChalkableException("Attribute has no attachments for remove.");
                if (attribute.IsStiAttribute)
                {
                    var activityAssignedAttribute = ConnectorLocator.ActivityAssignedAttributeConnector.GetAttribute(attribute.SisActivityId.Value, attribute.SisActivityAssignedAttributeId.Value);
                    activityAssignedAttribute.Attachment = null;
                    ConnectorLocator.ActivityAssignedAttributeConnector.Update(attribute.SisActivityId.Value, attribute.SisActivityAssignedAttributeId.Value, activityAssignedAttribute);
                }
                attribute.AttachmentRef = null;
                da.Update(attribute);
                uow.Commit();
                return attribute;
            }
        }

        public void AddMissingSisAttributes(IList<AnnouncementAssignedAttribute> attributes, UnitOfWork u)
        {
            var missingAttributes = attributes.Where(a => a.Id <= 0).ToList();
            //var stiAttachmentsIds = missingAttributes.Where(x => x.Attachment != null && x.Attachment.SisAttachmentId.HasValue)
            //                                         .Select(x => x.Attachment.SisAttachmentId.Value).ToList();
            //var attachmentDa = new AttachmentDataAccess(u);
            //var existingAttachments = attachmentDa.GetBySisAttachmentIds(stiAttachmentsIds); 
            //foreach (var attribute in missingAttributes)
            //{
            //    var attachment = attribute.Attachment;
            //    if (attachment != null && attachment.IsStiAttachment && ServiceLocator.CrocodocService.IsDocument(attribute.Attachment.Name))
            //    {
            //        var existingAtt = existingAttachments.FirstOrDefault(x => x.SisAttachmentId == attachment.SisAttachmentId);
            //        //if not exists uploadNewAttachment
            //        if (existingAtt == null)
            //        {
            //            var content = ConnectorLocator.AttachmentConnector.GetAttachmentContent(attribute.Attachment.SisAttachmentId.Value);
            //            attachment.Uuid = ServiceLocator.CrocodocService.UploadDocument(attribute.Attachment.Name, content).uuid;    
            //        }
            //    }
            //}
            new AnnouncementAssignedAttributeDataAccess(u).Insert(missingAttributes);
         }

        public void UploadMissingAttachments(IList<AnnouncementAssignedAttribute> attributes, UnitOfWork u)
        {
            var attributesForUpdate = attributes.Where(x => x.Attachment != null && x.Attachment.SisAttachmentId.HasValue && !x.AttachmentRef.HasValue).ToList();
            var da = new AttachmentDataAccess(u);
            var existingsAtts = da.GetBySisAttachmentIds(attributesForUpdate.Select(x => x.Attachment.SisAttachmentId.Value).ToList());
            var attsForUpload = new List<Attachment>();
            foreach (var attribute in attributesForUpdate)
            {
                var attachment = attribute.Attachment;
                var existingAtt = existingsAtts.FirstOrDefault(x => x.SisAttachmentId == attachment.SisAttachmentId);
                if (existingAtt == null)
                {
                    var content = ConnectorLocator.AttachmentConnector.GetAttachmentContent(attribute.Attachment.SisAttachmentId.Value);
                    attachment.Uuid = ServiceLocator.CrocodocService.UploadDocument(attribute.Attachment.Name, content).uuid;
                    attachment.UploadedDate = Context.NowSchoolYearTime;
                    attachment.LastAttachedDate = attachment.UploadedDate;
                    attsForUpload.Add(attachment);
                }
                else attribute.AttachmentRef = existingAtt.Id;
            }
            da.Insert(attsForUpload);
            existingsAtts = da.GetBySisAttachmentIds(attsForUpload.Select(x => x.SisAttachmentId.Value).ToList());
            foreach (var attribute in attributesForUpdate.Where(x=> !x.AttachmentRef.HasValue))
            {
                var att = existingsAtts.First(x => x.SisAttachmentId == attribute.Attachment.SisAttachmentId);
                attribute.Attachment = att;
                attribute.AttachmentRef = att.Id;
            }
            new AnnouncementAssignedAttributeDataAccess(u).Update(attributesForUpdate);
        }


        public IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, IList<int> toAnnouncementIds, UnitOfWork unitOfWork)
        {
            var da = new AnnouncementAssignedAttributeDataAccess(unitOfWork);
            var attributesForCopying = da.GetListByAnntId(fromAnnouncementId);
            attributesForCopying = attributesForCopying.Where(x => !x.SisActivityAssignedAttributeId.HasValue).ToList();

            var atributesInfo = new List<AnnouncementAssignedAttributeInfo>();
            foreach (var attributeForCopying in attributesForCopying)
            {
                foreach (var toAnnouncementId in toAnnouncementIds)
                {
                    var attribute = new AnnouncementAssignedAttribute
                    {
                        AnnouncementRef = toAnnouncementId,
                        AttributeTypeId = attributeForCopying.AttributeTypeId,
                        Name = attributeForCopying.Name,
                        Text = attributeForCopying.Text,
                        VisibleForStudents = attributeForCopying.VisibleForStudents,
                    };
                    if (attributeForCopying.AttachmentRef.HasValue)
                    {
                        var attService = ((AttachmentService) ServiceLocator.AttachementService);
                        var attContent = attService.GetAttachmentContent(attribute.Attachment);
                        var att = attService.Upload(attContent.Attachment.Name, attContent.Content, attContent.Attachment.IsStiAttachment, unitOfWork);
                        attribute.AttachmentRef = att.Id;
                        attribute.Attachment = att;
                    }
                }
            }
            da.Insert(atributesInfo.Select(x => x.Attribute).ToList());

            return da.GetLastListByAnnIds(toAnnouncementIds, atributesInfo.Count);
        }

        public IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, int toAnnouncementId)
        {
            using (var u = Update())
            {
                var res = CopyNonStiAttributes(fromAnnouncementId, new List<int>{toAnnouncementId}, u);
                u.Commit();
                return res;
            }
        }

    }
}
