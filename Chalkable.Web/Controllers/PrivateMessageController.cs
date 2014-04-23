using System;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Data.Master.Model;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public partial class PrivateMessageController : ChalkableController
    {
        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult List(int? start, int? count, bool? read, bool? income, string role, string keyword)
        {
            var messageType = (income ?? true) ? PrivateMessageType.Income : PrivateMessageType.Outcome;
            var res = SchoolLocator.PrivateMessageService.GetMessages(start ?? 0, count ?? 10, read, messageType, role, keyword);
            return Json(res.Transform(PrivateMessageViewData.Create));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Read(int id)
        {
            var res = SchoolLocator.PrivateMessageService.GetMessage(id);
            return Json(PrivateMessageViewData.Create(res));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_PRIVATE_MESSAGES_SEND, true, CallType.Get, new[] { AppPermissionType.Message })]
        public ActionResult Send(int personId, string subject, string body)
        {
            var res = SchoolLocator.PrivateMessageService.SendMessage(personId, subject, body);
            //if (res != null)
            //{
            //    MixPanelService.SentMessageTo(ServiceLocator.Context.UserName, res.Recipient.DisplayName);
            //}
            return Json(PrivateMessageViewData.Create(res));
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult MarkAsRead(IntList ids, bool read)
        {
            SchoolLocator.PrivateMessageService.MarkAsRead(ids, read);
            return Json(true);
        }

        [AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student")]
        public ActionResult Delete(IntList ids)
        {
            SchoolLocator.PrivateMessageService.Delete(ids);
            return Json(true);
        }

        //[AuthorizationFilter("AdminGrade, AdminEdit, AdminView, Teacher, Student", Preference.API_DESCR_PRIVATE_MESSAGES_LIST_POSSIBLE_RECIPIENTS, true, CallType.Get, new[] { AppPermissionType.User })]
        //public ActionResult ListPossibleRecipients(string query)
        //{
        //    if (!SchoolLocator.Context.SchoolId.HasValue)
        //        throw new UnassignedUserException();
        //    var students = SchoolLocator.PersonService.GetPersonsByFilter(ServiceLocator.Context.SchoolId.Value, query, null);
        //    return Json(students.Select(x => SchoolPersonViewData.Create(x, false)));
        //}
    }
}