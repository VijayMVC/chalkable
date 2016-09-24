using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.StiConnector.Connectors.Model;
using Chalkable.StiConnector.Exceptions;

namespace Chalkable.BusinessLogic.Services.School
{
    public enum SchoolSortType
    {
        SchoolAsc = 0,
        SchoolDesc,

        StudentsAsc,
        StudentsDesc,

        AttendanceAsc,
        AttendanceDesc,

        DisciplineAsc,
        DisciplineDesc,

        GradesAsc,
        GradesDesc
    }

    public interface ISchoolService
    {
        void Add(Data.School.Model.School school);
        void Add(IList<Data.School.Model.School> schools);
        void Edit(IList<Data.School.Model.School> schools);
        void Delete(IList<Data.School.Model.School> schools);
        IList<Data.School.Model.School> GetSchools();
        IList<Data.School.Model.School> GetSchoolsByIds(IList<int> schoolIds);
        Data.School.Model.School GetSchool(int schoolId);
        void AddSchoolOptions(IList<SchoolOption> schoolOptions);
        void EditSchoolOptions(IList<SchoolOption> schoolOptions);
        void DeleteSchoolOptions(IList<SchoolOption> schoolOptions);
        SchoolOption GetSchoolOption();
        StartupData GetStartupData();
        IList<SchoolSummaryInfo> GetShortSchoolSummariesInfo(int? start, int? count, string filter, SchoolSortType? sortType);
        int GetSchoolsCount(string filter = null);
        IList<Data.School.Model.School> GetUserLocalSchools();
    }

    public class SchoolService : SisConnectedService, ISchoolService
    {
        public SchoolService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void Add(Data.School.Model.School school)
        {
            Add(new List<Data.School.Model.School> {school});
        }

        public IList<Data.School.Model.School> GetSchools()
        {
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetAll();
            }
        }

        public IList<Data.School.Model.School> GetSchoolsByIds(IList<int> schoolIds)
        {
            return DoRead(u => new SchoolDataAccess(u).GetByIds(schoolIds));
        }

        public Data.School.Model.School GetSchool(int schoolId)
        {
            if(!(BaseSecurity.IsDistrictAdmin(Context) || Context.SchoolLocalId == schoolId))
                throw new ChalkableSecurityException();
            return DoRead(u => new SchoolDataAccess(u).GetById(schoolId));
        }

        public void Add(IList<Data.School.Model.School> schools)
        {
            var schoolInfos = schools.Select(x => new SchoolInfo
                {
                    LocalId = x.Id,
                    Name = x.Name,
                    IsChalkableEnabled = x.IsChalkableEnabled,
                    IsLEEnabled = x.IsLEEnabled,
                    IsLESyncComplete = x.IsLESyncComplete
                }).ToList();
            ModifySchool(da => da.Insert(schools), (iSchoolS, districtId) => iSchoolS.Add(schoolInfos, districtId));
        }

        public void Edit(IList<Data.School.Model.School> schools)
        {
            var schoolInfos = schools.Select(x => new SchoolInfo
                {
                    LocalId = x.Id,
                    Name = x.Name,
                    IsChalkableEnabled = x.IsChalkableEnabled,
                    IsLEEnabled = x.IsLEEnabled,
                    IsLESyncComplete = x.IsLESyncComplete
                }).ToList();
            ModifySchool(da => da.Update(schools), (iSchoolS, districtId) => iSchoolS.Edit(schoolInfos, districtId));
        }

        public void Delete(IList<Data.School.Model.School> schools)
        {
            var ids = schools.Select(x => x.Id).ToList();
            ModifySchool(da => da.Delete(schools), (schoolS, districtId) => schoolS.Delete(ids, districtId));
        }

        private void ModifySchool(Action<SchoolDataAccess> modifySchool, Action<Master.ISchoolService, Guid> modifyMasterSchool)
        {
            if (Context.Role.Id != CoreRoles.SUPER_ADMIN_ROLE.Id)
                throw new ChalkableSecurityException();
            if (!Context.DistrictId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                var da = new SchoolDataAccess(uow);
                modifySchool(da);
                uow.Commit();
            }
            modifyMasterSchool(ServiceLocator.ServiceLocatorMaster.SchoolService, Context.DistrictId.Value);
        }

