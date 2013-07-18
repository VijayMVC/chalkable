using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class AnnouncementViewData
    {
        private const int SHORT_LENGHT = 60;

        public DateTime Created { get; set; }

        public string AnnouncementTypeName { get; set; }
        public int AnnouncementTypeId { get; set; }
        public DateTime? ExpiresDate { get; set; }
        public bool IsOwner { get; set; }
        public bool Gradable { get; set; }
        public bool Starred { get; set; }
        public Guid Id { get; set; }
        public int Order { get; set; }
        public int State { get; set; }
        public AnnouncementState StateTyped { get; set; }
        public int QnACount { get; set; }
        public int AttachmentsCount { get; set; }
        public int OwnerAttachmentsCount { get; set; }
        public string Title { get; set; }
        public Guid? RecipientId { get; set; }
        public string Content { get; set; }
        public string ShortContent { get; set; }
        public string Subject { get; set; }
        
        public Guid PersonRef { get; set; }
        public string PersonName { get; set; }
        public string PersonGender { get; set; }
      
        public int? Grade { get; set; }
        public Guid? StudentAnnouncementId { get; set; }
        public bool? Dropped { get; set; }
        public int StudentsCount { get; set; }
        public int StudentsCountWithAttachments { get; set; }
        public int StudentsCountWithoutAttachments { get; set; }
        public int GradingStudentsCount { get; set; }
        public int NonGradingStudentsCount { get; set; }
        public string Comment { get; set; }
        public ClassViewData Class { get; set; }
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
        {
            AttachmentsCount = announcement.AttachmentsCount;
            OwnerAttachmentsCount = announcement.OwnerAttachmentsCount;
            QnACount = announcement.QnACount;
            AnnouncementTypeName = announcement.AnnouncementTypeName;
            AnnouncementTypeId = announcement.AnnouncementTypeRef;
            ExpiresDate = announcement.Expires == DateTime.MinValue ? (DateTime?)null : announcement.Expires;
            IsOwner = announcement.IsOwner;
            Gradable = isGradable;
            Starred = announcement.Starred ?? false;
            Id = announcement.Id;
            Order = announcement.Order;
            State = (int)announcement.State;
            Title = announcement.Title;
            RecipientId = announcement.ClassId;
            Content = announcement.Content;
            Subject = announcement.Subject;

            var content = announcement.Content ?? "";
            ShortContent = StringTools.BuildShortText(content, SHORT_LENGHT);

            PersonRef = announcement.PersonRef;
            PersonName = announcement.PersonName;
            PersonGender = announcement.Gender;
           
            Created = announcement.Created;
            
            Class = announcement.ClassId.HasValue ? ClassViewData.Create(announcement.ClassId.Value, announcement.ClassName) : null;
            StudentsCount = announcement.StudentsCount;
            StudentsCountWithAttachments = announcement.StudentsCountWithAttachments;
            StudentsCountWithoutAttachments = StudentsCount - StudentsCountWithAttachments;
            GradingStudentsCount = announcement.GradingsStudentsCount;
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
    }

    public class AnnouncementGradeViewData : AnnouncementViewData
    {
        protected AnnouncementGradeViewData(AnnouncementComplex announcement, IList<StudentAnnouncement> studentAnnouncements, IGradingStyleMapper mapper, bool isGradable = false, bool? wasAnnouncementTypeGraded = null)
            : base(announcement, wasAnnouncementTypeGraded, isGradable)
        {
            PrepareGradingInfo(this, announcement, studentAnnouncements, mapper);
        }

        protected AnnouncementGradeViewData(AnnouncementComplex announcement, bool isGradable, bool? wasAnnouncementTypeGraded = null)
            : base(announcement, wasAnnouncementTypeGraded, isGradable)
        {
        }

        public static AnnouncementGradeViewData Create(AnnouncementComplex announcement, IList<StudentAnnouncement> studentAnnouncements, IGradingStyleMapper mapper, bool isGradable = false, bool? wasAnnouncementTypeGraded = null)
        {
            var res = new AnnouncementGradeViewData(announcement, studentAnnouncements, mapper, isGradable, wasAnnouncementTypeGraded);
            return res;
        }

        private static void PrepareGradingInfo(AnnouncementViewData res, AnnouncementComplex announcement, IList<StudentAnnouncement> studentAnnouncements, IGradingStyleMapper mapper)
        {
            res.Avg = announcement.Avg;
            res.AvgNumeric = announcement.Avg;
            res.GradingStyle = (int)announcement.GradingStyle;
            if (studentAnnouncements != null && studentAnnouncements.Count > 0)
            {
                if (studentAnnouncements.Count == 1)
                {
                    var studentAnnouncement = studentAnnouncements.First();
                    res.Grade = studentAnnouncement.GradeValue;
                    res.StudentAnnouncementId = studentAnnouncement.Id;
                    res.Comment = studentAnnouncement.Comment;
                }
                int graded = 0;
                int? summ = null;
                int cnt = 0;
                foreach (var gradeItem in studentAnnouncements)
                {
                    if (gradeItem.GradeValue.HasValue)
                    {
                        graded++;
                        summ = summ.HasValue ? summ + gradeItem.GradeValue.Value : gradeItem.GradeValue.Value;
                        cnt++;
                    }
                }
                var count = studentAnnouncements.Count;
                res.GradeSummary = graded + "/" + count;
                res.AttachmentSummary = res.StudentsCountWithAttachments + "/" + count;
                res.Avg = summ.HasValue ? (int)Math.Round(1.0 * summ.Value / cnt) : (int?)null;
                res.AvgNumeric = summ.HasValue ? Math.Round(1.0 * summ.Value / cnt) : (double?)null;
            }
        }
    }

    public class AnnouncementDetailedViewData : AnnouncementGradeViewData
    {
        public IList<AnnouncementAttachmentViewData> AnnouncementAttachments { get; set; }
        public IList<AnnouncementQnAViewData> AnnouncementQnAs { get; set; }
        public IList<AnnouncementReminderViewData> AnnouncementReminders { get; set; }
        //public IList<AnnouncementApplicationViewData> Applications { get; set; }
        public StudentAnnouncementsViewData StudentAnnouncements { get; set; }
        public IList<String> autoGradeApps { get; set; }

        public ShortPersonViewData Owner { get; set; }
        public bool WasSubmittedToAdmin { get; set; }

        private AnnouncementDetailedViewData(AnnouncementDetails announcementDetails, IList<StudentAnnouncement> studentAnnouncements, IGradingStyleMapper mapper, Guid currentSchoolPersonId)
            : base(announcementDetails, studentAnnouncements, mapper, announcementDetails.IsGradable, null)
        {
            if (announcementDetails.AnnouncementQnAs != null)
                AnnouncementQnAs = AnnouncementQnAViewData.Create(announcementDetails.AnnouncementQnAs);

            AnnouncementReminders = AnnouncementReminderViewData.Create(announcementDetails.AnnouncementReminders, currentSchoolPersonId, Owner.Id);
            Owner = ShortPersonViewData.Create(announcementDetails.Owner);
            WasSubmittedToAdmin = announcementDetails.WasSubmittedToAdmin;

            if (announcementDetails.AnnouncementApplications == null) return;
            //TODO: applicationViewData
            //Applications = new List<AnnouncementApplicationViewData>();
            //foreach (var announcementApplication in announcementDetails.AnnouncementApplications)
            //{
            //    Applications.Add(AnnouncementApplicationViewData.Create(announcementApplication, currentSchoolPersonId));
            //}
        }

        private AnnouncementDetailedViewData(AnnouncementComplex announcement, bool isGradable, bool? wasAnnouncementTypeGraded)
            : base(announcement, isGradable, wasAnnouncementTypeGraded)
        {
        }

        public static AnnouncementDetailedViewData CreateEmpty(AnnouncementComplex announcement, bool isGradable = false, bool? wasAnnouncementTypeGraded = null)
        {
            return new AnnouncementDetailedViewData(announcement, isGradable, wasAnnouncementTypeGraded);
        }

        public static AnnouncementDetailedViewData Create(AnnouncementDetails announcementDetails, IGradingStyleMapper mapper, Guid currentSchoolPersonId)
        {
            var studentAnnouncements = announcementDetails.StudentAnnouncements.Select(x => new StudentAnnouncement
            {
                Id = x.Id,
                AnnouncementRef = x.AnnouncementRef,
                ClassPersonRef = x.ClassPersonRef,
                Comment = x.Comment,
                Dropped = x.Dropped,
                ExtraCredit = x.ExtraCredit,
                GradeValue = x.GradeValue
            }).ToList();
            return new AnnouncementDetailedViewData(announcementDetails, studentAnnouncements, mapper, currentSchoolPersonId);
        }

        public static AnnouncementDetailedViewData Create(AnnouncementDetails announcementDetails, IGradingStyleMapper mapper,
            Guid currentSchoolPersonId, IList<AnnouncementAttachmentInfo> attachmentInfos)
        {
            var res = Create(announcementDetails, mapper, currentSchoolPersonId);
            res.AnnouncementAttachments = AnnouncementAttachmentViewData.Create(attachmentInfos, currentSchoolPersonId);
            return res;
        }
    }
}