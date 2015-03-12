using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAlternateScoreService
    {
        void AddAlternateScores(IList<AlternateScore> alternateScores);
        void EditAlternateScores(IList<AlternateScore> alternateScores);
        void Delete(IList<AlternateScore> alternateScores);
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
            DoUpdate(u => new DataAccessBase<AlternateScore>(u).Insert(alternateScores));
        }

        public IList<AlternateScore> GetAlternateScores()
        {
            return DoRead(u => new DataAccessBase<AlternateScore>(u).GetAll());
        }

        public void Delete(IList<AlternateScore> alternateScores)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<AlternateScore>(u).Delete(alternateScores));
        }

        public void EditAlternateScores(IList<AlternateScore> alternateScores)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<AlternateScore>(u).Update(alternateScores));
        }


        public AlternateScore GetAlternateScore(int id)
        {
            return DoRead(u => new DataAccessBase<AlternateScore, int>(u).GetByIdOrNull(id));
        }
    }
}
