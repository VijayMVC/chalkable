﻿using System;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class DistrictController : ChalkableController
    {
        [AuthorizationFilter("SysAdmin")]
        public ActionResult List(int? start, int? count)
        {
            start = start ?? 0;
            count = count ?? 10;
            var districts = MasterLocator.DistrictService.GetDistricts(start.Value, count.Value);
            return Json(districts.Transform(DistrictViewData.Create));
        }


        [AuthorizationFilter("SysAdmin")]
        public ActionResult Create(string name, string dbName, string sisUrl, string sisUserName, string sisPassword, int sisSystemType)
        {
            var district = MasterLocator.DistrictService.Create(name, dbName, sisUrl, sisUserName, sisPassword, (ImportSystemTypeEnum)sisSystemType);
            return Json(DistrictViewData.Create(district));
        }

        [AuthorizationFilter("SysAdmin")]
        public ActionResult Delete(Guid districtId)
        {
            MasterLocator.DistrictService.Delete(districtId);
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
                    SisPassword =  sisPassword,
                    SisSystemType = sisSystemType
                });
            return Json(true);
        }

        public ActionResult Info(Guid districtId)
        {
            var result = MasterLocator.DistrictService.GetById(districtId);
            return Json(DistrictViewData.Create(result));
        }

    }
}
