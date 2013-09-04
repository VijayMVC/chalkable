using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.FinalGradesViewData
{
    public class StudentFinalGradeViewData : ShortPersonViewData
    {
        public int? GradeByAnnouncement { get; set; }
        public int? GradeByAttendance { get; set; }
        public int? GradeByDiscipline { get; set; }
        public int? GradeByParticipation { get; set; }
        public string Comment { get; set; }
        public int? Suggested { get; set; }
        public int? AdjustedTeacher { get; set; }
        public int? AdjustedAdmin { get; set; }

        protected StudentFinalGradeViewData(StudentFinalGradeDetails studentFinalGrade, IGradingStyleMapper gradingMapper)
            : base(studentFinalGrade.Student)
        {
            GradeByAnnouncement = gradingMapper.Map(studentFinalGrade.FinalGrade.GradingStyle, studentFinalGrade.GradeByAnnouncement);
            GradeByAttendance = studentFinalGrade.GradeByAttendance;
            GradeByDiscipline = studentFinalGrade.GradeByDiscipline;
            GradeByParticipation = studentFinalGrade.GradeByParticipation;
            Comment = studentFinalGrade.Comment;
            int? suggested = studentFinalGrade.GradeByAnnouncement + GradeByAttendance + GradeByDiscipline
                + (int?)(GradeByParticipation * studentFinalGrade.FinalGrade.ParticipationPercent / 100.0);
            Suggested = gradingMapper.Map(studentFinalGrade.FinalGrade.GradingStyle, suggested);
            AdjustedTeacher = studentFinalGrade.TeacherGrade - suggested;
            AdjustedAdmin = studentFinalGrade.AdminGrade - suggested;
            Comment = studentFinalGrade.Comment;
        }
    }
}