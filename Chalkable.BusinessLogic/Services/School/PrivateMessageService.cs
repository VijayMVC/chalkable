using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPrivateMessageService
    {
        void SendMessageToClass(int classId, string subject, string body);
        void SendMessageToPerson(int personId, string subject, string body);
        PaginatedList<PrivateMessage> GetMessages(int start, int count, bool? read, PrivateMessageType type, string role, string keyword, bool? classOnly, int? acadYear);
        void MarkAsRead(IList<int> ids, bool read);
        void Delete(IList<int> id, PrivateMessageType type);
        IncomePrivateMessage GetIncomeMessage(int messageId);
        SentPrivateMessage GetSentMessage(int messageId);
        PossibleMessageRecipients GetPossibleMessageRecipients(string filter);
        bool CanSendMessageToClass(int classId);
        bool CanSendMessageToPerson(int personId);
    }

    public enum PrivateMessageType
    {
        Income,
        Sent
    }

    //todo: needs tests

    public class PrivateMessageService : SchoolServiceBase, IPrivateMessageService
    {
        public PrivateMessageService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public void SendMessageToClass(int classId, string subject, string body)
        {
            if(!CanSendMessageToClass(classId))
                throw new ChalkableSecurityException("Current user have no right for sending message to class");

            using (var uow = Update())
            {
                var studentsIds = new ClassPersonDataAccess(uow).GetClassPersons(classId).Select(cp=>cp.PersonRef).Distinct().ToList();
                if(studentsIds.Count == 0)
                    throw new ChalkableException("Invalid classId param. Selected class has no students.");
                var messageId = CreatePrivateMessage(studentsIds, classId, subject, body, uow);
                uow.Commit();
                ServiceLocator.NotificationService.AddPrivateMessageNotification(messageId);
            }
            
        }

        public void SendMessageToPerson(int personId, string subject, string body)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            using (var uow = Update())
            {
                if (!CanSendMessageToPerson(personId, uow))
                    throw new ChalkableSecurityException("Current user has no rights for sending message to person");

                var messageId = CreatePrivateMessage(new List<int> {personId}, null, subject, body, uow);
                uow.Commit();

                //TODO: notification sending 
                ServiceLocator.NotificationService.AddPrivateMessageNotification(messageId);
            }
        }

        private int CreatePrivateMessage(IList<int> personIds, int? classId, string subject, string body, UnitOfWork uow)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            var da = new PrivateMessageDataAccess(uow);
            var message = new PrivateMessage
            {
                Body = body,
                Subject = subject,
                FromPersonRef = Context.PersonId.Value,
                Sent = Context.NowSchoolTime
            };
            var messageId = da.InsertWithEntityId(message);
            
            var messageRecipients = personIds.Select(pId => new PrivateMessageRecipient
            {
                PrivateMessageRef = messageId,
                RecipientClassRef = classId,
                RecipientRef = pId
            }).ToList();
            new PrivateMessageRecipientDataAccess(uow).Insert(messageRecipients);
            return messageId;
        }
        
        public PaginatedList<PrivateMessage> GetMessages(int start, int count, bool? read, PrivateMessageType type, string role, string keyword, bool? classOnly, int? acadYear)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            DateTime? fromDate = null;
            DateTime? toDate = null;
            if (acadYear.HasValue)
            {
                var schoolYears = ServiceLocator.SchoolYearService.GetSchoolYearsByAcadYear(acadYear.Value);
                if(schoolYears.Count == 0)
                    return new PaginatedList<PrivateMessage>(new List<PrivateMessage>(), start / count, count);

                fromDate = schoolYears.Min(x => x.StartDate);
                toDate = schoolYears.Max(x => x.EndDate);
            }

            PrivateMessageSecurity.EnsureMessgingPermission(Context);
            using (var uow = Read())
            {
                var da = new PrivateMessageDataAccess(uow);
                var roles = string.IsNullOrWhiteSpace(role) ? new List<string>() : role.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).ToList();
                var rolesIds = roles.Select(x => CoreRoles.GetByName(x).Id).ToList();
                switch (type)
                {
                    case PrivateMessageType.Income:
                        var inMsg = da.GetIncomeMessages(Context.PersonId.Value, rolesIds, keyword, read,  start, count, fromDate, toDate);
                        return new PaginatedList<PrivateMessage>(inMsg.Select(x => x), inMsg.PageIndex, inMsg.PageSize, inMsg.TotalCount);
                    case PrivateMessageType.Sent:
                        var sentMsg = da.GetSentMessages(Context.PersonId.Value, rolesIds, keyword, start, count, classOnly, fromDate, toDate);
                        return new PaginatedList<PrivateMessage>(sentMsg.Select(x => x), sentMsg.PageIndex, sentMsg.PageSize, sentMsg.TotalCount);
                    default:
                        throw new ChalkableException(ChlkResources.ERR_PRIVATE_MESSAGE_INVALID_TYPE);
                }    
            }
        }

        public void MarkAsRead(IList<int> ids, bool read)
        {
            Trace.Assert(Context.PersonId.HasValue);
           
            PrivateMessageSecurity.EnsureMessgingPermission(Context);
            if (ids != null)
                using (var uow = Update())
                {
                    var da = new PrivateMessageRecipientDataAccess(uow);
                    var messagesRecipients = da.GetNotDelatedMessageRecpients(ids, Context.PersonId.Value);
                    if(messagesRecipients.Count == 0)
                        throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_MARK_INVALID_RIGHTS);

                    foreach (var messageRecipient in messagesRecipients)
                        messageRecipient.Read = read;
                    
                    da.Update(messagesRecipients);
                    uow.Commit();
                }
        }

        public void Delete(IList<int> ids, PrivateMessageType type)
        {
            Trace.Assert(Context.PersonId.HasValue);
            if (ids == null) return;
            using (var uow = Update())
            {
                if (type == PrivateMessageType.Income)
                    DeleteIncomeMesssages(ids, uow);
                if (type == PrivateMessageType.Sent)
                    DeleteSentMessages(ids, uow);
                uow.Commit();
            }
        }

        private void DeleteIncomeMesssages(IList<int> ids, UnitOfWork uow)
        {
            var da = new PrivateMessageRecipientDataAccess(uow);
            var messagesRecipients = da.GetNotDelatedMessageRecpients(ids, Context.PersonId.Value);
            if (messagesRecipients.Count == 0)
                throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_MARK_INVALID_RIGHTS);

            foreach (var messageRecipient in messagesRecipients)
                messageRecipient.DeletedByRecipient = true;

            da.Update(messagesRecipients);
        }

        private void DeleteSentMessages(IList<int> ids, UnitOfWork uow)
        {
            var da = new PrivateMessageDataAccess(uow);
            var messages = da.GetNotDeleted(ids, Context.PersonId.Value);
            if (messages.Count == 0)
                throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_MARK_INVALID_RIGHTS);

            foreach (var message in messages)
                message.DeletedBySender = true;

            da.Update(messages);
        }

        public SentPrivateMessage GetSentMessage(int messageId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            PrivateMessageSecurity.EnsureMessgingPermission(Context);
            return DoRead(u => new PrivateMessageDataAccess(u).GetSentPrivateMessage(messageId, Context.PersonId.Value));
        }

        public PossibleMessageRecipients GetPossibleMessageRecipients(string filter)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolYearId.HasValue);

            var sl = filter.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string f1 = null, f2 = null, f3 = null;
            if (sl.Length == 0)
                return null;
            if (sl.Length > 0)
                f1 = sl[0];
            if (sl.Length > 1)
                f2 = sl[1];
            if (sl.Length > 2)
                f3 = sl[2];

            if (Context.MessagingDisabled)
                return null;

            return DoRead(u => new PrivateMessageDataAccess(u)
                .GetPossibleMessageRecipients(
                    Context.PersonId.Value,
                    Context.RoleId,
                    Context.SchoolYearId.Value,
                    Context.TeacherStudentMessaginEnabled,
                    Context.TeacherClassMessagingOnly,
                    Context.StudentMessagingEnabled,
                    Context.StudentClassMessagingOnly,
                    f1, f2, f3
                ));
        }

        public IncomePrivateMessage GetIncomeMessage(int messageId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            PrivateMessageSecurity.EnsureMessgingPermission(Context);
            return DoRead(u => new PrivateMessageDataAccess(u).GetIncomePrivateMessage(messageId, Context.PersonId.Value));
        }

        public bool CanSendMessageToClass(int classId)
        {
            return DoRead(u => CanSendMessageToClass(classId, u));
        }

        private bool CanSendMessageToClass(int classId, UnitOfWork uow)
        {
            return PrivateMessageSecurity.CanSendMessage(Context) && 
                   BaseSecurity.IsTeacher(Context) && Context.TeacherStudentMessaginEnabled &&
                   (!Context.TeacherClassMessagingOnly || new ClassTeacherDataAccess(uow).Exists(classId, Context.PersonId));
        }

        public bool CanSendMessageToPerson(int personId)
        {
            using (var uow = Read())
            {
                return CanSendMessageToPerson(personId, uow);
            }
        }

        private bool CanSendMessageToPerson(int personId, UnitOfWork uow)
        {
            if (BaseSecurity.IsTeacher(Context))
                return CanTeacherSendMessage(personId, uow) && PrivateMessageSecurity.CanSendMessage(Context);
            if (Context.Role == CoreRoles.STUDENT_ROLE)
                return CanStudentSendMessage(personId, uow) && PrivateMessageSecurity.CanSendMessage(Context);
            return PrivateMessageSecurity.CanSendMessage(Context);
        }

        private bool CanTeacherSendMessage(int personId, UnitOfWork uow)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            bool canSend = false;
            var toPerson = new PersonDataAccess(uow).GetPersonDetails(personId, Context.SchoolLocalId.Value);

            if (toPerson == null)
                return false;

            if (toPerson.RoleRef == CoreRoles.TEACHER_ROLE.Id)
                return true;

            if (toPerson.RoleRef == CoreRoles.STUDENT_ROLE.Id)
                if (Context.TeacherStudentMessaginEnabled)
                {
                    canSend = true;
                    if (Context.TeacherClassMessagingOnly)
                    {
                        var toPersonClasses = new ClassDataAccess(uow).GetStudentClasses(Context.SchoolYearId.Value,
                            personId, null);
                        var currPersonClasses = new ClassDataAccess(uow).GetTeacherClasses(Context.SchoolYearId.Value,
                            Context.PersonId.Value, null);

                        canSend = currPersonClasses.Any(x => toPersonClasses.Any(y => x.Id == y.Id));
                    }
                }

            return canSend;
        }

        private bool CanStudentSendMessage(int personId, UnitOfWork uow)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            bool canSend = false;
            var toPersonRole = ServiceLocator.PersonService.GetPersonRole(personId);

            var currPersonClasses = new ClassDataAccess(uow).GetStudentClasses(Context.SchoolYearId.Value,
                Context.PersonId.Value, null);

            if (toPersonRole == CoreRoles.TEACHER_ROLE && Context.TeacherStudentMessaginEnabled)
                if (Context.TeacherClassMessagingOnly)
                {
                    var toPersonClasses = new ClassDataAccess(uow).GetTeacherClasses(Context.SchoolYearId.Value,
                            personId, null);

                    canSend = currPersonClasses.Any(x => toPersonClasses.Any(y => x.Id == y.Id));
                }
                else
                    return true;

            if (toPersonRole == CoreRoles.STUDENT_ROLE)
                if (Context.StudentMessagingEnabled)
                {
                    canSend = true;
                    if (Context.StudentClassMessagingOnly)
                    {
                        var toPersonClasses = new ClassDataAccess(uow).GetStudentClasses(Context.SchoolYearId.Value,
                            personId, null);

                        canSend = currPersonClasses.Any(x => toPersonClasses.Any(y => x.Id == y.Id));
                    }
                }

            return canSend;
        }
    }
}
