using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.StiConnector.Connectors;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DistrictController : ChalkableController
    {
        public ActionResult Register(DistrictRegisterViewData data)
        {
            //http://sandbox.sti-k12.com/chalkable/api/chalkale/linkstatus?LinkKey=[LinkKeyGuid]
            string name = data.DistrictGuid.ToString();
            string timeZone = data.DistrictTimeZone ?? "UTC";
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var context = sl.UserService.Login(data.UserName, data.Password);
            if (context != null)
            {
                var district = MasterLocator.DistrictService.GetByIdOrNull(data.DistrictGuid);
                if (district == null && string.IsNullOrEmpty(data.SisPassword))
                    throw new Exception("SIS password can not be null for a new district");
                var pwd = data.SisPassword;
                if (string.IsNullOrEmpty(pwd))
                    pwd = district.SisPassword;
                var locator = ConnectorLocator.Create(data.SisUserName, pwd, data.ApiUrl);
                if (!locator.LinkConnector.Link(data.LinkKey))
                    throw new Exception("Link was not successful");
                if (district == null)
                {
                    sl.DistrictService.Create(data.DistrictGuid, name, data.ApiUrl, data.SisUserName, data.SisPassword, timeZone);
                }
                else
                {
                    district.Name = name;
                    if (!string.IsNullOrEmpty(data.Password))
                        district.SisPassword = data.Password;
                    district.SisUrl = data.ApiUrl;
                    district.SisUserName = data.SisUserName;
                    district.TimeZone = timeZone;
                    MasterLocator.DistrictService.Update(district);
                }


                
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
        public ActionResult Create()
        {
            throw new NotSupportedException();
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
