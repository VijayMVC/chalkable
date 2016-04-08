using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Chalkable.BusinessLogic.Mapping.EnumMappers;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.BusinessLogic.Services.School
{
    public enum TeacherSortType
    {
        TeacherAsc = 0,
        TeacherDesc,
        StudentsAsc,
        StudentsDesc,
        AttendanceAsc,
        AttendanceDesc,
        DisciplineAsc,
        DisciplineDesc,
        GradesAsc,
        GradesDesc
    }
    public interface IStaffService
    {
        void Add(IList<Staff> staffs);
        void Edit(IList<Staff> staffs);
        void Delete(IList<Staff> staffs);
        IList<Staff> GetStaffs();
        Staff GetStaff(int staffId);
        void AddStaffSchools(IList<StaffSchool> staffSchools);
        void EditStaffSchools(IList<StaffSchool> staffSchools);
        void DeleteStaffSchools(IList<StaffSchool> staffSchools);
        IList<StaffSchool> GetStaffSchools();
        PaginatedList<Staff> SearchStaff(int? schoolYearId, int? classId, int? studentId, string filter, bool orderByFirstName, int start, int count);
        IList<TeacherStatsInfo> GetTeachersStats(int schoolYearId, string filter, int? start, int? count, TeacherSortType? sortType);
    }

    public class StaffService : SisConnectedService, IStaffService
    {
        public StaffService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(IList<Staff> staffs)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StaffDataAccess(u).Insert(staffs));
        }
        public void Edit(IList<Staff> staffs)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StaffDataAccess(u).Update(staffs));
        }
        public void Delete(IList<Staff> staffs)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new StaffDataAccess(u).Delete(staffs));
        }

        public Staff GetStaff(int staffId)
        {
            return DoRead(uow => new StaffDataAccess(uow).GetById(staffId));
        }

        public IList<Staff> GetStaffs()
        {
            return DoRead(uow => new StaffDataAccess(uow).GetAll());
        }

        public void AddStaffSchools(IList<StaffSchool> staffSchools)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StaffSchool>(u).Insert(staffSchools));
        }

        public void EditStaffSchools(IList<StaffSchool> staffSchools)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StaffSchool>(u).Update(staffSchools));
        }

        public void DeleteStaffSchools(IList<StaffSchool> staffSchools)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<StaffSchool>(u).Delete(staffSchools));
        }

        public IList<StaffSchool> GetStaffSchools()
        {
            return DoRead(uow => new DataAccessBase<StaffSchool>(uow).GetAll());
        }
        
        public PaginatedList<Staff> SearchStaff(int? schoolYearId, int? classId, int? studentId, string filter, bool orderByFirstName,
            int start, int count)
        {
            schoolYearId = schoolYearId ?? ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            return DoRead(u => new StaffDataAccess(u).SearchStaff(schoolYearId, classId, studentId, filter, orderByFirstName,
                            start, count));
        }

        public IList<TeacherStatsInfo> GetTeachersStats(int schoolYearId, string filter, int? start, int? count, TeacherSortType? sortType)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            var iNowSortType = EnumMapperFactory.GetMapper<TeacherSortType, SectionSummarySortOption>()
                    .Map(sortType ?? TeacherSortType.TeacherAsc);

            try
            {
                var iNowRes = ConnectorLocator.ClassesDashboardConnector.GetTeachersSummaries(schoolYearId,
                    Context.NowSchoolYearTime, start.Value + 1, start.Value + count.Value, filter, iNowSortType);

                return iNowRes.Select(TeacherStatsInfo.Create).ToList();
            }
            catch (ChalkableSisNotSupportVersionException)
            {
                using (var u = Read())
                {
                    var teachers = new StaffDataAccess(u).GetShortStaffSummary(schoolYearId, filter, start.Value, count.Value, (int?) sortType);
                    var classes =new ClassDataAccess(u).GetClassesByTeachers(schoolYearId, teachers.Select(x => x.Id).ToList());
                    return TeacherStatsInfo.Create(teachers, classes);
                }               
            }
        }

    }
}
