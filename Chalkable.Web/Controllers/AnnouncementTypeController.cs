using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult ListByClass(int classId)
        {
            if (!SchoolLocator.Context.SchoolId.HasValue)
                throw new UnassignedUserException();
            var markingPeriod = SchoolLocator.MarkingPeriodService.GetLastMarkingPeriod();
            if (markingPeriod.EndDate < SchoolLocator.Context.NowSchoolTime.Date)
                markingPeriod = SchoolLocator.MarkingPeriodService.GetNextMarkingPeriodInYear(markingPeriod.Id);
            if (markingPeriod == null)
                return Json(new NoMarkingPeriodException());

            var standard = new[] {SchoolLocator.AnnouncementTypeService.GetAnnouncementTypeById((int) SystemAnnouncementType.Standard)};
            var res = GetTypesByClass(SchoolLocator, classId, standard);
            return Json(ClassAnnouncementTypeViewData.Create(res), 3);
        }

        public static IList<ClassAnnouncementType> GetTypesByClass(IServiceLocatorSchool serviceLocator, int classId, IEnumerable<AnnouncementType> defaults)
        {
            var classAnnTypes = serviceLocator.AnnouncementTypeService.GetClassAnnouncementTypes(classId, false).ToList();
            classAnnTypes.AddRange(defaults.Select(x=> new ClassAnnouncementType
                {
                    AnnouncementTypeRef = x.Id,
                    ClassRef = classId,
                    Description = x.Description,
                    Gradable = x.Gradable,
                    Name = x.Name,
                    Percentage = x.Percentage
                }));
            return classAnnTypes;
        }
    }
}