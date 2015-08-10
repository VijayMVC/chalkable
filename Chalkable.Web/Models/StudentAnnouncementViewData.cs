using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class StudentAnnouncementsViewData
    {
        public IList<StudentAnnouncementViewData> Items { get; set; }
        public string AnnouncmentTitle { get; set; }
        public int? AnnouncementTypeId { get; set; }
        public int? ChalkableAnnouncementTypeId { get; set; }
        public decimal? ClassAvg { get; set; }
        public string ClassName { get; set; }
        public Guid? CourseId { get; set; }
        public int GradedStudentCount { get; set; }

        private StudentAnnouncementsViewData() { }

        public static StudentAnnouncementsViewData Create(IList<StudentAnnouncementDetails> items, IList<AnnouncementAttachmentInfo> attachments)
        {
            return new StudentAnnouncementsViewData
                {
                    Items = StudentAnnouncementViewData.Create(items, attachments)
                };
            //CalculateClassAvg(res, items);
        }

        public static StudentAnnouncementsViewData Create(ClassAnnouncement anouncement, IList<StudentAnnouncementDetails> items,
            IList<AnnouncementAttachmentInfo> attachments)
        {
            var res = Create(items, attachments);
            res.ClassName = anouncement.ClassName;
            res.AnnouncementTypeId = anouncement.ClassAnnouncementTypeRef;
            res.ChalkableAnnouncementTypeId = anouncement.ChalkableAnnouncementType;
            return res;
        }
    }

    public class ShortStudentAnnouncementViewData
    {
        public int Id { get; set; }
        public string GradeValue { get; set; }
        public decimal? NumericGradeValue { get; set; }

        public bool AverageDropped { get; set; }
        public bool CategoryDropped { get; set; }
        public bool AnnouncementDropped { get; set; }
        public bool Dropped { get; set; }
        public bool AutomaticalyDropped { get; set; }
        
        public bool IsExempt { get; set; }
        public bool IsIncomplete { get; set; }
        public bool IsLate { get; set; }
        public bool IsAbsent { get; set; }
        public bool IsUnexcusedAbsent { get; set; }
        public int AnnouncementId { get; set; }
        public string Comment { get; set; }
        public string ExtraCredits { get; set; }
        public int State { get; set; }
        public int StudentId { get; set; }
        public bool IncludeInAverage { get; set; }
        
        protected ShortStudentAnnouncementViewData(StudentAnnouncement studentAnnouncement)
        {
            IsAbsent = studentAnnouncement.Absent;
            IsUnexcusedAbsent = studentAnnouncement.IsUnexcusedAbsent;
            IsExempt = studentAnnouncement.Exempt;
            IsIncomplete = studentAnnouncement.Incomplete;
            IsLate = studentAnnouncement.Late;
            AnnouncementId = studentAnnouncement.AnnouncementId;
            Comment = studentAnnouncement.Comment;
            AverageDropped = studentAnnouncement.AverageDropped;
            CategoryDropped = studentAnnouncement.CategoryDropped;
            AnnouncementDropped = studentAnnouncement.AnnouncementDropped;
            Dropped = studentAnnouncement.ScoreDropped;
            AutomaticalyDropped = studentAnnouncement.AutomaticalyDropped;
            GradeValue = studentAnnouncement.ScoreValue;
            ExtraCredits = studentAnnouncement.ExtraCredit;
            NumericGradeValue = studentAnnouncement.NumericScore;
            GradeValue = studentAnnouncement.ScoreValue;
            State = (int)studentAnnouncement.State;
            StudentId = studentAnnouncement.StudentId;
            IncludeInAverage = studentAnnouncement.IncludeInAverage;
        }
        public static ShortStudentAnnouncementViewData Create(StudentAnnouncement studentAnnouncement)
        {
            return new ShortStudentAnnouncementViewData(studentAnnouncement);
        }
        
    }

    public class StudentAnnouncementViewData : ShortStudentAnnouncementViewData
    {
        public StudentViewData StudentInfo { get; set; }
        public string Raw { get; set; }
        public IList<AnnouncementAttachmentViewData> Attachments { get; set; }
        
        protected StudentAnnouncementViewData(StudentAnnouncement studentAnnouncement)
            : base(studentAnnouncement)
        {
        }

        public static IList<StudentAnnouncementViewData> Create(IList<StudentAnnouncementDetails> items, IList<AnnouncementAttachmentInfo> attachments)
        {
            var res = new List<StudentAnnouncementViewData>();
            foreach (var studentAnnouncementInfo in items)
            {
                var spId = studentAnnouncementInfo.Student.Id;
                var ids = attachments != null ? attachments.Where(x => x.AttachmentInfo.Attachment.PersonRef == spId).ToList() : null;
                res.Add(Create(studentAnnouncementInfo, ids));
            }
            return res;
        }
        public static StudentAnnouncementViewData Create(StudentAnnouncementDetails studentAnnouncement, IList<AnnouncementAttachmentInfo> attachments)
        {
            return new StudentAnnouncementViewData(studentAnnouncement)
            {
                StudentInfo = StudentViewData.Create(studentAnnouncement.Student),
                Attachments = attachments.Select(x => AnnouncementAttachmentViewData.Create(x, true)).ToList(),

            };
        }
    }
}