        public void AddSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u=>new DataAccessBase<SchoolOption>(u).Insert(schoolOptions));
        }
        public void EditSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<SchoolOption>(u).Update(schoolOptions));
        }
        public void DeleteSchoolOptions(IList<SchoolOption> schoolOptions)
        {
            BaseSecurity.EnsureSysAdmin(Context);
            DoUpdate(u => new DataAccessBase<SchoolOption>(u).Delete(schoolOptions));
        }

        public SchoolOption GetSchoolOption()
        {
            if(!Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException();
            return DoRead(u => new DataAccessBase<SchoolOption, int>(u).GetByIdOrNull(Context.SchoolLocalId.Value));
        }

        public StartupData GetStartupData()
        {
            Trace.Assert(Context.SchoolYearId.HasValue);
            Trace.Assert(Context.PersonId.HasValue);
            var res = DoRead(uow =>
                        new SchoolDataAccess(uow).GetStartupData(Context.SchoolYearId.Value, Context.PersonId.Value,
                            Context.RoleId, Context.NowSchoolYearTime.Date));

           

            //TODO: add this to storage procedure
            res.GradingPeriods = ServiceLocator.GradingPeriodService.GetGradingPeriodsDetails(Context.SchoolYearId.Value);
            return res;
        }

        public IList<SchoolSummaryInfo> GetShortSchoolSummariesInfo(int? start, int? count, string filter, SchoolSortType? sortType)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            try
            {
                var iNowRes = ConnectorLocator.ClassesDashboardConnector.GetSchoolsSummaries(Context.NowSchoolTime);
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    filter = filter.ToLower();
                    iNowRes = iNowRes.Where(x => x.SchoolName.ToLower().Contains(filter)).ToList();
                }

                iNowRes = SortSchools(iNowRes, sortType ?? SchoolSortType.SchoolAsc);
                return iNowRes.Skip(start.Value).Take(count.Value).Select(SchoolSummaryInfo.Create).ToList();
            }
            catch (ChalkableSisNotSupportVersionException)
            {
                var chalkableRes = DoRead(u => new SchoolDataAccess(u).GetShortSchoolSummaries(start.Value, count.Value, filter, (int?) sortType));
                return chalkableRes.Select(SchoolSummaryInfo.Create).ToList();
            }
        }

        private IList<SchoolSummary> SortSchools(IList<SchoolSummary> schools, SchoolSortType sortType)
        {
            switch (sortType)
            {
                case SchoolSortType.SchoolAsc:
                    return schools.OrderBy(x => x.SchoolName).ToList();
                case SchoolSortType.SchoolDesc:
                    return schools.OrderByDescending(x => x.SchoolName).ToList();
                case SchoolSortType.StudentsAsc:
                    return schools.OrderBy(x => x.EnrollmentCount).ToList();
                case SchoolSortType.StudentsDesc:
                    return schools.OrderByDescending(x => x.EnrollmentCount).ToList();
                case SchoolSortType.AttendanceAsc:
                    return schools.OrderBy(x => x.AbsenceCount).ToList();
                case SchoolSortType.AttendanceDesc:
                    return schools.OrderByDescending(x => x.AbsenceCount).ToList();
                case SchoolSortType.DisciplineAsc:
                    return schools.OrderBy(x => x.DisciplineCount).ToList();
                case SchoolSortType.DisciplineDesc:
                    return schools.OrderByDescending(x => x.DisciplineCount).ToList();
                case SchoolSortType.GradesAsc:
                    return schools.OrderBy(x => x.Average).ToList();
                case SchoolSortType.GradesDesc:
                    return schools.OrderByDescending(x => x.Average).ToList();
                default:
                    return schools;
            }
        }

        public int GetSchoolsCount(string filter = null)
        {
            return DoRead(u => new SchoolDataAccess(u).GetShoolsCount(filter));
        }

        public IList<Data.School.Model.School> GetUserLocalSchools()
        {
            var schoolYears = ServiceLocator.SchoolYearService.GetSchoolYears().GroupBy(x => x.SchoolRef).Select(x => x.First());
            return ServiceLocator.SchoolService.GetSchoolsByIds(schoolYears.Select(x => x.SchoolRef).ToList());
        }
    }
}