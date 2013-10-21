using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.FinalGradesViewData
{
    public class StudentFinalGradeDetailsViewData : StudentFinalGradeViewData
    {
        public IList<GradingStatsByDateViewData> AvgPerDate { get; set; }
        public IList<FinalGradeAnnTypeGradeStatsViewData> AnnTypesGradeStats { get; set; }

        protected StudentFinalGradeDetailsViewData(StudentFinalGradeDetails studentFinalGrade, IGradingStyleMapper gradingMapper)
            : base(studentFinalGrade, gradingMapper)
        {
        }

        public static IList<StudentFinalGradeDetailsViewData> Create(IList<StudentFinalGradeDetails> studentsFinalGrade
            , IList<StudentClassGradeStats> studentsStats, IList<FinalGradeAnnouncementType> fgAnnTypes, IGradingStyleMapper gradingMapper)
        {
            var res = new List<StudentFinalGradeDetailsViewData>();
            foreach (var studentFg in studentsFinalGrade)
            {
                var stStats = studentsStats.FirstOrDefault(x => x.StudentId == studentFg.Student.Id);
                var stFgView = new StudentFinalGradeDetailsViewData(studentFg, gradingMapper);
                if (stStats != null)
                {
                    stFgView.AvgPerDate = GradingStatsByDateViewData.Create(stStats.GradeAvgPerDates);
                    if (stStats.AnnTypesGradeStats != null)
                    {
                        stFgView.AnnTypesGradeStats = FinalGradeAnnTypeGradeStatsViewData.Create(stStats.AnnTypesGradeStats, fgAnnTypes);
                    }
                }
            }
            return res;
        }
    }
}