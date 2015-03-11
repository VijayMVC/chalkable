using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
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
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                return da.GetById(id);
            }
        }

        public PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue)
        {
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                return da.GetPage(start, count);
            }
        }

        public void AssignStudent(IList<StudentSchoolYear> studentAssignments)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new StudentSchoolYearDataAccess(uow);
                da.Insert(studentAssignments);
                uow.Commit();
            }
        }
        
        public SchoolYear GetCurrentSchoolYear()
        {
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                if (Context.SchoolYearId.HasValue)
                    return da.GetById(Context.SchoolYearId.Value);
                var nowDate = Context.NowSchoolYearTime.Date;
                var res = da.GetByDate(nowDate);
                return res ?? da.GetLast(nowDate);
            }
        }
        public IList<SchoolYear> GetSortedYears()
        {
            using (var uow = Read())
            {
                var da = new SchoolYearDataAccess(uow, Context.SchoolLocalId);
                return da.GetAll();
            }
        }

        public IList<StudentSchoolYear> GetStudentAssignments()
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new StudentSchoolYearDataAccess(uow);
                return da.GetAll();
            }
        }


        public IList<SchoolYear> Add(IList<SchoolYear> schoolYears)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolYearDataAccess(uow, Context.SchoolLocalId).Insert(schoolYears);
                uow.Commit();
                return schoolYears;
            }
        }

        public void Delete(IList<int> schoolYearIds)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolYearDataAccess(uow, Context.SchoolLocalId).Delete(schoolYearIds);
                uow.Commit();
            }
        }


        public IList<SchoolYear> Edit(IList<SchoolYear> schoolYears)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new SchoolYearDataAccess(uow, Context.SchoolLocalId).Update(schoolYears);
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
                new StudentSchoolYearDataAccess(uow).Delete(studentSchoolYears);
                uow.Commit();
            }
        }

        public void EditStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears)
        {
            if (!BaseSecurity.IsSysAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new StudentSchoolYearDataAccess(uow).Update(studentSchoolYears);
                uow.Commit();
            }
        }
    }
}
