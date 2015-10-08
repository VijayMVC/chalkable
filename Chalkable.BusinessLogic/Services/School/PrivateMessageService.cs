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
        PaginatedList<PrivateMessage> GetMessages(int start, int count, bool? read, PrivateMessageType type, string role, string keyword);
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
            if(!BaseSecurity.IsTeacher(Context))
                throw new ChalkableSecurityException("Only teacher can send message to class");

            using (var uow = Update())
            {
                PrivateMessageSecurity.EnsureMessgingPermission(Context);
                if (!Context.TeacherStudentMessaginEnabled)
                    throw new ChalkableSecurityException("Teacher has no rights for sending messages to students");

                if (Context.TeacherClassMessagingOnly && !new ClassTeacherDataAccess(uow).Exists(classId, Context.PersonId))
                    throw new ChalkableSecurityException("Teacher has no right for sending messages to current class. He can send message only to his own classes");

                var studentsIds = new ClassPersonDataAccess(uow).GetClassPersons(classId).Select(cp=>cp.PersonRef).Distinct().ToList();
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
                var toPerson = new PersonDataAccess(uow).GetPersonDetails(personId, Context.SchoolLocalId.Value);
                if(BaseSecurity.IsTeacher(Context))
                    EnsureTeacherMessagingPermission(toPerson, uow);
                if(Context.Role == CoreRoles.STUDENT_ROLE)
                    EnsureStudentMessagingPermission(toPerson, uow);

                var messageId = CreatePrivateMessage(new List<int> {personId}, null, subject, body, uow);
                uow.Commit();

                //TODO: notification sending 
                ServiceLocator.NotificationService.AddPrivateMessageNotification(messageId);
            }
        }

        private void EnsureTeacherMessagingPermission(Person toPerson, UnitOfWork uow)
        {
            if (!Context.TeacherStudentMessaginEnabled)
                throw new ChalkableSecurityException("Current teacher has no right for sending message to student");
            if (Context.TeacherClassMessagingOnly)
            {
                var currentPersonClasses = new ClassTeacherDataAccess(uow).GetClassTeachers(null, Context.PersonId);
                var toPersonClasses = new ClassPersonDataAccess(uow).GetClassPersons(new ClassPersonQuery { PersonId = toPerson.Id });
                if (!currentPersonClasses.Any(x => toPersonClasses.Any(y => x.ClassRef == y.ClassRef)))
                    throw new ChalkableSecurityException("Teacher can send message only to his students");
            }
        }

        private void EnsureStudentMessagingPermission(Person toPerson, UnitOfWork uow)
        {
            
            if (toPerson.RoleRef == Context.RoleId)
            {
                if (!Context.StudentMessagingEnabled)
                    throw new ChalkableSecurityException("Current student has no rights for sending message to student");
                if (Context.StudentClassMessagingOnly)
                {
                    //TODO: add filtering by school year 
                    var currentPersonClasses = new ClassPersonDataAccess(uow).GetClassPersons(new ClassPersonQuery { PersonId = Context.PersonId });
                    var toPersonClasses = new ClassPersonDataAccess(uow).GetClassPersons(new ClassPersonQuery { PersonId = toPerson.Id });
                    if (!toPersonClasses.Any(x => currentPersonClasses.Any(y => y.ClassRef == x.ClassRef)))
                        throw new ChalkableSecurityException("Current student can send messages only to student that are in the same classes.");
                }
            }
            if (toPerson.RoleRef == CoreRoles.TEACHER_ROLE.Id)
            {
                if (!Context.TeacherStudentMessaginEnabled)
                    throw new ChalkableSecurityException("Current student has no rights for sending message to teacher");
                if (Context.TeacherClassMessagingOnly)
                {
                    //TODO: add filtering by school year 
                    var toPersonClasses = new ClassTeacherDataAccess(uow).GetClassTeachers(null, toPerson.Id);
                    var currentPersonClasses = new ClassPersonDataAccess(uow).GetClassPersons(new ClassPersonQuery { PersonId = Context.PersonId });
                    if (!currentPersonClasses.Any(x => toPersonClasses.Any(y => x.ClassRef == y.ClassRef)))
                        throw new ChalkableSecurityException("Current student can send messages only to his teachers.");
                }
            }
        }

        

        private int CreatePrivateMessage(IList<int> personIds, int? classId, string subject, string body, UnitOfWork uow)
        {
            Trace.Assert(Context.PersonId.HasValue);

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
        
        public PaginatedList<PrivateMessage> GetMessages(int start, int count, bool? read, PrivateMessageType type, string role, string keyword)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);
            
            PrivateMessageSecurity.EnsureMessgingPermission(Context);
            using (var uow = Read())
            {
                var da = new PrivateMessageDataAccess(uow);
                var roles = string.IsNullOrWhiteSpace(role) ? new List<string>() : role.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).ToList();
                var rolesIds = roles.Select(x => CoreRoles.GetByName(x).Id).ToList();
                switch (type)
                {
                    case PrivateMessageType.Income:
                        var inMsg = da.GetIncomeMessages(Context.PersonId.Value, null, rolesIds, keyword, read,  start, count);
                        return new PaginatedList<PrivateMessage>(inMsg.Select(x => x), inMsg.PageIndex, inMsg.PageSize, inMsg.TotalCount);
                    case PrivateMessageType.Sent:
                        var sentMsg = da.GetSentMessages(Context.PersonId.Value, null, rolesIds, keyword, start, count);
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
                    Context.TeacherStudentMessaginEnabled,
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
            return !Context.MessagingDisabled && BaseSecurity.IsTeacher(Context) && Context.TeacherStudentMessaginEnabled &&
                   (!Context.TeacherClassMessagingOnly || new ClassTeacherDataAccess(uow).Exists(classId, Context.PersonId));
        }

        public bool CanSendMessageToPerson(int personId)
        {
            Trace.Assert(Context.PersonId.HasValue);
            Trace.Assert(Context.SchoolLocalId.HasValue);

            if (Context.MessagingDisabled)
                return false;

            bool canSend = true;
            var uow = Update();
            var toPerson = new PersonDataAccess(uow).GetPersonDetails(personId, Context.SchoolLocalId.Value); ;

            if (BaseSecurity.IsTeacher(Context) && toPerson.RoleRef == CoreRoles.STUDENT_ROLE.Id)
            {
                if (Context.TeacherStudentMessaginEnabled)
                {
                    if (Context.TeacherClassMessagingOnly)
                    {
                        var toPersonClasses = new ClassDataAccess(uow).GetStudentClasses(Context.SchoolYearId.Value,
                            personId, null);
                        var currPersonClasses = new ClassDataAccess(uow).GetTeacherClasses(Context.SchoolYearId.Value,
                            Context.PersonId.Value, null);

                        canSend = currPersonClasses.Any(x => toPersonClasses.Any(y => x.Id == y.Id));
                    }
                }
                else canSend = false;

            }
            else if (Context.Role == CoreRoles.STUDENT_ROLE && toPerson.RoleRef == CoreRoles.STUDENT_ROLE.Id)
            {
                if (Context.StudentMessagingEnabled)
                {
                    if (Context.StudentClassMessagingOnly)
                    {
                        var toPersonClasses = new ClassDataAccess(uow).GetStudentClasses(Context.SchoolYearId.Value,
                            personId, null);
                        var currPersonClasses = new ClassDataAccess(uow).GetStudentClasses(Context.SchoolYearId.Value,
                            Context.PersonId.Value, null);

                        canSend = currPersonClasses.Any(x => toPersonClasses.Any(y => x.Id == y.Id));
                    }
                }
                else canSend = false;
            }
            else canSend = false;

            return canSend;
        }
    }
}
