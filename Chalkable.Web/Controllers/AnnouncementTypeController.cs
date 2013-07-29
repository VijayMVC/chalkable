using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    public class AnnouncementTypeController : ChalkableController
    {

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult List()
        {
            var list = SchoolLocator.AnnouncementTypeService.GetAnnouncementTypes(null);
            var res = AnnouncementTypeViewData.Create(list);
            return Json(res, 3);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ListByClass(Guid classId)
        {
            if (!SchoolLocator.Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            var markingPeriod = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            if (markingPeriod.EndDate < SchoolLocator.Context.NowSchoolTime.Date)
                markingPeriod = SchoolLocator.MarkingPeriodService.GetNextMarkingPeriodInYear(markingPeriod.Id);
            if (markingPeriod == null)
                return Json(new NoMarkingPeriodException());
            var res = GetTypesByClasses(SchoolLocator, markingPeriod.Id, classId);
            return Json(AnnouncementTypeViewData.Create(res), 3);
        }

        public static IList<AnnouncementType> GetTypesByClasses(IServiceLocatorSchool serviceLocator, Guid markingPeriodId, Guid classId)
        {
            var finalAnnTypes = serviceLocator.FinalGradeService.GetFinalGradeAnnouncementTypes(markingPeriodId, classId);
            var annTypes = finalAnnTypes.Where(x => x.PercentValue > 0).Select(x => x.AnnouncementType).ToList();
            annTypes.Add(serviceLocator.AnnouncementTypeService.GetAnnouncementTypeById((int)SystemAnnouncementType.Standard));
            foreach (var announcementType in annTypes)
            {
                announcementType.CanCreate = true;
            }
            return annTypes;
        }
    }
}