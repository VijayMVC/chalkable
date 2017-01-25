using System.Collections.Generic;
using System.Linq;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.BusinessLogic.Mapping.ModelMappers
{
    public class AnnouncementToActivityMapper : BaseMapper<Activity, Announcement>
    {
        protected override void InnerMap(Activity activity, Announcement ann)
        {
            activity.Name = ann.Title;
            activity.Unit = string.Empty;
            activity.Description = ann.Content;
        }
    }

    public class ClassAnnouncementToActivityMapper : BaseMapper<Activity, ClassAnnouncement>
    {
        protected override void InnerMap(Activity activity, ClassAnnouncement ann)
        {
            MapperFactory.GetMapper<Activity, Announcement>().Map(activity, ann);

            activity.Date = ann.Expires;
            activity.CategoryId = ann.ClassAnnouncementTypeRef;
            activity.IsDropped = ann.Dropped;
            activity.MaxScore = ann.MaxScore;
            activity.WeightAddition = ann.WeightAddition;
            activity.WeightMultiplier = ann.WeightMultiplier;
            activity.MayBeDropped = ann.MayBeDropped;
            activity.Description = ann.Content;
            activity.DisplayInHomePortal = ann.VisibleForStudent;
            activity.MayBeExempt = ann.MayBeExempt;
            activity.IsScored = ann.IsScored;
            activity.SectionId = ann.ClassRef;
        }
    }

    public class AnnouncementComplexToActivityMapper : BaseMapper<Activity, AnnouncementComplex>
    {
        protected override void InnerMap(Activity activity, AnnouncementComplex ann)
        {
            MapperFactory.GetMapper<Activity, ClassAnnouncement>().Map(activity, ann.ClassAnnouncementData);
            activity.Complete = ann.Complete;
            if (ann.CurrentStudentScore != null)
            {
                activity.Score = activity.Score ?? new Score();
                MapperFactory.GetMapper<Score, StudentAnnouncementDetails>().Map(activity.Score, ann.CurrentStudentScore);
            }
        }
    }

    public class AnnouncementDetailsToActivityMapper : BaseMapper<Activity, AnnouncementDetails>
    {
        protected override void InnerMap(Activity activity, AnnouncementDetails annDetails)
        {
            MapperFactory.GetMapper<Activity, AnnouncementComplex>().Map(activity, annDetails);
            if (annDetails.AnnouncementAttributes != null && annDetails.AnnouncementAttributes.Count > 0)
            {
                if (activity.Attributes == null)
                    activity.Attributes = new List<ActivityAssignedAttribute>();

                var newAtts = new List<ActivityAssignedAttribute>();

                foreach (var annAtt in annDetails.AnnouncementAttributes)
                {
                    var att =  activity.Attributes.FirstOrDefault(x => x.Id == annAtt.SisActivityAssignedAttributeId);
                    if (att == null)
                    {
                        att = new ActivityAssignedAttribute { ActivityId = activity.Id };
                        newAtts.Add(att);   
                    }
                    MapperFactory.GetMapper<ActivityAssignedAttribute, AnnouncementAssignedAttribute>().Map(att, annAtt);
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


