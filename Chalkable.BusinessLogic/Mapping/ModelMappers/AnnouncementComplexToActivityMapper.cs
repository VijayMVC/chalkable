using System.Collections.Generic;
using System.Linq;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class AnnouncementComplexToActivityMapper : BaseMapper<Activity, AnnouncementComplex>
    {
        protected override void InnerMap(Activity activity, AnnouncementComplex ann)
        {
            if(!ann.ClassRef.HasValue)
                throw new ChalkableException("Can't map announcement to activity that is not assigned to class");

            activity.Date = ann.Expires;
            activity.CategoryId = ann.ClassAnnouncementTypeRef;
            activity.IsDropped = ann.Dropped;
            activity.MaxScore = ann.MaxScore;
            activity.WeightAddition = ann.WeightAddition;
            activity.WeightMultiplier = ann.WeightMultiplier;
            activity.MayBeDropped = ann.MayBeDropped;
            activity.Description = ann.Content;
            activity.DisplayInHomePortal = ann.VisibleForStudent;
            activity.Name = ann.Title;
            activity.Unit = string.Empty;
            activity.MayBeExempt = ann.MayBeExempt;
            activity.IsScored = ann.IsScored;
            activity.SectionId = ann.ClassRef.Value;
            activity.Complete = ann.Complete;
            var annDetails = ann as AnnouncementDetails;
            if (annDetails != null)
            {
                if (annDetails.AnnouncementAttachments != null && annDetails.AnnouncementAttachments.Count > 0)
                {
                    if (activity.Attributes == null)
                        activity.Attributes = new List<ActivityAssignedAttribute>();
                    var newAtts = new List<ActivityAssignedAttribute>();
                    foreach (var annAtt in annDetails.AnnouncementAttachments)
                    {
                        if (annAtt.SisAttachmentId.HasValue)
                        {
                            var att = activity.Attributes.FirstOrDefault(x => x.Attachment != null && x.Attachment.AttachmentId == annAtt.SisAttachmentId);
                            if (att == null)
                            {
                                att = new ActivityAssignedAttribute
                                    {
                                        ActivityId = activity.Id,
                                        Attachment = new StiAttachment()
                                    };
                                newAtts.Add(att);
                            }
                            MapperFactory.GetMapper<StiAttachment, AnnouncementAttachment>().Map(att.Attachment, annAtt);
                        }
                    }
                    activity.Attributes = activity.Attributes.Concat(newAtts);
                }
                if (annDetails.AnnouncementStandards != null && annDetails.AnnouncementStandards.Count > 0)
                {
                    if (activity.Standards == null)
                        activity.Standards = new LinkedList<ActivityStandard>();
                    var newStandards = new List<ActivityStandard>();
                    foreach (var annStandard in annDetails.AnnouncementStandards)
                    {
                        if (annStandard.Standard != null)
                        {
                            var activitySt = activity.Standards.FirstOrDefault(x => x.Id == annStandard.StandardRef);
                            if (activitySt == null)
                            {
                                activitySt = new ActivityStandard();
                                newStandards.Add(activitySt);
                            }
                            activitySt.Id = annStandard.Standard.Id;
                            activitySt.Name = annStandard.Standard.Name;
                        }
                    }
                    activity.Standards = activity.Standards.Concat(newStandards);
                }
            }
        }
    }

}
