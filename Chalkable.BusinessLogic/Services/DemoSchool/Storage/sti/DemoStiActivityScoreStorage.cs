using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiActivityScoreStorage:BaseDemoStorage<int, Score>
    {
        public DemoStiActivityScoreStorage(DemoStorage storage)
            : base(storage)
        {
        }

        public Score GetScore(int value, int i)
        {
            throw new NotImplementedException();
        }

        public IList<Score> GetSores(int value)
        {
            throw new NotImplementedException();
        }

        public Score UpdateScore(int activityId, int studentId, Score score)
        {
            throw new NotImplementedException();
        }
    }
}
