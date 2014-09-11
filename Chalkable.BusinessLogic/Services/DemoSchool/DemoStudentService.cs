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
            Storage.DemoStudentStorage.Add(students);
        }

        public void EditStudents(IList<Student> students)
        {
            Storage.DemoStudentStorage.Update(students);
        }

        public void DeleteStudents(IList<Student> students)
        {
            Storage.DemoStudentStorage.Delete(students);
        }

        public void AddStudentSchools(IList<StudentSchool> studentSchools)
        {
            Storage.DemoStudentSchoolStorage.Add(studentSchools);
        }

        public void EditStudentSchools(IList<StudentSchool> studentSchools)
        {
            Storage.DemoStudentSchoolStorage.Update(studentSchools);
        }

        public void DeleteStudentSchools(IList<StudentSchool> studentSchools)
        {
            Storage.DemoStudentSchoolStorage.Delete(studentSchools);
        }
    }
}
