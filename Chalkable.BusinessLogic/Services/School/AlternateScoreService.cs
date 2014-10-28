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
        void EditAlternateScores(IList<AlternateScore> alternateScores);
        void Delete(int id);
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
        
        public void Delete(IList<int> ids)
        {
            using (var uow = Update())
            {
                new AlternateScoreDataAccess(uow).Delete(ids);
                uow.Commit();
            }
        }

        public void EditAlternateScores(IList<AlternateScore> alternateScores)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                new AlternateScoreDataAccess(uow).Update(alternateScores);
                uow.Commit();
            }
        }


        public AlternateScore GetAlternateScore(int id)
        {
            using (var  uow = Read())
            {
                return new AlternateScoreDataAccess(uow).GetByIdOrNull(id);
            }
        }
    }
}
