using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (ann is AnnouncementDetails && activity.Attachments != null && activity.Attachments.Any())
            {
                var annDetails = ann as AnnouncementDetails;
                if (annDetails.AnnouncementAttachments == null)
                    annDetails.AnnouncementAttachments = new List<AnnouncementAttachment>();
                foreach (var att in activity.Attachments)
                {
                    var annAtt = annDetails.AnnouncementAttachments.FirstOrDefault(x => x.SisAttachmentId == att.AttachmentId);
                    if (annAtt == null)
                    {
                        annAtt = new AnnouncementAttachment
                        {
                            AnnouncementRef = annDetails.Id,
                            SisAttachmentId = att.AttachmentId
                        };
                        annDetails.AnnouncementAttachments.Add(annAtt);
                    }
                    MapperFactory.GetMapper<AnnouncementAttachment, ActivityAttachment>().Map(annAtt, att);
                }

                if (activity.Standards != null && activity.Standards.Any())
                {
                    if (annDetails.AnnouncementStandards == null)
                        annDetails.AnnouncementStandards = new List<AnnouncementStandardDetails>();
                    foreach (var activityStandard in activity.Standards)
                    {
                        var annStandard = annDetails.AnnouncementStandards.FirstOrDefault(x => x.StandardRef == activityStandard.StandardId);
                        if (annStandard == null)
                        {
                            annStandard = new AnnouncementStandardDetails() { AnnouncementRef = annDetails.Id };
                            annDetails.AnnouncementStandards.Add(annStandard);
                        }
                        annStandard.StandardRef = activityStandard.StandardId;
                        if (annStandard.Standard == null)
                            annStandard.Standard = new Standard();
                        annStandard.Standard.Id = activityStandard.StandardId;
                        annStandard.Standard.Name = activityStandard.StandardName;
                    }
                }
            }
        }
    }

}
