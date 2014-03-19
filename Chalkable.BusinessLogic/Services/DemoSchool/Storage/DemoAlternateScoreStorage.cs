using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAlternateScoreStorage:BaseDemoStorage
    {
        private readonly Dictionary<int, AlternateScore> alternateScoresData = new Dictionary<int, AlternateScore>();

        public DemoAlternateScoreStorage(DemoStorage storage) : base(storage)
        {
        }

        public void Add(IList<AlternateScore> alternateScores)
        {
            foreach (var alternateScore in alternateScores)
            {
                var score = alternateScoresData.FirstOrDefault(x => x.Key == alternateScore.Id).Value;
                if (score == null)
                {
                    alternateScoresData[alternateScore.Id] = alternateScore;
                }
            }
        }

        public void Delete(int id)
        {
            alternateScoresData.Remove(id);
        }

        public IList<AlternateScore> GetAll()
        {
            return alternateScoresData.Select(x => x.Value).ToList();
        }

        public void Delete(IList<int> ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        public void Update(IList<AlternateScore> alternateScores)
        {
            foreach (var alternateScore in alternateScores)
            {
                var score = alternateScoresData.FirstOrDefault(x => x.Key == alternateScore.Id).Value;
                if (score != null)
                {
                    alternateScoresData[alternateScore.Id] = alternateScore;
                }

            }
        }
    }
}
