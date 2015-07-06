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


                if (annDetails.AnnouncementAttributes == null)
                    annDetails.AnnouncementAttributes = new List<AnnouncementAssignedAttribute>();

                var activityAtts = activity.Attributes != null ? activity.Attributes.ToList() : new List<ActivityAssignedAttribute>();

                foreach (var att in activityAtts)
                {
                    var annAtt = annDetails.AnnouncementAttributes.FirstOrDefault(x => x.Id == att.Id);
                    if (annAtt == null)
                    {
                        annAtt = new AnnouncementAssignedAttribute();
                        annDetails.AnnouncementAttributes.Add(annAtt);
                    }
                    MapperFactory.GetMapper<AnnouncementAssignedAttribute, ActivityAssignedAttribute>().Map(annAtt, att);
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
