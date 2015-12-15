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

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ISchoolService
    {
        void Add(Data.School.Model.School school);
        void Add(IList<Data.School.Model.School> schools);
        void Edit(IList<Data.School.Model.School> schools);
        void Delete(IList<Data.School.Model.School> schools);
        IList<Data.School.Model.School> GetSchools();
        Data.School.Model.School GetSchool(int schoolId);
        void AddSchoolOptions(IList<SchoolOption> schoolOptions);
        void EditSchoolOptions(IList<SchoolOption> schoolOptions);
        void DeleteSchoolOptions(IList<SchoolOption> schoolOptions);
        SchoolOption GetSchoolOption();
        StartupData GetStartupData();
        PaginatedList<SchoolSummaryInfo> GetShortSchoolSummariesInfo(int? start, int? count, string filter);
        int GetSchoolsCount(string filter = null);
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
            if (!BaseSecurity.IsDistrictAdmin(Context))
                throw new ChalkableSecurityException();
            using (var uow = Read())
            {
                var da = new SchoolDataAccess(uow);
                return da.GetAll();
            }
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

        public PaginatedList<SchoolSummaryInfo> GetShortSchoolSummariesInfo(int? start, int? count, string filter)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;

            var iNowRes = ConnectorLocator.ClassesDashboardConnector.GetSchoolsSummaries(Context.NowSchoolTime, filter);

            if (iNowRes == null)
                return SchoolSummaryInfo.Create(DoRead( u => new SchoolDataAccess(u).GetShortSchoolSummaries(start.Value, count.Value, filter)));

            var allSchoolCount = GetSchoolsCount(filter);

            return SchoolSummaryInfo.Create(iNowRes, start.Value, count.Value, allSchoolCount);
        }

        public int GetSchoolsCount(string filter = null)
        {
            return DoRead(u => new SchoolDataAccess(u).GetShoolsCount(filter));
        }
    }
}