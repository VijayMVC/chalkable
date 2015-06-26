using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class ActivityToAnnouncementMapper : BaseMapper<Announcement, Activity>
    {
        protected override void InnerMap(Announcement ann, Activity activity)
        {
            ann.Content = activity.Description;
            ann.MaxScore = activity.MaxScore;
            ann.ClassRef = activity.SectionId;
            ann.Expires = activity.Date;
            ann.MayBeDropped = activity.MayBeDropped;
            ann.WeightAddition = activity.WeightAddition;
            ann.WeightMultiplier = activity.WeightMultiplier;
            ann.Dropped = activity.IsDropped;
            ann.ClassAnnouncementTypeRef = activity.CategoryId;
            ann.VisibleForStudent = activity.DisplayInHomePortal;
            ann.Title = activity.Name;
            ann.MayBeExempt = activity.MayBeExempt;
            ann.IsScored = activity.IsScored;
            ann.Complete = activity.Complete;
            if (ann is AnnouncementComplex)
            {
                var annC = ann as AnnouncementComplex;
                annC.ClassAnnouncementTypeName = activity.CategoryName;
                annC.AttachmentsCount = activity.Attributes != null ? activity.Attributes.Count(x => x.Attachment != null) : 0;
                //annC.Starred = activity.Starred;
            }
            if (ann is AnnouncementDetails)
            {
                var annDetails = ann as AnnouncementDetails;
                if (annDetails.AnnouncementAttachments == null)
                    annDetails.AnnouncementAttachments = new List<AnnouncementAttachment>();

                var activityAtts = activity.Attributes != null ? activity.Attributes.Where(x => x.Attachment != null).ToList() : new List<ActivityAttribute>();
                annDetails.AnnouncementAttachments = annDetails.AnnouncementAttachments
                                    .Where(x => !x.SisAttachmentId.HasValue
                                              || (activityAtts.Any(att => att.Attachment.AttachmentId == x.SisAttachmentId)))
                                    .ToList();
                if (activityAtts.Any())
                {
                    foreach (var att in activityAtts)
                    {
                        var annAtt = annDetails.AnnouncementAttachments.FirstOrDefault(x => x.SisAttachmentId == att.Attachment.AttachmentId);
                        if (annAtt == null)
                        {
                            annAtt = new AnnouncementAttachment
                            {
                                AnnouncementRef = annDetails.Id,
                                SisAttachmentId = att.Attachment.AttachmentId
                            };
                            annDetails.AnnouncementAttachments.Add(annAtt);
                        }
                        MapperFactory.GetMapper<AnnouncementAttachment, ActivityAttribute>().Map(annAtt, att);
                    }
                }
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

}
