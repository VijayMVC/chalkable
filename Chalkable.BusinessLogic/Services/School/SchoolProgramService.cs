using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolProgramService
    {
        void Add(IList<SchoolProgram> schoolProgram);
        void Edit(IList<SchoolProgram> schoolProgram);
        void Delete(IList<SchoolProgram> schoolProgram);
    }
    public class SchoolProgramService : SchoolServiceBase, ISchoolProgramService
    {
        public SchoolProgramService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<SchoolProgram> schoolProgram)
        {
            DoUpdate(u => new DataAccessBase<SchoolProgram>(u).Insert(schoolProgram));
        }

        public void Edit(IList<SchoolProgram> schoolProgram)
        {
            DoUpdate(u => new DataAccessBase<SchoolProgram>(u).Update(schoolProgram));
        }

        public void Delete(IList<SchoolProgram> schoolProgram)
        {
            DoUpdate(u => new DataAccessBase<SchoolProgram>(u).Delete(schoolProgram));
        }
    }
}
