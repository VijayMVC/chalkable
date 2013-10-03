using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.FinalGradesViewData
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

        protected FinalGradeViewData(FinalGradeDetails finalGrade)
        {
            Id = finalGrade.Id;
            Class = ClassViewData.Create(finalGrade.Class);
            Attendance = finalGrade.Attendance;
            Discipline = finalGrade.Discipline;
            DropLowestAttendance = finalGrade.DropLowestAttendance;
            DropLowestDiscipline = finalGrade.DropLowestDiscipline;
            GradingStyle = (int) finalGrade.GradingStyle;
            Participation = finalGrade.ParticipationPercent;
            GradedStudentCount = finalGrade.StudentFinalGrades.Count(x => x.TeacherGrade.HasValue);
            FinalGradeAnnType = FinalGradeAnnouncementTypeViewData.Create(finalGrade.FinalGradeAnnouncementTypes);
        }
        public static FinalGradeViewData Create(FinalGradeDetails finalGrade)
        {
            return new FinalGradeViewData(finalGrade);
        }
    }
}
