using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAlternateScoreStorage:BaseDemoStorage<int, AlternateScore>
    {

        public DemoAlternateScoreStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Update(IList<AlternateScore> alternateScores)
        {
            foreach (var alternateScore in alternateScores)
            {
                var score = data.FirstOrDefault(x => x.Key == alternateScore.Id).Value;
                if (score != null)
                {
                    data[alternateScore.Id] = alternateScore;
                }

            }
        }
    }
}
