using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Services;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Authentication;

namespace Chalkable.Web.Controllers
{
    public class HomeController : ChalkableController
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LogOn(string userName, string password, bool remember)
        {
            var serviceLocator = ServiceLocatorFactory.CreateMasterSysAdmin();
            var context = serviceLocator.UserService.Login(userName, password);
            if (context != null)
            {
                ChalkableAuthentication.SignIn(context, remember);
                return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = false, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            var userName = ControllerContext.HttpContext.User.Identity.Name;
            ChalkableAuthentication.SignOut();
            return Json(new { Success = true, UserName = userName }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult SysAdmin()
        {
            return View();
        }


        //TODO: test only. don't forget to remove :)
        public ActionResult Create(string userName, string password)
        {
            ServiceLocatorFactory.CreateMasterSysAdmin().UserService.CreateSysAdmin(userName, password);
            return Json(new { Success = true, UserName = userName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RunSchoolImport(int sisSchoolId, int sisSchoolYearId, string name, string dataBaseName, string dataBaseUrl, string sisUser, string sisPwd)
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var district = sl.SchoolService.GetDistricts().First();

            var school = sl.SchoolService.Create(district.Id, name, new List<UserInfo> ());
            school.ImportSystemType = ImportSystemTypeEnum.Sti;
            sl.SchoolService.Update(school);
            var sync = new SisSync
                {
                    Id = school.Id,
                    SisDatabaseName = dataBaseName,
                    SisDatabaseUrl = dataBaseUrl,
                    SisDatabaseUserName = sisUser,
                    SisDatabasePassword = sisPwd,
                    SisSchoolId = sisSchoolId
                };
            sl.SchoolService.SetSyncData(sync);
            var data = new SisImportTaskData(school.Id, sisSchoolId, new List<int> { sisSchoolYearId });
            sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.SisDataImport, DateTime.UtcNow, school.Id, data.ToString());
            return Json(true);
        }

        public ActionResult Backup()
        {
            var sl = ServiceLocatorFactory.CreateMasterSysAdmin();
            var data = DateTime.UtcNow.Ticks;
            sl.BackgroundTaskService.ScheduleTask(BackgroundTaskTypeEnum.BackupDatabases, DateTime.UtcNow, null, data.ToString(CultureInfo.InvariantCulture));
            return Json(true);
        }
    }
}
