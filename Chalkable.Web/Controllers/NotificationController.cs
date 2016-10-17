using System.Linq;
using System.Web.Mvc;
using Chalkable.Common;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
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
    }
}