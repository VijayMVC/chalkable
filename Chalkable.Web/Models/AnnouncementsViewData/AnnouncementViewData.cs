using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Data.School.Model.Announcements.Sis;
using Chalkable.Data.School.Model.Chlk;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementViewData : ShortAnnouncementViewData
    {
        public ShortAnnouncementViewData LessonPlanData { get; set; }
        public ShortAnnouncementViewData AdminAnnouncementData { get; set; }
        public ShortAnnouncementViewData ClassAnnouncementData { get; set; }

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
        
        //Application
        public int ApplicationsCount { get; set; }
        public string ApplicationName { get; set; }
        public bool ShowGradingIcon { get; set; }
        public Guid? AssessmentApplicationId { get; set; }

        protected AnnouncementViewData(AnnouncementComplex announcement)
            : base(announcement)
        {
            ShortAnnouncementViewData annData = null;
            if (announcement.LessonPlanData != null)
            {
                LessonPlanData = LessonPlanViewData.Create(announcement.LessonPlanData);
                annData = LessonPlanData;
                ClassId = announcement.LessonPlanData.ClassRef;
                ClassName = announcement.LessonPlanData.ClassName;
            }
            if (announcement.AdminAnnouncementData != null)
            {
                AdminAnnouncementData = AdminAnnouncementViewData.Create(announcement.AdminAnnouncementData);
                annData = AdminAnnouncementData;
            }
            if (announcement.ClassAnnouncementData != null)
            {
                ClassAnnouncementData = ClassAnnouncementViewData.Create(announcement.ClassAnnouncementData);
                annData = ClassAnnouncementData;
                ClassId = announcement.ClassAnnouncementData.ClassRef;
                ClassName = announcement.ClassAnnouncementData.ClassName;
            }
            if (annData != null)
            {
                PersonId = annData.PersonId;
                PersonName = annData.PersonName;
                PersonGender = annData.PersonGender;   
            }
            AttachmentsCount = announcement.AttachmentsCount;
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