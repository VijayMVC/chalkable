﻿using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.Master.Model.Chlk;
using Chalkable.StiConnector.Connectors;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [TraceControllerFilter]
    public class DistrictController : ChalkableController
    {
        public ActionResult Register(DistrictRegisterViewData data)
        {
            string name = data.DistrictGuid.ToString();
            string timeZone = data.DistrictTimeZone ?? "UTC";
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var context = sl.UserService.Login(data.UserName, data.Password);
            if (context != null)
            {
                var district = sl.DistrictService.GetByIdOrNull(data.DistrictGuid);
                if (district == null && string.IsNullOrEmpty(data.SisPassword))
                    throw new Exception("SIS password can not be null for a new district");
                var pwd = data.SisPassword;
                if (string.IsNullOrEmpty(pwd))
                    pwd = district.SisPassword;
                var locator = ConnectorLocator.Create(data.SisUserName, pwd, data.ApiUrl);
                if (!locator.LinkConnector.Link(data.LinkKey))
                    throw new Exception("The link keys do not match.");
                if (district == null)
                {
                    sl.DistrictService.Create(data.DistrictGuid, name, data.ApiUrl, data.RedirectUrl, data.SisUserName, data.SisPassword, timeZone, data.DistrictState);
                }
                else
                {
                    if (String.Compare(district.SisUrl, data.ApiUrl, StringComparison.OrdinalIgnoreCase) != 0 )
                        throw new Exception("API url can't be changed for existing district");
                    district.Name = name;
                    if (!string.IsNullOrEmpty(data.SisPassword))
                        district.SisPassword = data.SisPassword;
                    district.SisUrl = data.ApiUrl;
                    district.SisUserName = data.SisUserName;
                    district.TimeZone = timeZone;
                    district.SisRedirectUrl = data.RedirectUrl;
                    district.StateCode = data.DistrictState;
                    sl.DistrictService.Update(district);
                }
                return Json(true);
            }
            return new HttpUnauthorizedResult();
        }

        [RequireHttps]
        [AuthorizationFilter("SysAdmin")]
        public ActionResult List(int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            var districts = MasterLocator.DistrictService.GetDistrictsSyncStatus(start.Value, count.Value);
            return Json(districts.Transform(DistrictSyncStatusViewData.Create));
        }

        public ActionResult ListTimeZones()
        {
            var tzCollection = DateTimeTools.GetAll();
            return Json(tzCollection);
        }
        
        [RequireHttps]
        [AuthorizationFilter("SysAdmin")]
        public ActionResult Sync(Guid districtId)
        {
            MasterLocator.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.SisDataImport, DateTime.UtcNow, districtId, string.Empty, districtId.ToString());
            return Json(true);
        }

        [RequireHttps]
        public ActionResult Info(Guid districtId)
        {
            var result = MasterLocator.DistrictService.GetByIdOrNull(districtId);
            return Json(DistrictViewData.Create(result));
        }
    }
}
