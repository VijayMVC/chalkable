using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStudentSchoolProgramService
    {
        void Add(IList<StudentSchoolProgram> studentSchoolPrograms);
        void Edit(IList<StudentSchoolProgram> studentSchoolPrograms);
        void Delete(IList<StudentSchoolProgram> studentSchoolPrograms);
    }
    public class StudentSchoolProgramService : SchoolServiceBase, IStudentSchoolProgramService
    {
        public StudentSchoolProgramService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<StudentSchoolProgram> studentSchoolPrograms)
        {
            DoUpdate(u => new DataAccessBase<StudentSchoolProgram>(u).Insert(studentSchoolPrograms));
        }

        public void Edit(IList<StudentSchoolProgram> studentSchoolPrograms)
        {
            DoUpdate(u => new DataAccessBase<StudentSchoolProgram>(u).Update(studentSchoolPrograms));
        }

        public void Delete(IList<StudentSchoolProgram> studentSchoolPrograms)
        {
            DoUpdate(u => new DataAccessBase<StudentSchoolProgram>(u).Delete(studentSchoolPrograms));
        }
    }
}
