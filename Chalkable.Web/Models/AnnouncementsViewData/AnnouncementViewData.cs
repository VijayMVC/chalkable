using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementViewData : AnnouncementShortViewData
    {
        private const int SHORT_LENGHT = 60;

        public DateTime Created { get; set; }

        public bool Gradable { get; set; }
        public bool Starred { get; set; }
        public int State { get; set; }
        public AnnouncementState StateTyped { get; set; }
        public int QnACount { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public int? RecipientId { get; set; }
        public string Content { get; set; }
        public string ShortContent { get; set; }
        
        public int? Grade { get; set; }
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

        protected AnnouncementViewData(AnnouncementComplex announcement, bool? wasAnnouncementTypeGraded, bool isGradable)
            : base(announcement)
        {
            AttachmentsCount = announcement.AttachmentsCount;
            OwnerAttachmentsCount = announcement.OwnerAttachmentsCount;
            QnACount = announcement.QnACount;
            ExpiresDate = announcement.Expires == DateTime.MinValue ? (DateTime?)null : announcement.Expires;
            IsOwner = announcement.IsOwner;
            Gradable = isGradable;
            Starred = announcement.Starred ?? false;
            Id = announcement.Id;
            Order = announcement.Order;
            State = (int)announcement.State;
            Title = announcement.Title;
            RecipientId = announcement.ClassRef;
            Content = announcement.Content;

            var content = announcement.Content ?? "";
            ShortContent = StringTools.BuildShortText(content, SHORT_LENGHT);

            Created = announcement.Created;  
            StudentsCount = announcement.StudentsCount;
            StudentsCountWithAttachments = announcement.StudentsCountWithAttachments;
            StudentsCountWithoutAttachments = StudentsCount - StudentsCountWithAttachments;
            GradingStudentsCount = announcement.GradingStudentsCount;
            NonGradingStudentsCount = StudentsCount - GradingStudentsCount;
            ApplicationsCount = announcement.ApplicationCount;
            WasAnnouncementTypeGraded = wasAnnouncementTypeGraded;
            ShowGradingIcon = StudentsCount > 0 && StudentsCountWithAttachments * 4 > StudentsCount || GradingStudentsCount > 0;
            //ApplicationName = announcement.ApplicationName;
            Dropped = announcement.Dropped;
        }


        public static AnnouncementViewData Create(AnnouncementComplex announcement, bool? wasAnnouncementTypeGraded = null, bool isGradable = false)
        {
            var res = new AnnouncementViewData(announcement, wasAnnouncementTypeGraded, isGradable);
            return res;
        }

        public new static IList<AnnouncementViewData> Create(IList<AnnouncementComplex> announcements)
        {
            return announcements.Select(x => Create(x)).ToList();
        }
    }
}