using System.Collections.Generic;
using System.Linq;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage.sti
{
    public class DemoStiActivityScoreStorage:BaseDemoIntStorage<Score>
    {
        public DemoStiActivityScoreStorage(DemoStorage storage)
            : base(storage, null, true)
        {
        }

        
        public Score GetScore(int sisActivityId, int userId)
        {
            return data.First(x => x.Value.ActivityId == sisActivityId && x.Value.StudentId == userId).Value;
        }

        public IList<Score> GetScores(int userId)
        {
            return data.Where(x => x.Value.StudentId == userId).Select(x => x.Value).ToList();
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

        public void ResetScores(int sisActivityId, IEnumerable<int> studentIds)
        {
            foreach (var studentId in studentIds)
            {
                UpdateScore(sisActivityId, studentId, new Score()
                {
                    ActivityId = sisActivityId,
                    StudentId = studentId
                });
            }
        }
    }
}
