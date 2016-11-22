using System.Collections.Generic;
using System.Linq;
using Chalkable.Common.Exceptions;
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
            ann.SisActivityId = activity.Id;
        }
    }

    public class ActivityToAnnouncementComplexMapper : BaseMapper<AnnouncementComplex, Activity>
    {
        protected override void InnerMap(AnnouncementComplex ann, Activity activity)
        {
            MapperFactory.GetMapper<Announcement, Activity>().Map(ann, activity);
            if(ann.ClassAnnouncementData == null)
                throw new ChalkableException("Only classAnnouncement can be mapped from activity");
            MapperFactory.GetMapper<ClassAnnouncement, Activity>().Map(ann.ClassAnnouncementData, activity);
            ann.Complete = activity.Complete;
            if (activity.Score != null)
            {
                ann.CurrentStudentScore = ann.CurrentStudentScore ?? new StudentAnnouncementDetails();
                MapperFactory.GetMapper<StudentAnnouncementDetails, Score>().Map(ann.CurrentStudentScore, activity.Score);
            }
        }
    }

    public class ActivityToAnnouncementDetailsMapper : BaseMapper<AnnouncementDetails, Activity>
    {
        protected override void InnerMap(AnnouncementDetails annDetails, Activity activity)
        {
            MapperFactory.GetMapper<AnnouncementComplex, Activity>().Map(annDetails, activity);

            if (annDetails.AnnouncementAttributes == null)
                annDetails.AnnouncementAttributes = new List<AnnouncementAssignedAttribute>();

            annDetails.AnnouncementAttributes = annDetails.AnnouncementAttributes.Where(a => activity.Attributes != null 
                && activity.Attributes.Any(x => x.Id == a.SisActivityAssignedAttributeId)).ToList();

            if (activity.Attributes != null && activity.Attributes.Any())
            {
                foreach (var attribute in activity.Attributes)
                {
                    var annAttribute = annDetails.AnnouncementAttributes.FirstOrDefault(aa => aa.SisActivityAssignedAttributeId == attribute.Id);
                    if (annAttribute == null)
                    {
                        annAttribute = new AnnouncementAssignedAttribute {AnnouncementRef = annDetails.Id};
                        annDetails.AnnouncementAttributes.Add(annAttribute);
                    }
                    MapperFactory.GetMapper<AnnouncementAssignedAttribute, ActivityAssignedAttribute>().Map(annAttribute, attribute);
                }
            }

            if (annDetails.AnnouncementStandards == null || activity.Standards == null || !activity.Standards.Any())
                annDetails.AnnouncementStandards = new List<AnnouncementStandardDetails>();
            
            if (activity.Standards != null && activity.Standards.Any())
            {
                annDetails.AnnouncementStandards = annDetails.AnnouncementStandards.Where(a => activity.Standards.Any(s => s.Id == a.StandardRef)).ToList();
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
