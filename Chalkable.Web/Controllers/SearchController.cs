using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class SearchController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_SEARCH_SEARCH, true, CallType.Get, new[] { AppPermissionType.User })]  // - System Admin
        public ActionResult Search(string query)
        {
            IDictionary<SearchTypeEnum, Object> searchRes = new Dictionary<SearchTypeEnum, Object>();
            query = query.ToLower();
            searchRes.Add(SearchTypeEnum.Persons, SchoolLocator.PersonService.SearchPersons(query, true, 0, 30));
            if (Context.SCEnabled)
                searchRes.Add(SearchTypeEnum.Applications, MasterLocator.ApplicationService.GetApplications(null, null, query, null, null));
            searchRes.Add(SearchTypeEnum.Announcements, SchoolLocator.AnnouncementService.GetAnnouncements(query));
            var attachments = SchoolLocator.AnnouncementAttachmentService.GetAttachments(query);
            searchRes.Add(SearchTypeEnum.Attachments, AttachmentLogic.PrepareAttachmentsInfo(attachments, MasterLocator.CrocodocService));
            searchRes.Add(SearchTypeEnum.Classes, SchoolLocator.ClassService.SearchClasses(query));
            return Json(SearchViewData.Create(searchRes));
        }
    }
}