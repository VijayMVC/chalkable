using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.Web.Logic;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class StudentAnnouncementsViewData
    {
        public IList<StudentAnnouncementViewData> Items { get; set; }
        public int GradingStyle { get; set; }
        public string AnnouncmentTitle { get; set; }
        public int? AnnouncementTypeId { get; set; }
        public int? ChalkableAnnouncementTypeId { get; set; }
        public decimal? ClassAvg { get; set; }
        public string ClassName { get; set; }
        public Guid? CourseId { get; set; }
        public int GradedStudentCount { get; set; }
        public GradingStyleViewData GradingStyleMapper { get; set; }

        private StudentAnnouncementsViewData() { }

        public static StudentAnnouncementsViewData Create(IList<StudentAnnouncementDetails> items, IList<AnnouncementAttachmentInfo> attachments,
            GradingStyleEnum gradingStyle = GradingStyleEnum.Numeric100)
        {
            var res = new StudentAnnouncementsViewData {GradingStyle = (int) gradingStyle};
            //CalculateClassAvg(res, items);
            res.Items = StudentAnnouncementViewData.Create(items, attachments);
            return res;
        }

        public static StudentAnnouncementsViewData Create(AnnouncementComplex anouncement, IList<StudentAnnouncementDetails> items,
            IList<AnnouncementAttachmentInfo> attachments, GradingStyleEnum gradingStyle = GradingStyleEnum.Numeric100)
        {
            var res = Create(items, attachments, gradingStyle);
            res.ClassName = anouncement.ClassName;
            //res.CourseId = anouncement.CourseId;
            //res.AnnouncmentTitle = anouncement.Title;
            res.AnnouncementTypeId = anouncement.ClassAnnouncementTypeRef;
            res.ChalkableAnnouncementTypeId = anouncement.ChalkableAnnouncementType;
            return res;
        }
        private static void CalculateClassAvg(StudentAnnouncementsViewData res, IEnumerable<StudentAnnouncementDetails> items)
        {
            int count = 0;
            decimal? classAvg = 0;
            foreach (var studentAnnouncement in items)
            {
                if (IncludeToAvg(studentAnnouncement))
                {
                    classAvg += studentAnnouncement.NumericScore.Value;
                    count++;
                }
            }
            classAvg = count != 0 ? classAvg / count : null;
            res.ClassAvg = classAvg;
            res.GradedStudentCount = count;
        }

        private static bool IncludeToAvg(StudentAnnouncementDetails item)
        {
            return item.NumericScore.HasValue && !item.Incomplete && !item.Exempt && !item.Dropped && !item.Absent
                && item.State == StudentAnnouncementStateEnum.Manual;
        }
    }

    public class ShortStudentAnnouncementViewData
    {
        public int Id { get; set; }
        public string GradeValue { get; set; }
        public decimal? NumericGradeValue { get; set; }
        public bool Dropped { get; set; }
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
            Dropped = studentAnnouncement.Dropped;
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
        public ShortPersonViewData StudentInfo { get; set; }
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
                var ids = attachments != null ? attachments.Where(x => x.Attachment.PersonRef == spId).ToList() : null;
                res.Add(Create(studentAnnouncementInfo, ids));
            }
            return res;
        }
        public static StudentAnnouncementViewData Create(StudentAnnouncementDetails studentAnnouncement, IList<AnnouncementAttachmentInfo> attachments)
        {
            return new StudentAnnouncementViewData(studentAnnouncement)
            {
                StudentInfo = ShortPersonViewData.Create(studentAnnouncement.Student),
                Attachments = attachments.Select(x => AnnouncementAttachmentViewData.Create(x, true)).ToList(),
            };
        }
    }
}