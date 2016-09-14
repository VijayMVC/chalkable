using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.School.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Common;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class SearchController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]  // - System Admin
        public ActionResult Search(string query)
        {
            IDictionary<SearchTypeEnum, object> searchRes = new Dictionary<SearchTypeEnum, object>();

            int? teacherId = null;
            if (Context.Role == CoreRoles.TEACHER_ROLE && !Context.Claims.HasPermission(ClaimInfo.VIEW_STUDENT))
                teacherId = Context.PersonId;

            int? studentId = null;
            if (Context.Role == CoreRoles.STUDENT_ROLE)
                studentId = Context.PersonId;

            query = query.ToLower();
            var personList = new PersonList
            {
                Students = SchoolLocator.StudentService.SearchStudents(Context.SchoolYearId.Value, null, null, null, null, teacherId, studentId, query, true, 0, 15, null, true),
                Staffs = SchoolLocator.StaffService.SearchStaff(Context.SchoolYearId.Value, null, studentId, query, true, 0, 15)
            };
            searchRes.Add(SearchTypeEnum.Persons, personList);
            searchRes.Add(SearchTypeEnum.Announcements, SchoolLocator.AnnouncementFetchService.GetAnnouncementsByFilter(query));
            var attachments = SchoolLocator.AnnouncementAttachmentService.GetAnnouncementAttachments(query);
            searchRes.Add(SearchTypeEnum.Attachments, SchoolLocator.AnnouncementAttachmentService.TransformToAttachmentsInfo(attachments, null));
            searchRes.Add(SearchTypeEnum.Classes, SchoolLocator.ClassService.SearchClasses(query));
            return Json(SearchViewData.Create(searchRes));
        }


        public class PersonList
        {
            public IList<Student> Students { get; set; }
            public IList<Staff> Staffs { get; set; }
        }
    }
}