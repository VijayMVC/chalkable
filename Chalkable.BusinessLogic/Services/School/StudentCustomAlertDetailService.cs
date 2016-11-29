using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IStudentCustomAlertDetailService
    {
        void Add(IList<StudentCustomAlertDetail> studentCustomAlertDetail);
        void Edit(IList<StudentCustomAlertDetail> studentCustomAlertDetail);
        void Delete(IList<StudentCustomAlertDetail> studentCustomAlertDetail);
        IList<StudentCustomAlertDetail> GetList(int studentId);
        IList<StudentCustomAlertDetail> GetList(IList<Student> students);
    }

    class StudentCustomAlertDetailService : SchoolServiceBase, IStudentCustomAlertDetailService
    {
        public StudentCustomAlertDetailService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<StudentCustomAlertDetail> studentCustomAlertDetail)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentCustomAlertDetail>(u).Insert(studentCustomAlertDetail));
        }

        public void Edit(IList<StudentCustomAlertDetail> studentCustomAlertDetail)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentCustomAlertDetail>(u).Update(studentCustomAlertDetail));
        }

        public void Delete(IList<StudentCustomAlertDetail> studentCustomAlertDetail)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentCustomAlertDetail>(u).Delete(studentCustomAlertDetail));
        }

        public IList<StudentCustomAlertDetail> GetList(int studentId)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);

            if (BaseSecurity.IsStudent(Context))
                return new List<StudentCustomAlertDetail>();

            return DoRead(u => new DataAccessBase<StudentCustomAlertDetail>(u).GetAll(new AndQueryCondition
            {
                {nameof(StudentCustomAlertDetail.StudentRef), studentId},
                {nameof(StudentCustomAlertDetail.SchoolYearRef), Context.SchoolYearId.Value}
            }).OrderBy(x => x.AlertText).ToList());
        }

        public IList<StudentCustomAlertDetail> GetList(IList<Student> students)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);

            return DoRead(u => new StudentCustomAlertDetailDataAccess(u).GetList(students.Select(x => x.Id).ToList(), Context.SchoolYearId.Value));
        }
    }
}
