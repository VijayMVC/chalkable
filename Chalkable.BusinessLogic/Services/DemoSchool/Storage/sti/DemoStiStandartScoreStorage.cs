using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiStandardScoreStorage:BaseDemoStorage<int, StandardScore>
    {
        public DemoStiStandardScoreStorage(DemoStorage storage)
            : base(storage)
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
            throw new NotImplementedException();
        }

        public override void Setup()
        {
            data.Add(GetNextFreeId(), new StandardScore
            {
                SectionId = 1,
                GradingPeriodId = 1,
                StudentId = 1196,
                StandardId = 1
            });

            data.Add(GetNextFreeId(), new StandardScore
            {
                SectionId = 1,
                GradingPeriodId = 2,
                StudentId = 1196,
                StandardId = 1
            });

            data.Add(GetNextFreeId(), new StandardScore
            {
                SectionId = 1,
                GradingPeriodId = 3,
                StudentId = 1196,
                StandardId = 1
            });

            data.Add(GetNextFreeId(), new StandardScore
            {
                SectionId = 1,
                GradingPeriodId = 4,
                StudentId = 1196,
                StandardId = 1
            });

            data.Add(GetNextFreeId(), new StandardScore
            {
                SectionId = 2,
                GradingPeriodId = 1,
                StudentId = 1196,
                StandardId = 1
            });

            data.Add(GetNextFreeId(), new StandardScore
            {
                SectionId = 2,
                GradingPeriodId = 2,
                StudentId = 1196,
                StandardId = 1
            });

            data.Add(GetNextFreeId(), new StandardScore
            {
                SectionId = 2,
                GradingPeriodId = 3,
                StudentId = 1196,
                StandardId = 1
            });

            data.Add(GetNextFreeId(), new StandardScore
            {
                SectionId = 2,
                GradingPeriodId = 4,
                StudentId = 1196,
                StandardId = 1
            });
        }
    }
}
