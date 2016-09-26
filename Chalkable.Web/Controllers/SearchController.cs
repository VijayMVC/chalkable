using System.Collections.Generic;
using System.Linq;
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
        public ActionResult Search(string query, IntList includedSearchType)
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
                Students = new List<StudentSchoolsInfo>(),
                Staffs = new List<Staff>()
            };

            if (includedSearchType != null)
            {
                if (includedSearchType.Any(x => x == (int) SearchTypeEnum.Persons))
                {
                    personList.Students = SchoolLocator.StudentService.SearchStudents(Context.SchoolYearId.Value, null, null, null, null, teacherId, studentId, query, true, 0, 15, null, true);
                    personList.Staffs = SchoolLocator.StaffService.SearchStaff(Context.SchoolYearId.Value, null, studentId, query, true, 0, 15);
                }

                if (includedSearchType.Any(x => x == (int) SearchTypeEnum.Student))
                    personList.Students = SchoolLocator.StudentService.SearchStudents(Context.SchoolYearId.Value, null, null, null, null, teacherId, studentId, query, true, 0, 15, null, true);

                if (includedSearchType.Any(x => x == (int) SearchTypeEnum.Student))
                    personList.Staffs = SchoolLocator.StaffService.SearchStaff(Context.SchoolYearId.Value, null, studentId, query, true, 0, 15);

                if (
                    includedSearchType.Any( x => x == (int) SearchTypeEnum.Persons || x == (int) SearchTypeEnum.Student || x == (int) SearchTypeEnum.Staff))
                    searchRes.Add(SearchTypeEnum.Persons, personList);

                if (includedSearchType.Any(x => x == (int) SearchTypeEnum.Announcements))
                    searchRes.Add(SearchTypeEnum.Announcements, SchoolLocator.AnnouncementFetchService.GetAnnouncementsByFilter(query));

                if (includedSearchType.Any(x => x == (int) SearchTypeEnum.Announcements))
                {
                    var attachments = SchoolLocator.AnnouncementAttachmentService.GetAnnouncementAttachments(query);
                    searchRes.Add(SearchTypeEnum.Attachments, SchoolLocator.AnnouncementAttachmentService.TransformToAttachmentsInfo(attachments, null));
                }

                if (includedSearchType.Any(x => x == (int) SearchTypeEnum.Announcements))
                    searchRes.Add(SearchTypeEnum.Classes, SchoolLocator.ClassService.SearchClasses(query));

                if (includedSearchType.Any(x => x == (int) SearchTypeEnum.Group) && Context.Role == CoreRoles.DISTRICT_ADMIN_ROLE)
                    searchRes.Add(SearchTypeEnum.Group, SchoolLocator.GroupService.GetGroups(Context.PersonId.Value, query));
                
            }
            else
            {
                personList.Students = SchoolLocator.StudentService.SearchStudents(Context.SchoolYearId.Value, null, null, null, null, teacherId, studentId, query, true, 0, 15, null, true);
                personList.Staffs = SchoolLocator.StaffService.SearchStaff(Context.SchoolYearId.Value, null, studentId, query, true, 0, 15);
                searchRes.Add(SearchTypeEnum.Persons, personList);
                searchRes.Add(SearchTypeEnum.Announcements, SchoolLocator.AnnouncementFetchService.GetAnnouncementsByFilter(query));
                var attachments = SchoolLocator.AnnouncementAttachmentService.GetAnnouncementAttachments(query);
                searchRes.Add(SearchTypeEnum.Attachments, SchoolLocator.AnnouncementAttachmentService.TransformToAttachmentsInfo(attachments, null));
                searchRes.Add(SearchTypeEnum.Classes, SchoolLocator.ClassService.SearchClasses(query));
                if (Context.Role == CoreRoles.DISTRICT_ADMIN_ROLE)
                    searchRes.Add(SearchTypeEnum.Group, SchoolLocator.GroupService.GetGroups(Context.PersonId.Value, query));
            }
           
            return Json(SearchViewData.Create(searchRes));
        }


        public class PersonList
        {
            public IList<StudentSchoolsInfo> Students { get; set; }
            public IList<Staff> Staffs { get; set; }
        }
    }
}