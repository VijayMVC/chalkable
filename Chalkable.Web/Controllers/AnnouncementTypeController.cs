﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    public class AnnouncementTypeController : ChalkableController
    {

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", true, new[] {AppPermissionType.Announcement})]
        public ActionResult List(int classId)
        {
            var list = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId);
            var res = ClassAnnouncementTypeViewData.Create(list);
            return Json(res, 3);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ListByClasses(IntList classIds)
        {
            var list = SchoolLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classIds);
            return Json(ClassAnnouncementTypeViewData.Create(list), 3);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult ListByClass(int classId)
        {
            if (!SchoolLocator.Context.PersonId.HasValue)
                throw new UnassignedUserException();
            var res = GetTypesByClass(SchoolLocator, classId);
            return Json(ClassAnnouncementTypeViewData.Create(res), 3);
        }

        public static IList<ClassAnnouncementType> GetTypesByClass(IServiceLocatorSchool serviceLocator, int classId)
        {
            var classAnnTypes = serviceLocator.ClassAnnouncementTypeService.GetClassAnnouncementTypes(classId, false).ToList();
            return classAnnTypes;
        }
    }
}