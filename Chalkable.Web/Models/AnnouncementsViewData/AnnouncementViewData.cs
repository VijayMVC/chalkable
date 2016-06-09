using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementViewData : ShortAnnouncementViewData
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
        public string AttachmentNames { get; set; }

        //Application
        public int ApplicationsCount { get; set; }
        public string ApplicationName { get; set; }
        public bool ShowGradingIcon { get; set; }
        public Guid? AssessmentApplicationId { get; set; }


        protected AnnouncementViewData(Announcement announcementData):base(announcementData)
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
                ClassAnnouncementData = ClassAnnouncementViewData.Create(classAnn);
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

        protected AnnouncementViewData(AnnouncementComplex announcement):this(announcement.AnnouncementData)
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


        public new static AnnouncementViewData Create(Announcement announcement)
        {
            return new AnnouncementViewData(announcement);
        }

        public static AnnouncementViewData Create(AnnouncementComplex announcement, string applicationName = null)
        {
            var res = new AnnouncementViewData(announcement);
            if (!string.IsNullOrEmpty(applicationName))
                res.ApplicationName = applicationName;
            return res;
        }

        public static IList<AnnouncementViewData> Create(IList<AnnouncementComplex> announcements)
        {
            return announcements.Select(x => Create(x)).ToList();
        }

        public static IList<AnnouncementViewData> Create(IList<AnnouncementComplex> announcements
            , IList<AnnouncementApplication> annApps, IList<Application> applications)
        {
            var res = new List<AnnouncementViewData>();
            foreach (var ann in announcements)
            {
                var app = applications.FirstOrDefault(a=> annApps.Any(annApp=>annApp.ApplicationRef == a.Id && annApp.AnnouncementRef == ann.Id));
                var appName = app != null ? app.Name : null;
                var annView = Create(ann, appName);
                if (string.IsNullOrEmpty(appName))
                    annView.ApplicationsCount = 0;
                res.Add(annView);
            }
            return res;
        } 
    }
}