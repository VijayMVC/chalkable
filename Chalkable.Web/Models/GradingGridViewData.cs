using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class GradingGridViewData
    {
        public int? Avg { get; set; }
        public IList<GradeStudentViewData> Students { get; set; }
        public IList<ShortAnnouncementGradeViewData> Announcements { get; set; }
        public MarkingPeriodViewData MarkingPeriod { get; set; }

        public static GradingGridViewData Create(ChalkableGradeBook gradeBook)
        {
            var res = new GradingGridViewData
                {
                    Avg = gradeBook.Avg,
                    MarkingPeriod = MarkingPeriodViewData.Create(gradeBook.MarkingPeriod),
                    Students = gradeBook.Students.Select(GradeStudentViewData.Create).ToList(),
                    Announcements = gradeBook.Announcements
                                             .Select(x => ShortAnnouncementGradeViewData.Create(x, x.StudentAnnouncements))
                                             .ToList(),
                };
            return res;
        }
        public static IList<GradingGridViewData> Create(IList<ChalkableGradeBook> gradeBooks)
        {
            return gradeBooks.Select(Create).ToList();
        }
    }

    public class GradeStudentViewData
    {
        public bool IsWithDrawn { get; set; }
        public ShortPersonViewData StudentInfo { get; set; }

        public static GradeStudentViewData Create(Person person)
        {
            return new GradeStudentViewData {StudentInfo = ShortPersonViewData.Create(person)};
        }
    }

    public class ShortStudentsAnnouncementsViewData
    {
        public IList<ShortStudentAnnouncementViewData> Items { get; set; }
        public static ShortStudentsAnnouncementsViewData Create(IList<StudentAnnouncementDetails> studentAnnouncements)
        {
            return new ShortStudentsAnnouncementsViewData
                {
                    Items = studentAnnouncements.Select(ShortStudentAnnouncementViewData.Create).ToList()
                };
        }
    }

    public class ShortAnnouncementGradeViewData : AnnouncementShortViewData
    {
        public ShortStudentsAnnouncementsViewData StudentAnnouncements { get; set; }

        protected ShortAnnouncementGradeViewData(AnnouncementComplex announcement) : base(announcement)
        {
        }

        public static ShortAnnouncementGradeViewData Create(AnnouncementComplex announcement, 
            IList<StudentAnnouncementDetails> studentAnnouncements)
        {
            return new ShortAnnouncementGradeViewData(announcement)
                {
                    StudentAnnouncements = ShortStudentsAnnouncementsViewData.Create(studentAnnouncements)
                };
        }
    }
}