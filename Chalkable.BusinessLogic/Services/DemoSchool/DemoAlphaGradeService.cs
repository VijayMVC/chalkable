using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{

    public class DemoAlphaGradeStorage : BaseDemoIntStorage<AlphaGrade>
    {
        public DemoAlphaGradeStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoAlphaGradeService : DemoSchoolServiceBase, IAlphaGradeService
    {
        private DemoAlphaGradeStorage AlphaGradeStorage { get; set; }

        public DemoAlphaGradeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            AlphaGradeStorage = new DemoAlphaGradeStorage();
        }

        public void AddAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            AlphaGradeStorage.Add(alphaGrades);
        }

        public void EditAlphaGrades(IList<AlphaGrade> alphaGrades)
        {
            AlphaGradeStorage.Update(alphaGrades);
        }

        public void Delete(IList<AlphaGrade> alphaGrades)
        {
            AlphaGradeStorage.Delete(alphaGrades);
        }

        public IList<AlphaGrade> GetAlphaGrades()
        {
            return AlphaGradeStorage.GetAll();
        }

        public IList<AlphaGrade> GetAlphaGradesByClassId(int classId)
        {
            throw new System.NotImplementedException();
        }

        public IList<AlphaGrade> GetStandardsAlphaGradesByClassId(int classId)
        {
            throw new System.NotImplementedException();
        }

        public IList<AlphaGrade> GetStandardsAlphaGradesForSchool(int schoolId)
        {
            throw new System.NotImplementedException();
        }

        public AlphaGrade GetAlphaGradeById(int id)
        {
            return AlphaGradeStorage.GetById(id);
        }
    }
}
