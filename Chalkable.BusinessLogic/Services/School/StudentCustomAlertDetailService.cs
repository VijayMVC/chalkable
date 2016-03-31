using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStudentCustomAlertDetailService
    {
        void AddStudentCustomAlertDetail(IList<StudentCustomAlertDetail> studentCustomAlertDetail);
        void EditStudentCustomAlertDetail(IList<StudentCustomAlertDetail> studentCustomAlertDetail);
        void DeleteStudentCustomAlertDetail(IList<StudentCustomAlertDetail> studentCustomAlertDetail);
        IList<StudentCustomAlertDetail> GetStudentCustomAlertDetailById(int studentId, int schoolYear);
    }

    class StudentCustomAlertDetailService : SchoolServiceBase, IStudentCustomAlertDetailService
    {
        public StudentCustomAlertDetailService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void AddStudentCustomAlertDetail(IList<StudentCustomAlertDetail> studentCustomAlertDetail)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentCustomAlertDetail>(u).Insert(studentCustomAlertDetail));
        }

        public void EditStudentCustomAlertDetail(IList<StudentCustomAlertDetail> studentCustomAlertDetail)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentCustomAlertDetail>(u).Update(studentCustomAlertDetail));
        }

        public void DeleteStudentCustomAlertDetail(IList<StudentCustomAlertDetail> studentCustomAlertDetail)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentCustomAlertDetail>(u).Delete(studentCustomAlertDetail));
        }

        public IList<StudentCustomAlertDetail> GetStudentCustomAlertDetailById(int studentId, int schoolYear)
        {
            return DoRead(u => new DataAccessBase<StudentCustomAlertDetail>(u).GetAll(new AndQueryCondition
            {
                {nameof(StudentCustomAlertDetail.StudentRef), studentId},
                {nameof(StudentCustomAlertDetail.SchoolYearRef), schoolYear}
            }));
        }
    }
}
