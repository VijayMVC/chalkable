using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Security;
using Chalkable.Data.Common;
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
        IList<StudentCustomAlertDetail> GetListByStudentIds(IList<int> studentIds);
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
            return GetListByStudentIds(new List<int> {studentId});
        }

        public IList<StudentCustomAlertDetail> GetListByStudentIds(IList<int> studentIds)
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            if (BaseSecurity.IsStudent(Context))
                return new List<StudentCustomAlertDetail>();

            return DoRead(u => new StudentCustomAlertDetailDataAccess(u).GetList(studentIds, Context.SchoolYearId.Value));
        }
    }
}
