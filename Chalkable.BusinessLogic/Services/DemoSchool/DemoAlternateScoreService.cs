using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAlternateScoreStorage : BaseDemoIntStorage<AlternateScore>
    {
        public DemoAlternateScoreStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoAlternateScoreService : DemoSchoolServiceBase, IAlternateScoreService
    {

        public DemoAlternateScoreStorage AlternateScoreStorage { get; set; }

        public DemoAlternateScoreService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            AlternateScoreStorage = new DemoAlternateScoreStorage();
        }

        public void AddAlternateScores(IList<AlternateScore> alternateScores)
        {
            AlternateScoreStorage.Add(alternateScores);
        }

        public void EditAlternateScores(IList<AlternateScore> alternateScores)
        {
            AlternateScoreStorage.Update(alternateScores);
        }

        public void Delete(IList<AlternateScore> alternateScores)
        {
            AlternateScoreStorage.Delete(alternateScores);
        }

        public IList<AlternateScore> GetAlternateScores()
        {
            return AlternateScoreStorage.GetAll();
        }

        public AlternateScore GetAlternateScore(int id)
        {
            return AlternateScoreStorage.GetById(id);
            
        }
    }
}
