using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAlphaGradeService
    {
        void AddAlphaGrades(IList<AlphaGrade> alphaGrades);
        void EditAlphaGrades(IList<AlphaGrade> alphaGrades);
        void Delete(IList<int> ids);
    }

    public class AlphaGradeService : SchoolServiceBase, IAlphaGradeService
    {
        public AlphaGradeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AlphaGradeDataAccess(u, null).Insert(alphaGrades));
        }

        public void Delete(IList<int> ids)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AlphaGradeDataAccess(u, Context.SchoolLocalId).Delete(ids));
        }

        public void EditAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new AlphaGradeDataAccess(u, null).Update(alphaGrades));
        }
    }
}
