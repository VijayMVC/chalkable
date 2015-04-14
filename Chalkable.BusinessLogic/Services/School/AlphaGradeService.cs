using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAlphaGradeService
    {
        void AddAlphaGrades(IList<AlphaGrade> alphaGrades);
        void EditAlphaGrades(IList<AlphaGrade> alphaGrades);
        void Delete(IList<AlphaGrade> alphaGrades);
    }

    public class AlphaGradeService : SchoolServiceBase, IAlphaGradeService
    {
        public AlphaGradeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<AlphaGrade>(u).Insert(alphaGrades));
        }

        public void Delete(IList<AlphaGrade> alphaGrades)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<AlphaGrade>(u).Delete(alphaGrades));
        }

        public void EditAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<AlphaGrade>(u).Update(alphaGrades));
        }
    }
}
