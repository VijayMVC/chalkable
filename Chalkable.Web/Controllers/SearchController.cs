using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Logic;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class SearchController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]  // - System Admin
        public ActionResult Search(string query)
        {
            IDictionary<SearchTypeEnum, Object> searchRes = new Dictionary<SearchTypeEnum, Object>();
            query = query.ToLower();
            searchRes.Add(SearchTypeEnum.Persons, SchoolLocator.PersonService.SearchPersons(query, true, 0, 30));
            if (Context.SCEnabled && Context.Role != CoreRoles.DISTRICT_ADMIN_ROLE)
                searchRes.Add(SearchTypeEnum.Applications, MasterLocator.ApplicationService.GetApplications(null, null, query, null, null));
            searchRes.Add(SearchTypeEnum.Announcements, SchoolLocator.AnnouncementFetchService.GetAnnouncementsByFilter(query));
            var attachments = SchoolLocator.AnnouncementAttachmentService.GetAttachments(query);
            searchRes.Add(SearchTypeEnum.Attachments, AttachmentLogic.PrepareAttachmentsInfo(attachments, MasterLocator.CrocodocService));
            searchRes.Add(SearchTypeEnum.Classes, SchoolLocator.ClassService.SearchClasses(query));
            return Json(SearchViewData.Create(searchRes));
        }
    }
}