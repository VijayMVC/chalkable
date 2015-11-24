using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common.Web;
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
        public bool ShowScoreSettings { get; set; } 
        public double? TotalPoint { get; set; }
        public double? WeightAddition { get; set; }
        public double? WeigntMultiplier { get; set; }

        public int? StandardId { get; set; }
        public string StandardName { get; set; }
        public string StandardDescription { get; set; }

        public int? AnnouncementAttachmentId { get; set; }
        public string AnnouncementAttachmentName { get; set; }
        public byte[] AnnouncementAttachmentImage { get; set; }
        public int AnnouncementAttachmentOrder { get; set; }
        public bool Document { get; set; }

        public int? AttributeId { get; set; }
        public int? SisAttributeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeDescription { get; set; }
        public string AttributeAttachmentName { get; set; }
        public byte[] AttrinuteAttachmentImage { get; set; }

        private class AttachmentItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public byte[] Image { get; set; }
            public int Order { get; set; }
            public bool Document { get; set; }
        }

        public FeedDetailsExportModel() { }

        protected FeedDetailsExportModel(Person person, string schoolName, string sy, AnnouncementDetails ann
            , IList<ClassTeacher> classTeachers, IList<Staff> staffs, Standard standard
            , AnnouncementAssignedAttribute attribute, IDictionary<int, byte[]> attributeImage)
            : base(person, schoolName, sy)
        {
            AnnouncementId = ann.Id;
            AnnouncementType = ann.AnnouncementTypeName;
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
                var currentClassTeachers = classTeachers.Where(x => x.ClassRef == ClassId).ToList();
                ClassTeacherNames = BuildTeachersNames(ann.LessonPlanData.PrimaryTeacherRef, currentClassTeachers, staffs);
            }
            if (ann.ClassAnnouncementData != null)
            {
                EndDate = ann.ClassAnnouncementData.Expires;
                IsHidden = !ann.ClassAnnouncementData.VisibleForStudent;
                ClassId = ann.ClassAnnouncementData.ClassRef;
                ClassName = ann.ClassAnnouncementData.FullClassName;
                var currentClassTeachers = classTeachers.Where(x => x.ClassRef == ClassId).ToList();
                ClassTeacherNames = BuildTeachersNames(ann.ClassAnnouncementData.PrimaryTeacherRef, currentClassTeachers, staffs);
                TotalPoint = (double?) ann.ClassAnnouncementData.MaxScore;
                WeightAddition = (double?) ann.ClassAnnouncementData.WeightAddition;
                WeigntMultiplier = (double?) ann.ClassAnnouncementData.WeightMultiplier;
                ShowScoreSettings = CanShowScoreSettings(ann.ClassAnnouncementData);
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
                {
                    AttributeAttachmentName = attribute.Attachment.Name;
                    AttrinuteAttachmentImage = attributeImage.ContainsKey(attribute.Attachment.Id) ? attributeImage[attribute.Attachment.Id] : null;
                }
            }
        }

        private static bool CanShowScoreSettings(ClassAnnouncement ann)
        {
            return ann.MaxScore.HasValue && ann.WeightAddition.HasValue && ann.WeightMultiplier.HasValue
                   && (ann.WeightAddition != ClassAnnouncement.DEFAULT_WEIGHT_ADDITION ||
                       ann.WeightMultiplier != ClassAnnouncement.DEFAULT_WEGIHT_MULTIPLIER);
        }

        public static IList<FeedDetailsExportModel> Create(Person person, string schoolName, string schoolYear,
            IList<AnnouncementDetails> anns, IList<ClassTeacher> classTeachers, IList<Staff> staffs,
            IList<ScheduleItem> scheduleItems, IList<Application> apps, IDictionary<Guid, byte[]> appsImages, IDictionary<int, byte[]> attsImages)
        {
            foreach (var scheduleItem in scheduleItems)
            {
                
            }
            var res = new List<FeedDetailsExportModel>();
            foreach (var ann in anns)
            {
                var attachmentItems = PrepareAttachmentItems(ann.AnnouncementAttachments, ann.AnnouncementApplications, apps, appsImages, attsImages);
                var items = ann.AnnouncementStandards.SelectMany(
                            sa => ann.AnnouncementAttributes.SelectMany(
                            aa => attachmentItems.Select(
                            attItem => Create(person, schoolName, schoolYear, ann, classTeachers, staffs, sa.Standard, aa, attItem, attsImages)
                            ))).ToList();
                res.AddRange(items);
            }
            return res;
        }

        private static FeedDetailsExportModel Create(Person person, string schoolName, string sy, AnnouncementDetails ann, IList<ClassTeacher> classTeachers
            , IList<Staff> staffs, Standard standard, AnnouncementAssignedAttribute attribute, AttachmentItem attachmentItem, IDictionary<int, byte[]> attsImages)
        {
            var res = new FeedDetailsExportModel(person, schoolName, sy, ann, classTeachers, staffs, standard, attribute, attsImages);
            if (attachmentItem != null)
            {
                res.AnnouncementAttachmentId = attachmentItem.Id;
                res.AnnouncementAttachmentName = attachmentItem.Name;
                res.Document = attachmentItem.Document;
                res.AnnouncementAttachmentImage = attachmentItem.Image;
            }
            return res;
        }

        private static IList<AttachmentItem> PrepareAttachmentItems(IList<AnnouncementAttachment> annAtts, IList<AnnouncementApplication> annApps
            , IList<Application> apps, IDictionary<Guid, byte[]> appsImages, IDictionary<int, byte[]> attsImages)
        {

            var res = new List<AttachmentItem>();
            if (annAtts != null && annAtts.Count > 0)
                res.AddRange(annAtts.Select(x => new AttachmentItem
                {
                    Id = x.Id,
                    Name = x.Attachment.Name,
                    Order = x.Order,
                    Document = x.Attachment.IsDocument,
                    Image = attsImages.ContainsKey(x.AttachmentRef) ? attsImages[x.AttachmentRef] : null
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
                        Order = annApp.Order,
                        Document = false,
                        Image = appsImages.ContainsKey(annApp.ApplicationRef) ? appsImages[annApp.ApplicationRef] : null
                    });
                }
            }
            return res.OrderBy(x => x.Order).ToList();
        }
    }
}
