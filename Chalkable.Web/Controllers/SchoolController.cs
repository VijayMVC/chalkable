using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.SisConnector.PublicModel;
using Chalkable.SisImportFacade;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;
using Chalkable.Web.Models.SchoolsViewData;


namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class SchoolController : ChalkableController
    {
        //TODO: add
        [AuthorizationFilter("SysAdmin")]
        public ActionResult List(Guid? districtId, int? start, int? count, bool? demoOnly, bool? unimportedOnly)
        {
            count = count ?? 10;
            start = start ?? 0;
            var schools = MasterLocator.SchoolService.GetSchools(districtId, start.Value, count.Value);
            return Json(schools.Transform(SchoolViewData.Create));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Summary(Guid schoolId)
        {
            if (SchoolLocator.Context.SchoolId != schoolId)
                SchoolLocator = MasterLocator.SchoolServiceLocator(schoolId);
            var school = MasterLocator.SchoolService.GetById(schoolId);
            var sisData = MasterLocator.SchoolService.GetSyncData(school.Id);
            return Json(SchoolInfoViewData.Create(school, sisData));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult People(Guid schoolId, int? roolId, Guid? gradeLevelId, int? start, int? count)
        {
            var school = MasterLocator.SchoolService.GetById(schoolId);
            if(SchoolLocator.Context.SchoolId != schoolId)
                 SchoolLocator = MasterLocator.SchoolServiceLocator(school.Id);
            var persons = SchoolLocator.PersonService.GetPersons();
            var studentsCount = persons.Count(x => x.RoleRef == CoreRoles.STUDENT_ROLE.Id);   
            var teachersCount = persons.Count(x => x.RoleRef == CoreRoles.TEACHER_ROLE.Id);
            var adminsCount = persons.Count(x => x.RoleRef == CoreRoles.ADMIN_EDIT_ROLE.Id)
                              + persons.Count(x => x.RoleRef == CoreRoles.ADMIN_GRADE_ROLE.Id)
                              + persons.Count(x => x.RoleRef == CoreRoles.ADMIN_VIEW_ROLE.Id);
            var sisData = MasterLocator.SchoolService.GetSyncData(schoolId);
            var resView = SchoolPeopleViewData.Create(school, sisData, studentsCount, teachersCount, adminsCount);
            return Json(resView);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult GetPersons(Guid schoolId, string roleName, GuidList gradeLevelIds, int? start, int? count, int? sortType)
        {
            if (SchoolLocator.Context.SchoolId != schoolId)
                SchoolLocator = MasterLocator.SchoolServiceLocator(schoolId);
            return Json(PersonLogic.GetPersons(SchoolLocator, start, count, sortType, null, roleName, null, gradeLevelIds));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult SisSyncInfo(Guid schoolId)
        {
            var syncInfo = MasterLocator.SchoolService.GetSyncData(schoolId);
            return Json(SisSyncViewData.Create(syncInfo));
        }

        public ActionResult ListTimeZones()
        {
            var tzCollection = DateTimeTools.GetAll();
            return Json(tzCollection);
        }

        public ActionResult GetSchoolsForImport(Guid districtId)
        {
            var distr = MasterLocator.DistrictService.GetById(districtId);
            var existing = MasterLocator.SchoolService.GetSchools(districtId);
            var connectionInfo = new SisConnectionInfo
                {
                    DbName = distr.DbName,
                    SisPassword = distr.SisPassword,
                    SisUrl = distr.SisUrl,
                    SisUserName = distr.SisUserName
                };
            var importService = SisImportProvider.CreateImportService(distr.SisSystemType, Guid.Empty, 0, null, connectionInfo, null);
            var res = importService.GetSchools();
            res = res.Where(x => !existing.Any(y => y.Name == x.Name)).ToList();
            return Json(res);
        }

        public ActionResult RunSchoolImport(Guid districtId, int sisSchoolId, int sisSchoolYearId, string name)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var district = sl.DistrictService.GetById(districtId);
            var school = sl.SchoolService.Create(district.Id, name, new List<UserInfo>());
            school.ImportSystemType = ImportSystemTypeEnum.Sti;
            sl.SchoolService.Update(school);
            var sync = new SisSync
            {
                Id = school.Id,
                SisSchoolId = sisSchoolId
            };
            sl.SchoolService.SetSyncData(sync);
            var data = new SisImportTaskData(school.Id, sisSchoolId, new List<int> { sisSchoolYearId });
            sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.SisDataImport, DateTime.UtcNow, school.Id, data.ToString());
            return Json(true);
        }
    }
}
