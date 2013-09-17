using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.FinalGradesViewData
{
    public class FinalGradeDetailsViewData : FinalGradeViewData
    {
        IList<StudentFinalGradeDetailsViewData> StudentFinalGradeDetails { get; set; }

        protected FinalGradeDetailsViewData(FinalGradeDetails finalGrade): base(finalGrade)
        {
        }

        public static FinalGradeDetailsViewData Create(FinalGradeDetails finalGrade, IList<StudentClassGradeStats> studentsStats
            , IGradingStyleMapper gradingMapper)
        {
            return new FinalGradeDetailsViewData(finalGrade)
            {
                StudentFinalGradeDetails = StudentFinalGradeDetailsViewData.Create(finalGrade.StudentFinalGrades,
                                            studentsStats, finalGrade.FinalGradeAnnouncementTypes, gradingMapper)
            };
        }
    }
}