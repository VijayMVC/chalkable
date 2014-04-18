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

        public void Add(Score s)
        {
            data.Add(GetNextFreeId(), s);
        }

        public Score GetScore(int sisActivityId, int userId)
        {
            return data.First(x => x.Value.ActivityId == sisActivityId && x.Value.StudentId == userId).Value;
        }

        public IList<Score> GetSores(int sisActivityId)
        {
            return data.Where(x => x.Value.ActivityId == sisActivityId).Select(x => x.Value).ToList();
        }

        public Score UpdateScore(int activityId, int studentId, Score score)
        {
            var item = data.First(x => x.Value.ActivityId == activityId && x.Value.StudentId == studentId);
            data[item.Key] = score;
            return score;
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
