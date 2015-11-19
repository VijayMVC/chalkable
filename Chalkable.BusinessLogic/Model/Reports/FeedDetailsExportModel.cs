using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class FeedDetailsExportModel : ShortFeedExportModel
    {
        public string AnnouncementDescription { get; set; }
        public string AnnouncementType { get; set; }

        public bool IsLessonPlan { get; set; }
        public bool IsAdminAnnouncement { get; set; } 
        public decimal? TotalPoint { get; set; }
        public decimal? WeightAddition { get; set; }
        public decimal? WeigntMultiplier { get; set; }

        public int? StandardId { get; set; }
        public string StandardName { get; set; }
        public string StandardDescription { get; set; }

        public int? AnnouncementAttachmentId { get; set; }
        public string AnnouncementAttachmentName { get; set; }
        
        public int? AttributeId { get; set; }
        public int? SisAttributeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeDescription { get; set; }
        public string AttributeAttachmentName { get; set; }

        private class AttachmentItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public byte[] Image { get; set; }
            public int Order { get; set; }
        }

        public FeedDetailsExportModel() { }

        protected FeedDetailsExportModel(Person person, string schoolName, string sy, AnnouncementDetails ann
            , IList<ClassTeacher> classTeachers, IList<Staff> staffs, Standard standard, AnnouncementAssignedAttribute attribute)
            : base(person, schoolName, sy)
        {
            AnnouncementId = ann.Id;
            AnnouncementName = ann.Title;
            AnnouncementDescription = ann.Content;
            Complete = ann.Complete;
            if (ann.LessonPlanData != null)
            {
                IsLessonPlan = true;
                StartDate = ann.LessonPlanData.StartDate;
                EndDate = ann.LessonPlanData.EndDate;
                IsHidden = !ann.LessonPlanData.VisibleForStudent;
                ClassId = ann.LessonPlanData.ClassRef;
                ClassName = ann.LessonPlanData.FullClassName;
                var currentClassTeachers = classTeachers.Where(x => x.ClassRef == ClassId);
                ClassTeacherNames = BuildTeachersNames(ann.LessonPlanData.PrimaryTeacherRef, classTeachers, staffs);
            }
            if (ann.ClassAnnouncementData != null)
            {
                EndDate = ann.ClassAnnouncementData.Expires;
                IsHidden = !ann.ClassAnnouncementData.VisibleForStudent;
                ClassId = ann.ClassAnnouncementData.ClassRef;
                ClassName = ann.ClassAnnouncementData.FullClassName;
                var currentClassTeachers = classTeachers.Where(x => x.ClassRef == ClassId);
                ClassTeacherNames = BuildTeachersNames(ann.ClassAnnouncementData.PrimaryTeacherRef, classTeachers, staffs);
                TotalPoint = ann.ClassAnnouncementData.MaxScore;
                WeightAddition = ann.ClassAnnouncementData.WeightAddition;
                WeigntMultiplier = ann.ClassAnnouncementData.WeightMultiplier;
            }
            if (ann.AdminAnnouncementData != null)
            {
                IsAdminAnnouncement = true;
                EndDate = ann.AdminAnnouncementData.Expires;
            }
            if (standard != null)
            {
                StandardId = standard.Id;
                StandardName = standard.Name;
                StandardDescription = standard.Description;
            }
            if (attribute != null)
            {
                AttributeId = attribute.Id;
                AttributeName = attribute.Name;
                AttributeDescription = attribute.Text;
                if (attribute.Attachment != null)
                    AttributeAttachmentName = attribute.Attachment.Name;
            }
        }

        public static IList<FeedDetailsExportModel> Create(Person person, string schoolName, string schoolYear,
            IList<AnnouncementDetails> anns, IList<ClassTeacher> classTeachers, 
            IList<Staff> staffs, IList<ScheduleItem> scheduleItems, IList<Application> apps)
        {
            foreach (var scheduleItem in scheduleItems)
            {
                
            }
            var res = new List<FeedDetailsExportModel>();
            foreach (var ann in anns)
            {
                var attachmentItems = PrepareAttachmentItems(ann.AnnouncementAttachments, ann.AnnouncementApplications, apps);
                var items = ann.AnnouncementStandards.SelectMany(
                            sa => ann.AnnouncementAttributes.SelectMany(
                            aa => attachmentItems.Select(
                            attItem => Create(person, schoolName, schoolYear, ann, classTeachers, staffs, sa.Standard, aa, attItem)
                            ))).ToList();
                items.AddRange(items);
            }
            return res;
        }

        private static FeedDetailsExportModel Create(Person person, string schoolName, string sy, AnnouncementDetails ann, IList<ClassTeacher> classTeachers
            , IList<Staff> staffs, Standard standard, AnnouncementAssignedAttribute attribute, AttachmentItem attachmentItem)
        {
            var res = new FeedDetailsExportModel(person, schoolName, sy, ann, classTeachers, staffs, standard, attribute);
            if (attachmentItem != null)
            {
                res.AnnouncementAttachmentId = attachmentItem.Id;
                res.AnnouncementAttachmentName = attachmentItem.Name;
            }
            return res;
        }

        private static IList<AttachmentItem> PrepareAttachmentItems(IList<AnnouncementAttachment> annAtts,
            IList<AnnouncementApplication> annApps, IList<Application> apps)
        {

            var res = new List<AttachmentItem>();
            if (annAtts != null && annAtts.Count > 0)
                res.AddRange(annAtts.Select(x => new AttachmentItem
                {
                    Id = x.Id,
                    Name = x.Attachment.Name
                }));
            if (annApps != null && annApps.Count > 0 && apps != null && apps.Count > 0)
            {
                foreach (var annApp in annApps)
                {
                    var app = apps.FirstOrDefault(a => a.Id == annApp.ApplicationRef);
                    if (app == null) continue;
                    res.Add(new AttachmentItem
                    {
                        Id = annApp.Id,
                        Name = app.Name,
                        Order = annApp.Order
                    });
                }
            }
            return res.OrderBy(x => x.Order).ToList();
        }
    }
}
