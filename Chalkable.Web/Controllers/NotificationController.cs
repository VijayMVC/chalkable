﻿using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;
using Chalkable.Data.Common.Enums;
using Chalkable.Common.Exceptions;

namespace Chalkable.Web.Controllers
{
    /**
     * Need to turn off TraceControllerFilter, in order to enable
     * html in params. It fires param validation before mvc sees
     * that property of post model has AllowHtmlAttribute.
    */
    [RequireHttps /*, TraceControllerFilter*/]
    public class NotificationController : ChalkableController
    {
        [AuthorizationFilter("Super Admin,DistrictAdmin, Teacher, Student")]
        public ActionResult List(int? start, int? count)
        {
            var listN = SchoolLocator.NotificationService.GetNotifications(start ?? 0, count ?? 10);
            return Json(listN.Transform(NotificationViewData.Create));
        }

        [AuthorizationFilter("Super Admin,DistrictAdmin, Teacher, Student")]
        public ActionResult MarkAsShown(int id)
        {
            SchoolLocator.NotificationService.MarkAsShown(new[] { id });
            return Json(true);
        }


        [AuthorizationFilter("Super Admin,DistrictAdmin, Teacher, Student")]
        public ActionResult ListByDays(int? start, int? count)
        {
            var dayCount = count ?? 7;
            var str = start ?? 0;
            var personNotifications = SchoolLocator.NotificationService.GetNotifications(0, int.MaxValue);
            var groupedNotifications = personNotifications.GroupBy(x => x.Created.Date).ToList();
            var dictionary = groupedNotifications.Skip(str).Take(dayCount).ToDictionary(x => x.Key, t => t.ToList());
            var resCount = groupedNotifications.Count;
            return Json(new PaginatedList<NotificationsByDateViewData>(NotificationsByDateViewData.Create(dictionary), str / dayCount, dayCount, resCount));
        }

        [AuthorizationFilter("Super Admin,DistrictAdmin, Teacher, Student"), IgnoreTimeOut]
        public ActionResult GetUnShownCount()
        {
            return Json(SchoolLocator.NotificationService.GetUnshownNotificationsCount());
        }

        [AuthorizationFilter("Super Admin,DistrictAdmin, Teacher, Student")]
        public ActionResult MarkAllAsShown()
        {
            var notifications = SchoolLocator.NotificationService.GetUnshownNotifications();
            SchoolLocator.NotificationService.MarkAsShown(notifications.Select(x => x.Id).ToArray());
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User })]
        [ValidateInput(false)]
        public ActionResult ApplicationNotification(AppNotificationInput inputData)
        {
            System.Diagnostics.Trace.Assert(Context.PersonId.HasValue);

            if (string.IsNullOrWhiteSpace(SchoolLocator.Context.OAuthApplication))
                throw new ChalkableException("Only Application can post notifications");

            var app = MasterLocator.ApplicationService.GetApplicationByUrl(SchoolLocator.Context.OAuthApplication);
            SchoolLocator.NotificationService.AddApplicationNotification(Context.PersonId.Value, app.Id, app.Name, inputData.HtmlText);

            return Json(true);
        }
    }
}