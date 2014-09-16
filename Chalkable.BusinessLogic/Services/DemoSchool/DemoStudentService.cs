using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoStudentService : DemoSchoolServiceBase, IStudentService
    {
        public DemoStudentService(IServiceLocatorSchool serviceLocator, DemoStorage demoStorage) : base(serviceLocator, demoStorage)
        {
        }

        public void AddStudents(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void EditStudents(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudents(IList<Student> students)
        {
            throw new NotImplementedException();
        }

        public void AddStudentSchools(IList<StudentSchool> studentSchools)
        {
            throw new NotImplementedException();
        }

        public void EditStudentSchools(IList<StudentSchool> studentSchools)
        {
            throw new NotImplementedException();
        }

        public void DeleteStudentSchools(IList<StudentSchool> studentSchools)
        {
            throw new NotImplementedException();
        }
    }
}
