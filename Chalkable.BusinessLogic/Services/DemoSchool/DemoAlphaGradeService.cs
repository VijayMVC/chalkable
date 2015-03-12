using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
  
    public class DemoAlphaGradeService : DemoSchoolServiceBase, IAlphaGradeService
    {
        public DemoAlphaGradeService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public void AddAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();

            Storage.AlphaGradeStorage.Add(alphaGrades);
            
        }

        public void EditAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.AlphaGradeStorage.Update(alphaGrades);

        }

        public void Delete(IList<AlphaGrade> alphaGrades)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            Storage.AlphaGradeStorage.Delete(alphaGrades);
        }
    }
}
