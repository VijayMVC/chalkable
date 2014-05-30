using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiStandardScoreStorage:BaseDemoIntStorage<StandardScore>
    {
        public DemoStiStandardScoreStorage(DemoStorage storage)
            : base(storage, null, true)
        {
        }

        public IEnumerable<StandardScore> GetStandardScores(int sectionId, int? standardId, int? gradingPeriodId)
        {
            var scores = data.Select(x => x.Value);

            scores = scores.Where(x => x.SectionId == sectionId);

            if (standardId.HasValue)
                scores = scores.Where(x => x.StandardId == sectionId);
            if (gradingPeriodId.HasValue)
                scores = scores.Where(x => x.GradingPeriodId == gradingPeriodId);

            return scores.ToList();
        }

        public StandardScore Update(int classId, int studentId, int standardId, int gradingPeriodId, StandardScore standardScore)
        {
            if (data.Any(x => x.Value.StudentId == studentId && x.Value.SectionId == classId &&
                              x.Value.StandardId == standardId && x.Value.GradingPeriodId == gradingPeriodId))
            {
                var item = data.First(x => x.Value.StudentId == studentId && x.Value.SectionId == classId &&
                                           x.Value.StandardId == standardId &&
                                           x.Value.GradingPeriodId == gradingPeriodId).Key;
                data[item] = standardScore;
            }
            return standardScore;
        }


        private void AddStandardScores(int sectionId, int gradingPeriodId, int standardId)
        {
            Add(new StandardScore
            {
                SectionId = sectionId,
                GradingPeriodId = gradingPeriodId,
                StudentId = DemoSchoolConstants.FirstStudentId,
                StandardId = standardId,
                ComputedScore = 100,
                ComputedScoreAlphaGradeName = "A",
                EnteredScoreAlphaGradeName = "A"
            });

            Add(new StandardScore
            {
                SectionId = sectionId,
                GradingPeriodId = gradingPeriodId,
                StudentId = DemoSchoolConstants.SecondStudentId,
                StandardId = standardId,
                ComputedScore = 100,
                ComputedScoreAlphaGradeName = "A",
                EnteredScoreAlphaGradeName = "A"
            });

            Add(new StandardScore
            {
                SectionId = sectionId,
                GradingPeriodId = gradingPeriodId,
                StudentId = DemoSchoolConstants.ThirdStudentId,
                StandardId = standardId,
                ComputedScore = 100,
                ComputedScoreAlphaGradeName = "A",
                EnteredScoreAlphaGradeName = "A"
            });
        }

        public void Setup()
        {
            for (var i = 0; i < 4; ++i)
            {
                AddStandardScores(DemoSchoolConstants.AlgebraClassId, i + 1, DemoSchoolConstants.MathStandard1);
                AddStandardScores(DemoSchoolConstants.AlgebraClassId, i + 1, DemoSchoolConstants.MathStandard2);
                AddStandardScores(DemoSchoolConstants.AlgebraClassId, i + 1, DemoSchoolConstants.MathStandard3);

                AddStandardScores(DemoSchoolConstants.GeometryClassId, i + 1, DemoSchoolConstants.MathStandard1);
                AddStandardScores(DemoSchoolConstants.GeometryClassId, i + 1, DemoSchoolConstants.MathStandard2);
                AddStandardScores(DemoSchoolConstants.GeometryClassId, i + 1, DemoSchoolConstants.MathStandard3);
            }

        }
    }
}
