using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementViewData : AnnouncementShortViewData
    {
        private const int SHORT_LENGHT = 60;

        public DateTime Created { get; set; }

        public bool Complete { get; set; }
        public int State { get; set; }
        public AnnouncementState StateTyped { get; set; }
        public int QnACount { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public int? RecipientId { get; set; }
        public string Content { get; set; }
        public string ShortContent { get; set; }
        public decimal? WeightMultiplier { get; set; }
        public decimal? WeightAddition { get; set; }
        public bool HideFromStudents { get; set; }
        
        public bool CanAddStandard { get; set; }

        public decimal? Grade { get; set; }
        public int? StudentAnnouncementId { get; set; }
        public int StudentsCount { get; set; }
        public int StudentsCountWithAttachments { get; set; }
        public int StudentsCountWithoutAttachments { get; set; }
        public int GradingStudentsCount { get; set; }
        public int NonGradingStudentsCount { get; set; }
        public string Comment { get; set; }
        //Grading
        public string GradeSummary { get; set; }
        public string AttachmentSummary { get; set; }
        public int? Avg { get; set; }
        public double? AvgNumeric { get; set; }
        public int GradingStyle { get; set; }
        //Application
        public int ApplicationsCount { get; set; }
        public string ApplicationName { get; set; }

        public bool? WasAnnouncementTypeGraded { get; set; }
        public bool ShowGradingIcon { get; set; }

        protected AnnouncementViewData(AnnouncementComplex announcement, bool? wasAnnouncementTypeGraded)
            : base(announcement)
        {
            AttachmentsCount = announcement.AttachmentsCount;
            OwnerAttachmentsCount = announcement.OwnerAttachmentsCount;
            QnACount = announcement.QnACount;
            Complete = announcement.Complete;
            State = (int)announcement.State;
            RecipientId = announcement.ClassRef;
            Content = announcement.Content;

            var content = announcement.Content ?? "";
            ShortContent = StringTools.BuildShortText(content, SHORT_LENGHT);

            HideFromStudents = !announcement.VisibleForStudent;
            WeightAddition = announcement.WeightAddition;
            WeightMultiplier = announcement.WeightMultiplier;
            Created = announcement.Created;  
            StudentsCount = announcement.StudentsCount;
            StudentsCountWithAttachments = announcement.StudentsCountWithAttachments;
            StudentsCountWithoutAttachments = StudentsCount - StudentsCountWithAttachments;
            GradingStudentsCount = announcement.GradingStudentsCount;
            NonGradingStudentsCount = StudentsCount - GradingStudentsCount;
            ApplicationsCount = announcement.ApplicationCount;
            WasAnnouncementTypeGraded = wasAnnouncementTypeGraded;
            ShowGradingIcon = StudentsCount > 0 && StudentsCountWithAttachments * 4 > StudentsCount || GradingStudentsCount > 0;
        }


        public static AnnouncementViewData Create(AnnouncementComplex announcement, bool? wasAnnouncementTypeGraded = null, string applicationName = null)
        {
            var res = new AnnouncementViewData(announcement, wasAnnouncementTypeGraded);
            if (!string.IsNullOrEmpty(applicationName))
                res.ApplicationName = applicationName;
            return res;
        }

        public new static IList<AnnouncementViewData> Create(IList<AnnouncementComplex> announcements)
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
                var annView = Create(ann, null, appName);
                if (string.IsNullOrEmpty(appName))
                    annView.ApplicationsCount = 0;
                res.Add(annView);
            }
            return res;
        } 
    }
}