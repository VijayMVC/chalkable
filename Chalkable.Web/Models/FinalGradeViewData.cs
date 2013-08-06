using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Models
{
    public class FinalGradeViewData
    {
        public Guid Id { get; set; }
        public int State { get; set; }
        public ClassViewData Class { get; set; }
        public int GradedStudentCount { get; set; }
        
        public int Participation { get; set; }
        public int Discipline { get; set; }
        public bool DropLowestDiscipline { get; set; }
        public int Attendance { get; set; }
        public bool DropLowestAttendance { get; set; }
        public int GradingStyle { get; set; }

        public IList<FinalGradeAnnouncementTypeViewData> FinalGradeAnnType { get; set; }
        //public IList<StudentFinalGradeViewData> StudentFinalGrades { get; set; }


        public static FinalGradeViewData Create(FinalGradeDetails finalGrade)
        {
            var res = new FinalGradeViewData
                {
                    Id = finalGrade.Id,
                    Class = ClassViewData.Create(finalGrade.Class),
                    Attendance = finalGrade.Attendance,
                    Discipline = finalGrade.Discipline,
                    DropLowestAttendance = finalGrade.DropLowestAttendance,
                    DropLowestDiscipline = finalGrade.DropLowestDiscipline,
                    GradingStyle = (int) finalGrade.GradingStyle,
                    Participation = finalGrade.ParticipationPercent,
                    GradedStudentCount = finalGrade.StudentFinalGrades.Count(x => x.TeacherGrade.HasValue),
                    FinalGradeAnnType = FinalGradeAnnouncementTypeViewData.Create(finalGrade.FinalGradeAnnouncementTypes)
                };
            return res;
        }
    }

    public class FinalGradeAnnouncementTypeViewData
    {
        public Guid Id { get; set; }
        public string TypeName { get; set; }
        public int Value { get; set; }
        public bool DropLowest { get; set; }
        public int GradingStyle { get; set; }

        public static FinalGradeAnnouncementTypeViewData Create(FinalGradeAnnouncementType finalGradeAnnType)
        {
            return new FinalGradeAnnouncementTypeViewData
                {
                    Id = finalGradeAnnType.Id,
                    DropLowest = finalGradeAnnType.DropLowest,
                    GradingStyle = (int) finalGradeAnnType.GradingStyle,
                    TypeName = finalGradeAnnType.AnnouncementType.Name,
                    Value = finalGradeAnnType.PercentValue
                };
        }
        public static IList<FinalGradeAnnouncementTypeViewData> Create(IList<FinalGradeAnnouncementType> finalGradeAnnTypes)
        {
            return finalGradeAnnTypes.Select(Create).ToList();
        }
    }
}