using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolYearService
    {
        IList<SchoolYear> Add(IList<SchoolYear> schoolYears); 
        IList<SchoolYear> Edit(IList<SchoolYear> schoolYears); 
        SchoolYear GetSchoolYearById(int id);
        PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue, int? schoolId = null);
        IList<int> GetYears(); 
        void Delete(IList<int> schoolYearIds);
        SchoolYear GetCurrentSchoolYear();
        IList<SchoolYear> GetPreviousSchoolYears(int fromSchoolYearid, int count = 1);
        IList<SchoolYear> GetSchoolYearsByAcadYear(int year, bool activeOnly = true); 
        IList<StudentSchoolYear> GetStudentAssignments();
        void AssignStudent(IList<StudentSchoolYear> studentAssignments);
        void UnassignStudents(IList<StudentSchoolYear> studentSchoolYears);
        void EditStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears);
        IList<SchoolYear> GetDescSortedYearsByIds(IList<int> ids);
        StudentSchoolYear GetPreviousStudentSchoolYearOrNull(int studentId);
        IList<SchoolYear> GetSchoolYearsByStudent(int studentId, StudentEnrollmentStatusEnum? enrollmentStatus, DateTime? date);
        SchoolYear GetCurrentSchoolYearByStudent(int studentId);
        void PrepareToDeleteStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears);
    }

    public class SchoolYearService : SisConnectedService, ISchoolYearService
    {
        public SchoolYearService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }
        
        public SchoolYear GetSchoolYearById(int id)
        {
            return DoRead(u => new SchoolYearDataAccess(u).GetById(id));
        }

        public PaginatedList<SchoolYear> GetSchoolYears(int start = 0, int count = int.MaxValue, int? schoolId = null)
        {
            var acadSessions = ConnectorLocator.UsersConnector.GetUserAcadSessionsIds(); 
            
            var conds = new AndQueryCondition();
            if(schoolId.HasValue)
                conds.Add(nameof(SchoolYear.SchoolRef), schoolId.Value);

            var res = DoRead(u => new SchoolYearDataAccess(u).GetAll(conds));
            res = res.Where(x => acadSessions.Contains(x.Id)).ToList();

            if (res.Count == 0)
                throw new ChalkableException("Current user does not have access to any of school acadSessions");

            res = res.OrderBy(x => x.StartDate).ToList();

            return new PaginatedList<SchoolYear>(res, start/count, count, res.Count);
        }

        public IList<int> GetYears()
        {
            var schoolYears = ServiceLocator.SchoolYearService.GetSchoolYears();
            return schoolYears.Select(x => x.AcadYear).Distinct().OrderBy(x => x).ToList();
        }

        public IList<SchoolYear> GetPreviousSchoolYears(int fromSchoolYearId, int count = 1)
        {
            var current = GetSchoolYearById(fromSchoolYearId);
            return DoRead(u => new SchoolYearDataAccess(u).GetPreviousSchoolYears(current.StartDate ?? Context.NowSchoolYearTime, current.SchoolRef, count));
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
        
        public IList<SchoolYear> GetSchoolYearsByAcadYear(int year, bool activeOnly = true)
        {
            var conds = new AndQueryCondition {{SchoolYear.ACAD_YEAR_FIELD, year}};
            if(activeOnly)
                conds.Add(SchoolYear.ARCHIVE_DATE, null);
            return DoRead(u=> new SchoolYearDataAccess(u).GetAll(conds));
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

        public IList<SchoolYear> GetDescSortedYearsByIds(IList<int> ids)
        {
            return DoRead(u => new SchoolYearDataAccess(u).GetByIds(ids).OrderByDescending(x => x.StartDate).ToList());
        }

        public IList<StudentSchoolYear> GetStudentAssignments()
        {
            BaseSecurity.EnsureDistrictAdmin(Context);
            return DoRead(u => new DataAccessBase<StudentSchoolYear>(u).GetAll());
        }

        public StudentSchoolYear GetPreviousStudentSchoolYearOrNull(int studentId)
        {
            return DoRead(u => new SchoolYearDataAccess(u).GetPreviousStudentSchoolYearOrNull(studentId));
        }

        public IList<SchoolYear> GetSchoolYearsByStudent(int studentId, StudentEnrollmentStatusEnum? enrollmentStatus, DateTime? date)
        {
            var res = DoRead(u => new SchoolYearDataAccess(u).GetSchoolYearsByStudent(studentId, enrollmentStatus));

            if(date.HasValue)
                res = res.Where(x => x.StartDate <= date && x.EndDate >= date).ToList();

            return res;
        }

        public SchoolYear GetCurrentSchoolYearByStudent(int studentId)
        {
            var sys = ServiceLocator.SchoolYearService.GetSchoolYearsByStudent(studentId, StudentEnrollmentStatusEnum.CurrentlyEnrolled, null);

            //sys = sys.Intersect(GetSchoolYears()).ToList();

            sys = sys.OrderByDescending(x => x.EndDate).ToList();

            var res = sys.FirstOrDefault(x => x.Id == Context.SchoolYearId);
            if (res == null)
                res = sys.FirstOrDefault(x => x.StartDate <= Context.NowSchoolTime && x.EndDate >= Context.NowSchoolTime);
            if (res == null)
                res = sys.FirstOrDefault(x => x.SchoolRef == Context.SchoolLocalId);
            if (res == null)
                res = sys.FirstOrDefault();
            return res ?? GetCurrentSchoolYear();
        }

        public void PrepareToDeleteStudentSchoolYears(IList<StudentSchoolYear> studentSchoolYears)
        {
            DoUpdate(u => new DataAccessBase<StudentSchoolYear>(u).PrepareToDelete(studentSchoolYears));
        }
    }
}
