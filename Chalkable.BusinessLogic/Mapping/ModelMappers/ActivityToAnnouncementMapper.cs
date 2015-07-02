using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class ActivityToAnnouncementMapper : BaseMapper<Announcement, Activity>
    {
        protected override void InnerMap(Announcement ann, Activity activity)
        {
            ann.Content = activity.Description;
            ann.Title = activity.Name;
        }
    }

    public class ActivityToClassAnnouncementMapper : BaseMapper<ClassAnnouncement, Activity>
    {
        protected override void InnerMap(ClassAnnouncement ann, Activity activity)
        {
            MapperFactory.GetMapper<Announcement, Activity>().Map(ann, activity);
            ann.MayBeExempt = activity.MayBeExempt;
            ann.IsScored = activity.IsScored;
            ann.MaxScore = activity.MaxScore;
            ann.ClassRef = activity.SectionId;
            ann.Expires = activity.Date;
            ann.MayBeDropped = activity.MayBeDropped;
            ann.WeightAddition = activity.WeightAddition;
            ann.WeightMultiplier = activity.WeightMultiplier;
            ann.Dropped = activity.IsDropped;
            ann.ClassAnnouncementTypeRef = activity.CategoryId;
            ann.VisibleForStudent = activity.DisplayInHomePortal;
            ann.ClassAnnouncementTypeName = activity.CategoryName;
        }
    }

    public class ActivityToAnnouncementComplexMapper : BaseMapper<AnnouncementComplex, Activity>
    {
        protected override void InnerMap(AnnouncementComplex ann, Activity activity)
        {
            MapperFactory.GetMapper<Announcement, Activity>().Map(ann, activity);
            MapperFactory.GetMapper<ClassAnnouncement, Activity>().Map(ann.ClassAnnouncementData, activity);
           // ann.AttachmentsCount = activity.Attachments != null ? activity.Attachments.Count() : 0;
        }
    }

    public class ActivityToAnnouncementDetailsMapper : BaseMapper<AnnouncementDetails, Activity>
    {
        protected override void InnerMap(AnnouncementDetails annDetails, Activity activity)
        {
            MapperFactory.GetMapper<AnnouncementComplex, Activity>().Map(annDetails, activity);

            if (annDetails.AnnouncementAttachments == null)
                annDetails.AnnouncementAttachments = new List<AnnouncementAttachment>();

            //annDetails.AnnouncementAttachments = annDetails.AnnouncementAttachments
            //                    .Where(x => !x.SisAttachmentId.HasValue
            //                              || (activity.Attachments != null && activity.Attachments.Any(att => att.AttachmentId == x.SisAttachmentId)))
            //                    .ToList();
            //if (activity.Attachments != null && activity.Attachments.Any())
            //{
            //    foreach (var att in activity.Attachments)
            //    {
            //        var annAtt = annDetails.AnnouncementAttachments.FirstOrDefault(x => x.SisAttachmentId == att.AttachmentId);
            //        if (annAtt == null)
            //        {
            //            annAtt = new AnnouncementAttachment
            //            {
            //                AnnouncementRef = annDetails.Id,
            //                SisAttachmentId = att.AttachmentId
            //            };
            //            annDetails.AnnouncementAttachments.Add(annAtt);
            //        }
            //        MapperFactory.GetMapper<AnnouncementAttachment, ActivityAttachment>().Map(annAtt, att);
            //    }
            //}
            if (annDetails.AnnouncementStandards == null)
                annDetails.AnnouncementStandards = new List<AnnouncementStandardDetails>();
            if (activity.Standards != null && activity.Standards.Any())
            {
                foreach (var activityStandard in activity.Standards)
                {
                    var annStandard = annDetails.AnnouncementStandards.FirstOrDefault(x => x.StandardRef == activityStandard.Id);
                    if (annStandard == null)
                    {
                        annStandard = new AnnouncementStandardDetails { AnnouncementRef = annDetails.Id };
                        annDetails.AnnouncementStandards.Add(annStandard);
                    }
                    annStandard.StandardRef = activityStandard.Id;
                    if (annStandard.Standard == null)
                        annStandard.Standard = new Standard();
                    annStandard.Standard.Id = activityStandard.Id;
                    annStandard.Standard.Name = activityStandard.Name;
                }
            }
        }
    }
    
}
