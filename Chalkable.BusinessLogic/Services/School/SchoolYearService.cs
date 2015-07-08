using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolYearService
    {
        IList<SchoolYear> Add(IList<SchoolYear> schoolYears); 
        IList<SchoolYear> Edit(IList<SchoolYear> schoolYears); 
        SchoolYear GetSchoolYearById(int id);
        PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue);
        void Delete(IList<int> schoolYearIds);
        SchoolYear GetCurrentSchoolYear();
        IList<SchoolYear> GetSortedYears();
        IList<StudentSchoolYear> GetStudentAssignments();
        void AssignStudent(IList<StudentSchoolYear> studentAssignments);
        void UnassignStudents(IList<StudentSchoolYear> studentSchoolYears);
        void EditStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears);
        
    }

    public class SchoolYearService : SchoolServiceBase, ISchoolYearService
    {
        public SchoolYearService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        public SchoolYear GetSchoolYearById(int id)
        {
            return DoRead(u => new SchoolYearDataAccess(u).GetById(id));
        }

        public PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue)
        {
            return DoRead(u => new SchoolYearDataAccess(u).GetPage(start, count));
        }

       
        public SchoolYear GetCurrentSchoolYear()
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow);
                if (Context.SchoolYearId.HasValue)
                    return da.GetById(Context.SchoolYearId.Value);
                var nowDate = Context.NowSchoolYearTime.Date;
                var res = da.GetByDate(nowDate, Context.SchoolLocalId.Value);
                return res ?? da.GetLast(nowDate, Context.SchoolLocalId.Value);
            }
        }

        public IList<SchoolYear> GetSortedYears()
        {
            return DoRead(u => new SchoolYearDataAccess(u).GetAll());
        }

        public IList<SchoolYear> Add(IList<SchoolYear> schoolYears)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new SchoolYearDataAccess(u).Insert(schoolYears));
            return schoolYears;
        }

        public void Delete(IList<int> schoolYearIds)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new SchoolYearDataAccess(u).Delete(schoolYearIds));
        }


        public IList<SchoolYear> Edit(IList<SchoolYear> schoolYears)
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            DoUpdate(u => new SchoolYearDataAccess(u).Update(schoolYears));
            return schoolYears;
        }
        
        public void AssignStudent(IList<StudentSchoolYear> studentAssignments)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentSchoolYear>(u).Insert(studentAssignments));
        }

        public void UnassignStudents(IList<StudentSchoolYear> studentSchoolYears)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentSchoolYear>(u).Delete(studentSchoolYears));
        }

        public void EditStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentSchoolYear>(u).Update(studentSchoolYears));
        }

        public IList<StudentSchoolYear> GetStudentAssignments()
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            return DoRead(u => new DataAccessBase<StudentSchoolYear>(u).GetAll());
        }

    }
}
