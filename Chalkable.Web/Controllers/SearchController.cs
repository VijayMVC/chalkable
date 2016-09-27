using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
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
            Trace.Assert(Context.PersonId.HasValue);

            IDictionary<SearchTypeEnum, Func<string, object>> searchActionRegister = new Dictionary <SearchTypeEnum, Func<string, object>>
            {
                {SearchTypeEnum.Student, SearchStudents},
                {SearchTypeEnum.Staff, SearchStaffs},
                {SearchTypeEnum.Announcements, SchoolLocator.AnnouncementFetchService.GetAnnouncementsByFilter},
                {SearchTypeEnum.Attachments, SearchAttachments},
                {SearchTypeEnum.Classes, SchoolLocator.ClassService.SearchClasses}
            };
            if(Context.Role == CoreRoles.DISTRICT_ADMIN_ROLE)
                searchActionRegister.Add(SearchTypeEnum.Group, q => SchoolLocator.GroupService.GetGroups(Context.PersonId.Value, q));

            IDictionary<SearchTypeEnum, object> searchRes = new Dictionary<SearchTypeEnum, object>();
            query = query.ToLower();
            foreach (var keyAction in searchActionRegister.Where(keyAction => IsTypeIncluded(includedSearchType, keyAction.Key)))
            {
                searchRes.Add(keyAction.Key, keyAction.Value(query));
            }
            return Json(SearchViewData.Create(searchRes));
        }

        private static bool IsTypeIncluded(IList<int> includedSearchTypes, SearchTypeEnum type)
        {
            return includedSearchTypes == null || includedSearchTypes.Count == 0 || includedSearchTypes.Contains((int) type);
        }

        private PaginatedList<StudentSchoolsInfo> SearchStudents(string query)
        {
            var teacherId = BaseSecurity.IsTeacher(Context) && !Context.Claims.HasPermission(ClaimInfo.VIEW_STUDENT) ? Context.PersonId : null;
            var studentId = Context.Role == CoreRoles.STUDENT_ROLE ? Context.PersonId : null;
            return SchoolLocator.StudentService.SearchStudents(Context.SchoolYearId.Value, null, null, null, null, teacherId, studentId, query, true, 0, 15, null, true);
        }
        private PaginatedList<Staff> SearchStaffs(string query)
        {
            var studentId = Context.Role == CoreRoles.STUDENT_ROLE ? Context.PersonId : null;
            return SchoolLocator.StaffService.SearchStaff(Context.SchoolYearId.Value, null, studentId, query, true, 0, 15);
        } 
        private IList<AnnouncementAttachmentInfo> SearchAttachments(string query)
        {
            var attachments = SchoolLocator.AnnouncementAttachmentService.GetAnnouncementAttachments(query);
            return SchoolLocator.AnnouncementAttachmentService.TransformToAttachmentsInfo(attachments, null);
        } 
    }
}