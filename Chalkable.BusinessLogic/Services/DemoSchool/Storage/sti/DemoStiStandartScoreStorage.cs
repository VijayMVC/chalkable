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
            return new List<StandardScore>();
        }

        public StandardScore Update(int classId, int studentId, int standardId, int gradingPeriodId, StandardScore standardScore)
        {
            throw new NotImplementedException();
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
