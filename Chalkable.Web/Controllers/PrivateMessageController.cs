using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.Common.Enums;
using Chalkable.Web.ActionFilters;
using Chalkable.Web.Models;

namespace Chalkable.Web.Controllers
{
    [RequireHttps, TraceControllerFilter]
    public class PrivateMessageController : ChalkableController
    {
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult List(int? start, int? count, bool? read, bool? income, string role, string keyword, bool? classOnly, int? acadYear)
        {
            var messageType = (income ?? true) ? PrivateMessageType.Income : PrivateMessageType.Sent;
            var res = SchoolLocator.PrivateMessageService.GetMessages(start ?? 0, count ?? 10, read, messageType, role, keyword, classOnly, acadYear);
            return Json(res.Transform(PrivateMessageComplexViewData.Create));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Read(int id, bool? income)
        {
            PrivateMessageComplexViewData res;
            if (income.HasValue && !income.Value)
                res = PrivateMessageComplexViewData.Create(SchoolLocator.PrivateMessageService.GetSentMessage(id));
            else
                res = PrivateMessageComplexViewData.Create(SchoolLocator.PrivateMessageService.GetIncomeMessage(id));
            return Json(res);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.Message })]
        public ActionResult Send(int? personId, int? classId, string subject, string body)
        {
            if(classId.HasValue)
                SchoolLocator.PrivateMessageService.SendMessageToClass(classId.Value, subject, body);
            if(personId.HasValue)
                SchoolLocator.PrivateMessageService.SendMessageToPerson(personId.Value, subject, body);

            //TODO : add tracking 
            return Json(true);
            //var res = SchoolLocator.PrivateMessageService.SendMessage(personId.Value, subject, body);
            //if (res != null)
            //{
            //    MasterLocator.UserTrackingService.SentMessageTo(Context.Login, res.Recipient.FullName());
            //}
            //return Json(PrivateMessageViewData.Create(res));
        }
        
        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult MarkAsRead(IntList ids, bool read)
        {
            SchoolLocator.PrivateMessageService.MarkAsRead(ids, read);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult Delete(IntList ids, bool? income)
        {
            var messageType = (income ?? true) ? PrivateMessageType.Income : PrivateMessageType.Sent;
            SchoolLocator.PrivateMessageService.Delete(ids, messageType);
            return Json(true);
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student", true, new[] { AppPermissionType.User, AppPermissionType.Message})]
        public ActionResult ListPossibleRecipients(string filter)
        {
            Trace.Assert(Context.SchoolLocalId.HasValue);

            var possibleRecipients = SchoolLocator.PrivateMessageService.GetPossibleMessageRecipients(filter);
            return Json(PossibleMessageRecipientViewData.Create(possibleRecipients));
        }

        [AuthorizationFilter("DistrictAdmin, Teacher, Student")]
        public ActionResult CanSendMessage(int? personId, int? classId)
        {
            return Json((personId.HasValue && SchoolLocator.PrivateMessageService.CanSendMessageToPerson(personId.Value))
                     || (classId.HasValue && SchoolLocator.PrivateMessageService.CanSendMessageToClass(classId.Value))
                    );
        }
    }
}