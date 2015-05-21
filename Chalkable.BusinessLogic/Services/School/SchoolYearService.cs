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

        public void AssignStudent(IList<StudentSchoolYear> studentAssignments)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StudentSchoolYear>(u).Insert(studentAssignments));
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
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow);
                return da.GetAll();
            }
        }

        public IList<StudentSchoolYear> GetStudentAssignments()
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new DataAccessBase<StudentSchoolYear>(uow);
                return da.GetAll();
            }
        }


        public IList<SchoolYear> Add(IList<SchoolYear> schoolYears)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolYearDataAccess(uow).Insert(schoolYears);
                uow.Commit();
                return schoolYears;
            }
        }

        public void Delete(IList<int> schoolYearIds)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolYearDataAccess(uow).Delete(schoolYearIds);
                uow.Commit();
            }
        }


        public IList<SchoolYear> Edit(IList<SchoolYear> schoolYears)
        {
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolYearDataAccess(uow).Update(schoolYears);
                uow.Commit();
                return schoolYears;
            }
        }

        public void UnassignStudents(IList<StudentSchoolYear> studentSchoolYears)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new DataAccessBase<StudentSchoolYear>(uow).Delete(studentSchoolYears);
                uow.Commit();
            }
        }

        public void EditStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new DataAccessBase<StudentSchoolYear>(uow).Update(studentSchoolYears);
                uow.Commit();
            }
        }
    }
}
