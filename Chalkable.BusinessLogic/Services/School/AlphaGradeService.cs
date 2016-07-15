using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAlphaGradeService
    {
        void AddAlphaGrades(IList<AlphaGrade> alphaGrades);
        void EditAlphaGrades(IList<AlphaGrade> alphaGrades);
        void Delete(IList<AlphaGrade> alphaGrades);
        IList<AlphaGrade> GetAlphaGrades();
        IList<AlphaGrade> GetAlphaGradesByClassId(int classId);
        IList<AlphaGrade> GetStandardsAlphaGradesByClassId(int classId);
        IList<AlphaGrade> GetStandardsAlphaGradesForSchool(int schoolId);
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

        public IList<AlphaGrade> GetAlphaGrades()
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            return DoRead(u => new AlphaGradeDataAccess(u).GetAlphaGradesBySchoolId(Context.SchoolLocalId.Value));
        }

        public IList<AlphaGrade> GetAlphaGradesByClassId(int classId)
        {
            var res = DoRead(u => new AlphaGradeDataAccess(u).GetAlphaGradeForClasses(new List<int> {classId}));
            return res[classId];
        }
        public IList<AlphaGrade> GetStandardsAlphaGradesByClassId(int classId)
        {
            var res = DoRead(u => new AlphaGradeDataAccess(u).GetAlphaGradesForClassStandards(new List<int> { classId }));
            return res[classId];
        }

        public IList<AlphaGrade> GetStandardsAlphaGradesForSchool(int schoolId)
        {
            var res = DoRead(u => new AlphaGradeDataAccess(u).GetAlphaGradesForSchoolStandarts(new List<int> {schoolId}));
            return res[schoolId];
        }

        public void EditAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<AlphaGrade>(u).Update(alphaGrades));
        }
    }
}
