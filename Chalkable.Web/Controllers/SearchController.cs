using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class SearchController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]  // - System Admin
        public ActionResult Search(string query)
        {
            IDictionary<SearchTypeEnum, object> searchRes = new Dictionary<SearchTypeEnum, object>();
            query = query.ToLower();
            searchRes.Add(SearchTypeEnum.Persons, SchoolLocator.PersonService.SearchPersons(query, true, 0, 30));
            searchRes.Add(SearchTypeEnum.Announcements, SchoolLocator.AnnouncementFetchService.GetAnnouncementsByFilter(query));
            var attachments = SchoolLocator.AnnouncementAttachmentService.GetAnnouncementAttachments(query);
            searchRes.Add(SearchTypeEnum.Attachments, SchoolLocator.AnnouncementAttachmentService.TransformToAttachmentsInfo(attachments, null));
            searchRes.Add(SearchTypeEnum.Classes, SchoolLocator.ClassService.SearchClasses(query));
            return Json(SearchViewData.Create(searchRes));
        }
    }
}