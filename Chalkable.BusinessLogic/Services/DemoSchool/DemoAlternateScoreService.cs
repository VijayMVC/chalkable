using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoAlternateScoreService : DemoSchoolServiceBase, IAlternateScoreService
    {
        public DemoAlternateScoreService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public void AddAlternateScores(IList<AlternateScore> alternateScores)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            Storage.AlternateScoreStorage.Add(alternateScores);
        }

        public void EditAlternateScores(IList<AlternateScore> alternateScores)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            Storage.AlternateScoreStorage.Update(alternateScores);
        }

        public void Delete(IList<int> ids)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            Storage.AlternateScoreStorage.Delete(ids);
        }

        public IList<AlternateScore> GetAlternateScores()
        {
            return Storage.AlternateScoreStorage.GetAll();
        }


        public AlternateScore GetAlternateScore(int id)
        {
            return Storage.AlternateScoreStorage.GetById(id);
        }
    }
}
