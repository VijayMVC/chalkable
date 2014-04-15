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
                if (data.ContainsKey(alternateScore.Id))
                {
                    data[alternateScore.Id] = alternateScore;
                }

            }
        }

        public void Add(IList<AlternateScore> alternateScores)
        {
            foreach (var alternateScore in alternateScores)
            {
                if (!data.ContainsKey(alternateScore.Id))
                {
                    data[alternateScore.Id] = alternateScore;
                }

            }
        }

        public override void Setup()
        {
            throw new System.NotImplementedException();
        }
    }
}
