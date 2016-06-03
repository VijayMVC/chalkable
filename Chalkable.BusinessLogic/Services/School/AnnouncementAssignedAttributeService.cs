using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.UI.WebControls;
using Chalkable.BusinessLogic.Mapping.ModelMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementAssignedAttributeService
    {
        void Edit(AnnouncementTypeEnum announcementType, int announcementId, IList<AssignedAttributeInputModel> attributes);
        void Delete(int announcementAssignedAttributeId);
        AnnouncementAssignedAttribute Add(AnnouncementTypeEnum announcementType, int announcementId, int attributeTypeId);
        AnnouncementAssignedAttribute UploadAttachment(AnnouncementTypeEnum announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name);
        AnnouncementAssignedAttribute AddAttachment(AnnouncementTypeEnum announcementType, int announcementId, int assignedAttributeId, int attachmentId);
        AnnouncementAssignedAttribute GetAssignedAttributeById(int announcementAssignedAttributeId);
        AnnouncementAssignedAttribute RemoveAttachment(int announcementAssignedAttributeId);
        IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, int toAnnouncementId);
        void ValidateAttributes(IList<AnnouncementAssignedAttribute> attributes);
    }

    public class AnnouncementAssignedAttributeService : SisConnectedService, IAnnouncementAssignedAttributeService
    {
        public AnnouncementAssignedAttributeService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }

        public void Edit(AnnouncementTypeEnum announcementType, int announcementId, IList<AssignedAttributeInputModel> inputAttributes)
        {
            BaseSecurity.EnsureAdminOrTeacher(Context);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            var annoncement = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(announcementId); // security check
            if (inputAttributes != null)
            {
                DoUpdate(u =>
                    {
                        var da = new AnnouncementAssignedAttributeDataAccess(u);
                        var attributesForUpdate = da.GetAttributesByIds(inputAttributes.Select(x => x.Id).ToList());
                        foreach (var attribute in attributesForUpdate)
                        {
                            var inputAttr = inputAttributes.FirstOrDefault(x => x.Id == attribute.Id);
                            if (inputAttr == null) continue;
                            attribute.AnnouncementRef = inputAttr.AnnouncementId;
                            attribute.Name = inputAttr.Name;
                            attribute.Text = inputAttr.Text;
                            attribute.VisibleForStudents = inputAttr.VisibleForStudents;
                            attribute.AttributeTypeId = inputAttr.AttributeTypeId;
                            attribute.AttachmentRef = inputAttr.AttachmentId;
                        }
                        if(!annoncement.IsDraft) ValidateAttributes(attributesForUpdate);
                        da.Update(attributesForUpdate);
                    });
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

        public AnnouncementAssignedAttribute Add(AnnouncementTypeEnum announcementType, int announcementId, int attributeTypeId)
        {
            var ann = ServiceLocator.GetAnnouncementService(announcementType).GetAnnouncementById(announcementId);
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            var attributeType = ServiceLocator.AnnouncementAttributeService.GetAttributeById(attributeTypeId, true);


            using (var uow = Update())
            {
                if (!CanAttach(ann))
                    throw new ChalkableSecurityException();

                var annAttribute = new AnnouncementAssignedAttribute
                {
                    AnnouncementRef = ann.Id,
                    AttributeTypeId = attributeType.Id,
                    Text = "",
                    VisibleForStudents = false,
                    Name = attributeType.Name
                };

                if (announcementType == AnnouncementTypeEnum.Class)
                {
                    var announcement = ServiceLocator.ClassAnnouncementService.GetClassAnnouncemenById(announcementId);
                    if (announcement.SisActivityId.HasValue)
                    {
                        var activityAssignedAttr = new ActivityAssignedAttribute();
                        MapperFactory.GetMapper<ActivityAssignedAttribute, AnnouncementAssignedAttribute>().Map(activityAssignedAttr, annAttribute);
                        activityAssignedAttr.Text = " ";
                        activityAssignedAttr = ConnectorLocator.ActivityAssignedAttributeConnector.CreateActivityAttribute(announcement.SisActivityId.Value, activityAssignedAttr);
                        MapperFactory.GetMapper<AnnouncementAssignedAttribute, ActivityAssignedAttribute>().Map(annAttribute, activityAssignedAttr);
                        annAttribute.Name = attributeType.Name;//activity attr returns null name
                    }
                }
                
                var da = new AnnouncementAssignedAttributeDataAccess(uow);
                var id = da.InsertWithEntityId(annAttribute);
                uow.Commit();
                return da.GetById(id);
            }
        }

        public AnnouncementAssignedAttribute UploadAttachment(AnnouncementTypeEnum announcementType, int announcementId, int assignedAttributeId, byte[] bin, string name)
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

                var isStiAttribute = assignedAttribute.IsStiAttribute || announcementType == AnnouncementTypeEnum.Class;
                var attachment = AttachmentService.Upload(name, bin, isStiAttribute, uow, ServiceLocator, ConnectorLocator);
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

        public AnnouncementAssignedAttribute AddAttachment(AnnouncementTypeEnum announcementType, int announcementId, int assignedAttributeId, int attachmentId)
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

                attribute.AttachmentRef = attachment.Id;
                attribute.Attachment = attachment;
                da.Update(attribute);

                if (announcementType == AnnouncementTypeEnum.Class)
                {
                    if (!attachment.SisAttachmentId.HasValue)
                    {
                        var attContent = ServiceLocator.AttachementService.GetAttachmentContent(attachment);
                        var stiAtt = ConnectorLocator.AttachmentConnector.UploadAttachment(attachment.Name, attContent.Content).Last();
                        MapperFactory.GetMapper<Attachment, StiAttachment>().Map(attachment, stiAtt);
                        attDa.Update(attachment);
                    }

                    if (attribute.SisActivityAssignedAttributeId.HasValue)
                    {
                        var stiAttribute = ConnectorLocator.ActivityAssignedAttributeConnector.GetAttribute(attribute.SisActivityId.Value, attribute.SisActivityAssignedAttributeId.Value);
                        MapperFactory.GetMapper<ActivityAssignedAttribute, AnnouncementAssignedAttribute>().Map(stiAttribute, attribute);
                        ConnectorLocator.ActivityAssignedAttributeConnector.Update(stiAttribute.ActivityId, stiAttribute.Id, stiAttribute);
                    }
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

        public static void AddMissingSisAttributes(ClassAnnouncement classAnn, IList<AnnouncementAssignedAttribute> attributes, UnitOfWork u
            , ConnectorLocator connectorLocator, IServiceLocatorSchool serviceLocator)
        {
            var missingAttributes = attributes.Where(a => a.Id <= 0).ToList();
            var attrs = UploadMissingAttachments(classAnn, missingAttributes, u, connectorLocator, serviceLocator);
            if(attributes.Count > 0)
                foreach (var missingAttr in missingAttributes)
                {
                    var attr = attrs.FirstOrDefault(x => x.SisActivityAssignedAttributeId == missingAttr.SisActivityAssignedAttributeId);
                    if(attr == null) continue;
                    missingAttr.AttachmentRef = attr.AttachmentRef;
                    missingAttr.Attachment = attr.Attachment;
                }
            new AnnouncementAssignedAttributeDataAccess(u).Insert(missingAttributes);
         }

        public static void AttachMissingAttachments(ClassAnnouncement classAnn, IList<AnnouncementAssignedAttribute> attributes, UnitOfWork u
            , ConnectorLocator connectorLocator, IServiceLocatorSchool serviceLocator)
        {
            attributes = UploadMissingAttachments(classAnn, attributes, u, connectorLocator, serviceLocator);
            new AnnouncementAssignedAttributeDataAccess(u).Update(attributes);
        }

        private static IList<AnnouncementAssignedAttribute> UploadMissingAttachments(ClassAnnouncement classAnn, IList<AnnouncementAssignedAttribute> attributes, UnitOfWork u
            , ConnectorLocator connectorLocator, IServiceLocatorSchool serviceLocator)
        {
            var attributesForUpdate = attributes.Where(x => x.Attachment != null && x.Attachment.SisAttachmentId.HasValue && !x.AttachmentRef.HasValue).ToList();
            if(attributesForUpdate.Count == 0) return new List<AnnouncementAssignedAttribute>();
            var da = new AttachmentDataAccess(u);
            var existingsAtts = da.GetBySisAttachmentIds(attributesForUpdate.Select(x => x.Attachment.SisAttachmentId.Value).ToList());
            var attsForUpload = new List<Attachment>();
            foreach (var attribute in attributesForUpdate)
            {
                var attachment = attribute.Attachment;
                var existingAtt = existingsAtts.FirstOrDefault(x => x.SisAttachmentId == attachment.SisAttachmentId);
                if (existingAtt == null)
                {
                    var content = connectorLocator.AttachmentConnector.GetAttachmentContent(attribute.Attachment.SisAttachmentId.Value);
                    if(serviceLocator.CrocodocService.IsDocument(attribute.Attachment.Name))
                        attachment.Uuid = serviceLocator.CrocodocService.UploadDocument(attribute.Attachment.Name, content).uuid;
                    attachment.UploadedDate = classAnn.Created;
                    attachment.LastAttachedDate = attachment.UploadedDate;
                    attachment.PersonRef = classAnn.PrimaryTeacherRef;
                    attsForUpload.Add(attachment);
                }
                else attribute.AttachmentRef = existingAtt.Id;
            }
            if (attsForUpload.Count > 0)
            {
                da.Insert(attsForUpload);
                existingsAtts = da.GetBySisAttachmentIds(attsForUpload.Select(x => x.SisAttachmentId.Value).ToList());
                foreach (var attribute in attributesForUpdate.Where(x => !x.AttachmentRef.HasValue))
                {
                    var att = existingsAtts.First(x => x.SisAttachmentId == attribute.Attachment.SisAttachmentId);
                    attribute.Attachment = att;
                    attribute.AttachmentRef = att.Id;
                }    
            }
            return attributesForUpdate;
        }


        public static IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, IList<int> toAnnouncementIds, 
            UnitOfWork unitOfWork, IServiceLocatorSchool serviceLocator, ConnectorLocator connectorLocator)
        {
            var da = new AnnouncementAssignedAttributeDataAccess(unitOfWork);
            var attributesForCopying = da.GetListByAnntId(fromAnnouncementId);
            attributesForCopying = attributesForCopying.Where(x => !x.SisActivityAssignedAttributeId.HasValue).ToList();

            var attributes = new List<AnnouncementAssignedAttribute>();
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
                    if (attributeForCopying.Attachment != null)
                    {
                        var attContent = serviceLocator.AttachementService.GetAttachmentContent(attributeForCopying.Attachment);
                        if (attContent.Content != null)
                        {
                            var att = AttachmentService.Upload(attContent.Attachment.Name, attContent.Content, attContent.Attachment.IsStiAttachment, unitOfWork, serviceLocator, connectorLocator);
                            attribute.AttachmentRef = att.Id;
                            attribute.Attachment = att;
                        }
                    }
                    attributes.Add(attribute);
                }
            }
            da.Insert(attributes);
            return da.GetLastListByAnnIds(toAnnouncementIds, attributes.Count);
        }

        public static IList<Pair<AnnouncementAssignedAttribute, AnnouncementAssignedAttribute>> CopyNonStiAttributes(IDictionary<int, int> fromToAnnouncementIds, 
            UnitOfWork unitOfWork, IServiceLocatorSchool serviceLocator, ConnectorLocator connectorLocator)
        {
            var attachmentDA = new AttachmentDataAccess(unitOfWork);

            var annAssignedAttributeDA = new AnnouncementAssignedAttributeDataAccess(unitOfWork);
            var attributesForCopying = annAssignedAttributeDA.GetLastListByAnnIds(fromToAnnouncementIds.Select(x => x.Key).ToList(), int.MaxValue)
                .Where(x => !x.SisActivityAssignedAttributeId.HasValue).ToList();

            var fromToAttributes = new List<Pair<AnnouncementAssignedAttribute, AnnouncementAssignedAttribute>>();
            
            foreach (var announcementPair in fromToAnnouncementIds)
            {
                var assignedAttToCopy = attributesForCopying.Where(x => x.AnnouncementRef == announcementPair.Key).ToList();
                foreach (var attributeToCopy in assignedAttToCopy)
                {
                    var newAttribute = new AnnouncementAssignedAttribute
                    {
                        AnnouncementRef = announcementPair.Value,
                        AttributeTypeId = attributeToCopy.AttributeTypeId,
                        Name = attributeToCopy.Name,
                        Text = attributeToCopy.Text,
                        VisibleForStudents = attributeToCopy.VisibleForStudents
                    };

                    if (attributeToCopy.Attachment != null)
                    {
                        var attachment = new Attachment
                        {
                            Name = attributeToCopy.Attachment.Name,
                            PersonRef = serviceLocator.Context.PersonId.Value,
                            Uuid = null,
                            UploadedDate = serviceLocator.Context.NowSchoolTime,
                            LastAttachedDate = serviceLocator.Context.NowSchoolTime,
                        };

                        attachment.Id = attachmentDA.InsertWithEntityId(attachment);

                        newAttribute.AttachmentRef = attachment.Id;
                        newAttribute.Attachment = attachment;
                    }
                    fromToAttributes.Add(new Pair<AnnouncementAssignedAttribute, AnnouncementAssignedAttribute>(attributeToCopy, newAttribute));
                }
            }

            annAssignedAttributeDA.Insert(fromToAttributes.Select(x => x.Second).ToList());
            return fromToAttributes;
        }

        public IList<AnnouncementAssignedAttribute> CopyNonStiAttributes(int fromAnnouncementId, int toAnnouncementId)
        {
            IList<AnnouncementAssignedAttribute> res = null;
            DoUpdate(u=> { res = CopyNonStiAttributes(fromAnnouncementId, new List<int> {toAnnouncementId}, u, ServiceLocator, ConnectorLocator); });
            return res;
        }

        public void ValidateAttributes(IList<AnnouncementAssignedAttribute> attributes)
        {
            if (attributes.Any(attr => string.IsNullOrWhiteSpace(attr.Text)))
                throw new ChalkableException("Looks like you forgot to include text with your attached attribute.");
            
        }
    }
}
