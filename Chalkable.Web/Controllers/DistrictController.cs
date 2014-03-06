using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DistrictController : ChalkableController
    {
        public ActionResult Register(string userName, string password, Guid guid, string sisUrl, string sisUserName, string sisPassword, string timeZone)
        {
            string name = guid.ToString();
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var context = sl.UserService.Login(userName, password);
            if (context != null)
            {
                sl.DistrictService.Create(name, sisUrl, sisUserName, sisPassword, timeZone ?? "UTC", guid);
                return Json(true);
            }
            return Json("Invalid credentials");
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult List(int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
            var districts = MasterLocator.DistrictService.GetDistricts(start.Value, count.Value);
            return Json(districts.Transform(DistrictViewData.Create));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Create(string name, Guid guid, string sisUrl, string sisUserName, string sisPassword, string timeZone)
        {
            var district = MasterLocator.DistrictService.Create(name, sisUrl, sisUserName, sisPassword, timeZone ?? "UTC", guid);
            return Json(DistrictViewData.Create(district));
        }

        public ActionResult ListTimeZones()
        {
            var tzCollection = DateTimeTools.GetAll();
            return Json(tzCollection);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Delete(Guid districtId)
        {
            MasterLocator.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.DeleteDistrict, DateTime.UtcNow, districtId, districtId.ToString());
            return Json(true);
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Update(Guid districtId, string name, string dbName, string sisUrl, string sisUserName, string sisPassword, int sisSystemType)
        {
            MasterLocator.DistrictService.Update(
                new District
                {
                    Id = districtId,
                    Name = name,
                    DbName = dbName,
                    SisUrl = sisUrl,
                    SisUserName = sisUserName,
                    SisPassword =  sisPassword
                });
            return Json(true);
        }

        public ActionResult Info(Guid districtId)
        {
            var result = MasterLocator.DistrictService.GetByIdOrNull(districtId);
            return Json(DistrictViewData.Create(result));
        }
    }
}
