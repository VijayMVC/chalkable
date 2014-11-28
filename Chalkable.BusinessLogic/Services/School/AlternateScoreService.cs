using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAlternateScoreService
    {
        void AddAlternateScores(IList<AlternateScore> alternateScores);
        void EditAlternateScores(IList<AlternateScore> alternateScores);
        void Delete(IList<int> ids);
        IList<AlternateScore> GetAlternateScores();
        AlternateScore GetAlternateScore(int id);
    }

    public class AlternateScoreService : SchoolServiceBase, IAlternateScoreService
    {
        public AlternateScoreService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddAlternateScores(IList<AlternateScore> alternateScores)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AlternateScoreDataAccess(u).Insert(alternateScores));
        }

        public IList<AlternateScore> GetAlternateScores()
        {
            return DoRead(u => new AlternateScoreDataAccess(u).GetAll());
        }
        
        public void Delete(IList<int> ids)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AlternateScoreDataAccess(u).Delete(ids));
        }

        public void EditAlternateScores(IList<AlternateScore> alternateScores)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AlternateScoreDataAccess(u).Update(alternateScores));
        }


        public AlternateScore GetAlternateScore(int id)
        {
            return DoRead(u=>new AlternateScoreDataAccess(u).GetByIdOrNull(id));
        }
    }
}
