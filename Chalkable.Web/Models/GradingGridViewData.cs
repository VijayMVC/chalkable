﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{

    public class GradingGridSummaryViewData
    {
        public GradingPeriodViewData GradingPeriod { get; set; }
        public int? Avg { get; set; }

        protected GradingGridSummaryViewData(GradingPeriod gradingPeriod, int? avg)
        {
            Avg = avg;
            GradingPeriod = GradingPeriodViewData.Create(gradingPeriod);
        }
        public static GradingGridSummaryViewData Create(GradingPeriod gradingPeriod, int? avg)
        {
            return new GradingGridSummaryViewData(gradingPeriod, avg);
        }
    }

    public class GradingGridsViewData
    {
        public GradingGridViewData CurrentGradingGrid { get; set; }
        public IList<GradingPeriodViewData> GradingPeriods { get; set; }
        public IList<AnnouncementStandardViewData> Standards { get; set; }
        public IList<ClassAnnouncementTypeViewData> ClassAnnouncementTypes { get; set; }

        public static GradingGridsViewData Create(ChalkableGradeBook grid, IList<GradingPeriodDetails> gradingPeriods
            , IList<Standard> standards, IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            return new GradingGridsViewData
                {
                    CurrentGradingGrid = GradingGridViewData.Create(grid),
                    GradingPeriods = gradingPeriods.Select(GradingPeriodViewData.Create).ToList(),
                    Standards = AnnouncementStandardViewData.Create(standards),
                    ClassAnnouncementTypes = ClassAnnouncementTypeViewData.Create(classAnnouncementTypes)
                };
        }
    }

    public class GradingGridViewData : GradingGridSummaryViewData
    {
        protected GradingGridViewData(ChalkableGradeBook gradeBook)
            : base(gradeBook.GradingPeriod, gradeBook.Avg)
        {
        }

        public IList<GradeStudentViewData> Students { get; set; }
        public IList<ShortAnnouncementGradeViewData> GradingItems { get; set; }
        
        public static GradingGridViewData Create(ChalkableGradeBook gradeBook)
        {
            var res = new GradingGridViewData(gradeBook)
                {
                    Students = new List<GradeStudentViewData>()
                };
            foreach (var student in gradeBook.Students)
            {
                var ann = gradeBook.Announcements.FirstOrDefault();
                bool isWithdrawn = ann != null && ann.StudentAnnouncements.FirstOrDefault() != null
                                   && ann.StudentAnnouncements.First().Withdrawn;
                res.Students.Add(GradeStudentViewData.Create(student, isWithdrawn));
            }
            var stIds = res.Students.Select(x => x.StudentInfo.Id).ToList();
            res.GradingItems = gradeBook.Announcements
                                        .Select(x => ShortAnnouncementGradeViewData.Create(x, x.StudentAnnouncements, stIds))
                                        .ToList();

            return res;
        }
        public static IList<GradingGridViewData> Create(IList<ChalkableGradeBook> gradeBooks)
        {
            return gradeBooks.Select(Create).ToList();
        }
    }

    public class GradeStudentViewData
    {
        public bool? IsWithDrawn { get; set; }
        public ShortPersonViewData StudentInfo { get; set; }

        public static GradeStudentViewData Create(Person person, bool? isWithDrawn)
        {
            return new GradeStudentViewData {StudentInfo = ShortPersonViewData.Create(person), IsWithDrawn = isWithDrawn};
        }
    }

    public class ShortStudentsAnnouncementsViewData
    {
        public IList<ShortStudentAnnouncementViewData> Items { get; set; }
        public static ShortStudentsAnnouncementsViewData Create(IList<StudentAnnouncementDetails> studentAnnouncements, IList<int> studentIds)
        {
            var res = new ShortStudentsAnnouncementsViewData  { Items = new List<ShortStudentAnnouncementViewData>() };
            foreach (var studentId in studentIds)
            {
                var stAnn = studentAnnouncements.FirstOrDefault(x => x.StudentId == studentId);
                if(stAnn != null)
                    res.Items.Add(ShortStudentAnnouncementViewData.Create(stAnn));
            }
            return res;
        }
    }

    public class ShortAnnouncementGradeViewData : AnnouncementShortViewData
    {
        public ShortStudentsAnnouncementsViewData StudentAnnouncements { get; set; }

        protected ShortAnnouncementGradeViewData(AnnouncementComplex announcement) : base(announcement)
        {
        }

        public static ShortAnnouncementGradeViewData Create(AnnouncementComplex announcement, 
            IList<StudentAnnouncementDetails> studentAnnouncements, IList<int> studentIds)
        {
            return new ShortAnnouncementGradeViewData(announcement)
                {
                    StudentAnnouncements = ShortStudentsAnnouncementsViewData.Create(studentAnnouncements, studentIds)
                };
        }
    }
}