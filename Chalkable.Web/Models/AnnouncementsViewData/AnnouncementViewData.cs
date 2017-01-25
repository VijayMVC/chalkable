using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class 
        AnnouncementViewData : ShortAnnouncementViewData
    {
        public ShortAnnouncementViewData LessonPlanData { get; set; }
        public ShortAnnouncementViewData AdminAnnouncementData { get; set; }
        public ShortAnnouncementViewData ClassAnnouncementData { get; set; }
        public ShortAnnouncementViewData SupplementalAnnouncementData { get; set; }

        public int? ClassId { get; set; }
        public string ClassName { get; set; }

        //Announcement summary data
        private const int SHORT_LENGHT = 60;
        public string ShortContent { get; set; }
        public bool Complete { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public bool CanAddStandard { get; set; }
        public int? StudentAnnouncementId { get; set; }
        public int StudentsCountWithAttachments { get; set; }
        public int StudentsCountWithoutAttachments { get; set; }
        public int GradingStudentsCount { get; set; }
        public IList<string> AttachmentNames { get; set; }

        //Application
        public int ApplicationsCount { get; set; }
        public string ApplicationName { get; set; }
        public bool ShowGradingIcon { get; set; }
        public Guid? AssessmentApplicationId { get; set; }

        public decimal? Grade { get; set; }
        public string Comment { get; set; }

        protected AnnouncementViewData(Announcement announcementData, IList<ClaimInfo> claims) :base(announcementData)
        {
            ShortAnnouncementViewData annData = null;
            var lp = announcementData as LessonPlan;
            if (lp != null)
            {
                LessonPlanData = LessonPlanViewData.Create(lp);
                annData = LessonPlanData;
                ClassId = lp.ClassRef;
                ClassName = lp.ClassName;
            }
            var sup = announcementData as SupplementalAnnouncement;
            if (sup != null)
            {
                SupplementalAnnouncementData = SupplementalAnnouncementViewData.Create(sup);
                annData = SupplementalAnnouncementData;
                ClassId = sup.ClassRef;
                ClassName = sup.ClassName;
            }
            
            var adminAnn = announcementData as AdminAnnouncement;
            if (adminAnn != null)
            {
                AdminAnnouncementData = AdminAnnouncementViewData.Create(adminAnn);
                annData = AdminAnnouncementData;
            }
            var classAnn = announcementData as ClassAnnouncement;
            if (classAnn != null)
            {
                ClassAnnouncementData = ClassAnnouncementViewData.Create(classAnn, claims);
                annData = ClassAnnouncementData;
                ClassId = classAnn.ClassRef;
                ClassName = classAnn.ClassName;
            }
            if (annData != null)
            {
                PersonId = annData.PersonId;
                PersonName = annData.PersonName;
                PersonGender = annData.PersonGender;
            }
        }

        protected AnnouncementViewData(AnnouncementComplex announcement, IList<ClaimInfo> claims) :this(announcement.AnnouncementData, claims)
        {
            AttachmentsCount = announcement.AttachmentsCount;
            AttachmentNames = announcement.AttachmentNames;
            OwnerAttachmentsCount = announcement.OwnerAttachmentsCount;
            Complete = announcement.Complete;

            var content = announcement.Content ?? "";
            ShortContent = StringTools.BuildShortText(content, SHORT_LENGHT);

            var studentsCounts = announcement.StudentsCount;
            StudentsCountWithAttachments = announcement.StudentsCountWithAttachments;
            StudentsCountWithoutAttachments = studentsCounts - StudentsCountWithAttachments;
            GradingStudentsCount = announcement.GradingStudentsCount;
            ApplicationsCount = announcement.ApplicationCount;
            ShowGradingIcon = studentsCounts > 0 && StudentsCountWithAttachments * 4 > studentsCounts || GradingStudentsCount > 0;
        }

        protected AnnouncementViewData(AnnouncementComplex announcement, IList<StudentAnnouncement> studentAnnouncements, IList<ClaimInfo> claims)
                : this(announcement, claims)
        {
            PrepareGradingInfo(this, studentAnnouncements);
        }


        public static AnnouncementViewData Create(Announcement announcement, IList<ClaimInfo> claims)
        {
            return new AnnouncementViewData(announcement, claims);
        }

        public static IList<AnnouncementViewData> Create(IEnumerable<AnnouncementComplex> announcements, IList<ClaimInfo> claims)
        {
            return announcements.Select(x => Create(x, claims)).ToList();
        }

        public static IList<AnnouncementViewData> Create(IEnumerable<AnnouncementComplex> announcements, IEnumerable<StudentAnnouncement> studentAnnouncements)
        {
            return Create(announcements, new List<AnnouncementApplication>(), new List<Application>(), new List<ClaimInfo>(), studentAnnouncements);
        }

        public static IList<AnnouncementViewData> Create(IEnumerable<AnnouncementComplex> announcements
            , IList<AnnouncementApplication> annApps, IList<Application> applications
            , IList<ClaimInfo> claims, IEnumerable<StudentAnnouncement> studentAnnouncements)
        {
            var res = new List<AnnouncementViewData>();
            var stAnns = studentAnnouncements.ToList();
            foreach (var ann in announcements)
            {
                var app = applications.FirstOrDefault(a=> annApps.Any(annApp=>annApp.ApplicationRef == a.Id && annApp.AnnouncementRef == ann.Id));
                var appName = app?.Name;
                var annView = new AnnouncementViewData(ann,
                    stAnns.Where(x => x.AnnouncementId == ann.Id).ToList(), claims)
                {
                    ApplicationName = appName
                };
                if (string.IsNullOrEmpty(appName))
                    annView.ApplicationsCount = 0;
                res.Add(annView);
            }
            return res;
        }

        private static void PrepareGradingInfo(AnnouncementViewData res, IList<StudentAnnouncement> studentAnnouncements)
        {
            if (studentAnnouncements != null && studentAnnouncements.Count > 0)
            {
                if (studentAnnouncements.Count == 1)
                {
                    var studentAnnouncement = studentAnnouncements.First();
                    res.Grade = studentAnnouncement.NumericScore;
                    res.Comment = studentAnnouncement.Comment;
                }
            }
        }
    }
}