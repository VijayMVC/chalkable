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
        IList<AlphaGrade> GetAlphaGrades();

        IList<AlphaGrade> GetAlphaGradesForClass(int classId);
        IList<AlphaGrade> GetAlphaGradesForClassStandards(int classId);
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

        public IList<AlphaGrade> GetAlphaGrades()
        {
            return DoRead(u => new AlphaGradeDataAccess(u, Context.SchoolLocalId).GetList());
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
        
        public IList<AlphaGrade> GetAlphaGradesForClass(int classId)
        {
            return DoRead(u => new AlphaGradeDataAccess(u, Context.SchoolLocalId).GetForClass(classId));
        }

        public IList<AlphaGrade> GetAlphaGradesForClassStandards(int classId)
        {
            return DoRead(u => new AlphaGradeDataAccess(u, Context.SchoolLocalId).GetForClassStandards(classId));
        }
    }
}
