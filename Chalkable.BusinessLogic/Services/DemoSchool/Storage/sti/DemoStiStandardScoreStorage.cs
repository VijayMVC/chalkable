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

        public IEnumerable<StandardScore> GetStandardScores(int classId, object o, object o1)
        {
            throw new NotImplementedException();
        }

        public StandardScore Update(int classId, int studentId, int standardId, int gradingPeriodId, StandardScore standardScore)
        {
            throw new NotImplementedException();
        }
    }
}
