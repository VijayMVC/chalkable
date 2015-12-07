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
        public bool IsLessonPlan { get; set; }
        public bool ShowScoreSettings { get; set; } 
        public double? TotalPoint { get; set; }
        public double? WeightAddition { get; set; }
        public double? WeigntMultiplier { get; set; }
        public bool HasStandards { get; set; }
        public bool HasAttachments { get; set; }
        public bool HasAttributes { get; set; }

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

        private class AttachmentItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public byte[] Image { get; set; }
            public int Order { get; set; }
            public bool Document { get; set; }
        }
        
        public FeedDetailsExportModel() { }

        protected FeedDetailsExportModel(Person person, string schoolName, string sy, DateTime nowSchoolTime, AnnouncementDetails ann
            , ClassDetails classDetails, IList<DayType> dayTypes, IList<Staff> staffs, Standard standard, AnnouncementAssignedAttribute attribute)
            : base(person, schoolName, sy, nowSchoolTime, classDetails, dayTypes, staffs, ann)
        {
            AnnouncementDescription = ann.Content;
            HasStandards = ann.AnnouncementStandards.Count > 0;
            HasAttachments = ann.AnnouncementAttachments.Count + ann.AnnouncementApplications.Count > 0;
            HasAttributes = ann.AnnouncementAttributes.Count > 0;
            IsLessonPlan = ann.LessonPlanData != null;

            if (ann.ClassAnnouncementData != null)
            {
                TotalPoint = (double?) ann.ClassAnnouncementData.MaxScore ?? ClassAnnouncement.DEFAULT_MAX_SCORE;
                WeightAddition = (double?) ann.ClassAnnouncementData.WeightAddition ?? ClassAnnouncement.DEFAULT_WEIGHT_ADDITION;
                WeigntMultiplier = (double?) ann.ClassAnnouncementData.WeightMultiplier ?? ClassAnnouncement.DEFAULT_WEGIHT_MULTIPLIER;
                ShowScoreSettings = CanShowScoreSettings(ann.ClassAnnouncementData);
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
                }
            }
        }

        private static bool CanShowScoreSettings(ClassAnnouncement ann)
        {
            return ann.WeightAddition.HasValue && ann.WeightMultiplier.HasValue
                   && (ann.WeightAddition != ClassAnnouncement.DEFAULT_WEIGHT_ADDITION ||
                       ann.WeightMultiplier != ClassAnnouncement.DEFAULT_WEGIHT_MULTIPLIER);
        }

        public static IList<FeedDetailsExportModel> Create(Person person, string schoolName, string schoolYear, DateTime nowSchoolTime,
            IList<AnnouncementDetails> anns, IList<ClassDetails> classDetailses, IList<DayType> dayTypes,
            IList<Staff> staffs, IList<Application> apps, IDictionary<Guid, byte[]> appsImages)
        {
            
            var res = new List<FeedDetailsExportModel>();
            foreach (var c in classDetailses)
            {
                var selectedAnns = anns.Where(x => x.ClassRef == c.Id).ToList();
                if(selectedAnns.Count == 0) continue;
                selectedAnns = selectedAnns.OrderBy(x =>
                {
                    if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                    return x.LessonPlanData != null ? x.LessonPlanData.StartDate : x.Created;
                }).ToList();
                res.AddRange(CreateGroupOfItems(person, schoolName, schoolYear, nowSchoolTime, selectedAnns, c, dayTypes, staffs, apps, appsImages));
            }

            var adminAnns = anns.Where(x => x.AdminAnnouncementData != null).ToList();
            if (adminAnns.Count > 0)
            {
                adminAnns = adminAnns.OrderBy(x => x.AdminAnnouncementData.Expires).ToList();
                res.AddRange(CreateGroupOfItems(person, schoolName, schoolYear, nowSchoolTime, adminAnns, null, dayTypes, staffs, apps, appsImages));
            }
            return res;
        }

        private static IList<FeedDetailsExportModel> CreateGroupOfItems(Person person, string schoolName, string sy, DateTime nowSchoolTime,
            IList<AnnouncementDetails> anns, ClassDetails classDetails, IList<DayType> dayTypes, IList<Staff> staffs, IList<Application> apps, IDictionary<Guid, byte[]> appsImages)
        {
            var items = (from a in anns
                         from sa in a.AnnouncementStandards.DefaultIfEmpty()
                         from aa in a.AnnouncementAttributes.DefaultIfEmpty()
                         from attItem in PrepareAttachmentItems(a.AnnouncementAttachments, a.AnnouncementApplications, apps, appsImages).DefaultIfEmpty()
                         select Create(person, schoolName, sy, nowSchoolTime, a, classDetails, dayTypes, staffs, sa?.Standard, aa, attItem)).ToList();
            return items;
        }

        private static FeedDetailsExportModel Create(Person person, string schoolName, string sy, DateTime nowSchoolTime, AnnouncementDetails ann, ClassDetails classDetails, IList<DayType> dayTypes 
            , IList<Staff> staffs, Standard standard, AnnouncementAssignedAttribute attribute, AttachmentItem attachmentItem)
        {
            var res = new FeedDetailsExportModel(person, schoolName, sy, nowSchoolTime, ann, classDetails, dayTypes, staffs, standard, attribute);
            if (attachmentItem != null)
            {
                res.AnnouncementAttachmentId = attachmentItem.Id;
                res.AnnouncementAttachmentName = attachmentItem.Name;
                res.Document = attachmentItem.Document;
                res.AnnouncementAttachmentImage = attachmentItem.Image;
                res.AnnouncementAttachmentOrder = attachmentItem.Order;
            }
            return res;
        }

        private static IList<AttachmentItem> PrepareAttachmentItems(IList<AnnouncementAttachment> annAtts, IList<AnnouncementApplication> annApps
            , IList<Application> apps, IDictionary<Guid, byte[]> appsImages)
        {

            var res = new List<AttachmentItem>();
            if (annAtts != null && annAtts.Count > 0)
                res.AddRange(annAtts.Select(x => new AttachmentItem
                {
                    Id = x.Id,
                    Name = x.Attachment.Name,
                    Order = x.Order,
                    Document = true
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
            res = res.OrderBy(x => x.Order).ToList();
            
            //reset order
            for (int i = 0; i < res.Count; i++)
                res[i].Order = i;
            return res;
        }
    }
}
