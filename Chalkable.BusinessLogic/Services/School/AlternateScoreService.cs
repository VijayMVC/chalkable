using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAlternateScoreService
    {
        void AddAlternateScores(IList<AlternateScore> alternateScores);
        void Delete(int id);
        IList<AlternateScore> GetAlternateScores();
    }

    public class AlternateScoreService : SchoolServiceBase, IAlternateScoreService
    {
        public AlternateScoreService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddAlternateScores(IList<AlternateScore> alternateScores)
        {
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                new AlternateScoreDataAccess(uow).Insert(alternateScores);
                uow.Commit();
            }
        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new AlternateScoreDataAccess(uow).Delete(id);
                uow.Commit();
            }
        }

        public IList<AlternateScore> GetAlternateScores()
        {
            using (var uow = Read())
            {
                return new AlternateScoreDataAccess(uow).GetAll();
            }
        }
    }
}
