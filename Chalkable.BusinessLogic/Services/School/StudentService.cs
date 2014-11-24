using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStudentService
    {
        void AddStudents(IList<Student> students);
        void EditStudents(IList<Student> students);
        void DeleteStudents(IList<Student> students);

        void AddStudentSchools(IList<StudentSchool> studentSchools);
        void EditStudentSchools(IList<StudentSchool> studentSchools);
        void DeleteStudentSchools(IList<StudentSchool> studentSchools);

        StudentDetails GetById(int id, int schoolYearId);
    }

    public class StudentService : SchoolServiceBase, IStudentService
    {
        public StudentService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddStudents(IList<Student> students)
        {
            ModifyStudet(da => da.Insert(students));
        }

        public void EditStudents(IList<Student> students)
        {
            ModifyStudet(da => da.Update(students));
        }

        public void DeleteStudents(IList<Student> students)
        {
            ModifyStudet(da => da.Delete(students));
        }
        private void ModifyStudet(Action<StudentDataAccess> action)
        {
            Modify(uow => action(new StudentDataAccess(uow)));
        }
        private void ModifyStudentSchool(Action<StudentSchoolDataAccess> action)
        {
            Modify(uow => action(new StudentSchoolDataAccess(uow, Context.SchoolLocalId)));
        }
        private void Modify(Action<UnitOfWork> modifyAction)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                modifyAction(uow);
                uow.Commit();
            }
        }
        public void AddStudentSchools(IList<StudentSchool> studentSchools)
        {
            ModifyStudentSchool(da => da.Insert(studentSchools));
        }
        public void EditStudentSchools(IList<StudentSchool> studentSchools)
        {
            ModifyStudentSchool(da => da.Update(studentSchools));
        }
        public void DeleteStudentSchools(IList<StudentSchool> studentSchools)
        {
            ModifyStudentSchool(da => da.Delete(studentSchools));
        }

        public StudentDetails GetById(int id, int schoolYearId)
        {
            using (var uow = Read())
            {
                var da = new StudentDataAccess(uow);
                return da.GetDetailsById(id, schoolYearId);
            }
        }
    }
}
