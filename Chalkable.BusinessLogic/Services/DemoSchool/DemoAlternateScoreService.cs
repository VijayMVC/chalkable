using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
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
            if(!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            Storage.AlternateScoreStorage.Add(alternateScores);
        }

        public void EditAlternateScores(IList<AlternateScore> alternateScores)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();


            Storage.AlternateScoreStorage.Update(alternateScores);
        }

        public void Delete(int id)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.AlternateScoreStorage.Delete(id);
            
        }

        public void Delete(IList<int> ids)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.AlternateScoreStorage.Delete(ids);
        }

        public IList<AlternateScore> GetAlternateScores()
        {
            return Storage.AlternateScoreStorage.GetAll();
        }
    }
}
