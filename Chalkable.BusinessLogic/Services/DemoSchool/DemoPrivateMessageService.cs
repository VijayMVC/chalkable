using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPrivateMessageStorage : BaseDemoIntStorage<PrivateMessage>
    {
        public DemoPrivateMessageStorage()
            : base(x => x.Id, true)
        {
        }

    }

    public class DemoPrivateMessageService : DemoSchoolServiceBase, IPrivateMessageService
    {
        private DemoPrivateMessageStorage PrivateMessageStorage { get; set; }
        public DemoPrivateMessageService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            PrivateMessageStorage = new DemoPrivateMessageStorage();
        }

        public PrivateMessageDetails SendMessage(int toPersonId, string subject, string body)
        {
            if (!Context.PersonId.HasValue || !Context.SchoolLocalId.HasValue)
                throw new UnassignedUserException();

            var toSchoolPerson = ServiceLocator.PersonService.GetPerson(toPersonId);
                if(toSchoolPerson == null)
                    throw new ChalkableSecurityException();

            var message = new PrivateMessage
            {
                Body = body,
                Subject = subject,
                FromPersonRef = Context.PersonId.Value,
                ToPersonRef = toPersonId,
                Sent = Context.NowSchoolTime,
                DeletedBySender = false,
                DeletedByRecipient = false
            };

            PrivateMessageStorage.Add(message);
            message = GetOutComeMessage(null, null, Context.PersonId.Value, 0, int.MaxValue).OrderByDescending(x => x.Id).First();

            ServiceLocator.NotificationService.AddPrivateMessageNotification(message.Id);
            return GetMessage(message.Id, Context.PersonId.Value);
        }

        public PaginatedList<PrivateMessageDetails> GetMessages(int start, int count, bool? read, PrivateMessageType type, string role, string keyword)
        {
            var roles = role.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var rolesIds = roles.Select(x => CoreRoles.GetByName(x).Id).ToList();

            switch (type)
            {
                case PrivateMessageType.Income:
                    return GetIncomeMessages(rolesIds, keyword, read, Context.PersonId ?? 0, start, count);
                case PrivateMessageType.Outcome:
                    return GetOutComeMessage(rolesIds, keyword, Context.PersonId ?? 0, start, count);
                default:
                    throw new ChalkableException(ChlkResources.ERR_PRIVATE_MESSAGE_INVALID_TYPE);
            }    
        }

        public void MarkAsRead(IList<int> ids, bool read)
        {
            if (ids == null) return;
            var messages =
                GetNotDeleted(Context.PersonId ?? 0)
                    .Where(x => ids.Contains(x.Id))
                    .ToList();
            if (messages.Count == 0)
                throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_MARK_INVALID_RIGHTS);
            foreach (var message in messages)
            {
                if (!PrivateMessageSecurity.CanMarkMessage(message, Context))
                    throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_MARK_INVALID_RIGHTS);
                message.Read = read;
            }
            PrivateMessageStorage.Update(messages);
        }

        public void Delete(IList<int> ids)
        {
            if (ids != null)
            {

                var messages = GetNotDeleted(Context.PersonId ?? 0).Where(x => ids.Contains(x.Id)).ToList();
                if (messages.Count == 0)
                    throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_DELETE_INVALID_RIGHTS);

                foreach (var message in messages)
                {
                    if (!PrivateMessageSecurity.CanDeleteMessage(message, Context))
                        throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_DELETE_INVALID_RIGHTS);

                    if (message.ToPersonRef == Context.PersonId)
                        message.DeletedByRecipient = true;
                    if (message.FromPersonRef == Context.PersonId)
                        message.DeletedBySender = true;
                }
                PrivateMessageStorage.Update(messages);
            }
        }

        public PrivateMessageDetails GetMessage(int id)
        {
            return GetMessage(id, Context.PersonId ?? 0);
        }

        public PrivateMessageDetails GetMessage(int msgId, int userLocalId)
        {
            var msg =
               PrivateMessageStorage.GetData().Where(
                   x =>
                       x.Value.Id == msgId && (x.Value.ToPersonRef == userLocalId || x.Value.FromPersonRef == userLocalId))
                   .Select(x => x.Value)
                   .First();

            var msgDetails = new PrivateMessageDetails
            {
                Body = msg.Body,
                DeletedByRecipient = msg.DeletedByRecipient,
                DeletedBySender = msg.DeletedBySender,
                FromPersonRef = msg.FromPersonRef,
                Id = msg.Id,
                Read = msg.Read,
                Sent = msg.Sent,
                ToPersonRef = msg.ToPersonRef,
                Subject = msg.Subject
            };

            msgDetails.Sender = ServiceLocator.PersonService.GetPerson(msgDetails.FromPersonRef);
            msgDetails.Recipient = ServiceLocator.PersonService.GetPerson(msgDetails.ToPersonRef);
            return msgDetails;
        }

        private IEnumerable<PrivateMessageDetails> GetPrivateMessagesQuery(IList<int> roles, string keyword, bool? read, int personId, bool isIncome, int start, int count)
        {
            var msgs = PrivateMessageStorage.GetData().Select(x => x.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                msgs = msgs.Where(x => x.Body.Contains(keyword) || x.Subject.Contains(keyword));
            }

            if (roles == null)
                roles = new List<int>();

            if (!isIncome)
            {

                var fromPersonIds = msgs.Select(x => x.FromPersonRef);
                var persons = fromPersonIds.Select(fromPersonId => ServiceLocator.PersonService.GetPerson(fromPersonId)).ToList();

                var fromPersonIdsFiltered = roles.Count > 0
                    ? persons.Where(x => roles.Contains(x.RoleRef)).Select(x => x.Id).ToList()
                    : persons.Select(x => x.Id).ToList();

                msgs = msgs.Where(x => fromPersonIdsFiltered.Contains(x.FromPersonRef) && !x.DeletedBySender);
            }
            else
            {
                var toPersonIds = msgs.Select(x => x.ToPersonRef);

                var persons = toPersonIds.Select(toPersonId => ServiceLocator.PersonService.GetPerson(toPersonId)).ToList();

                var toPersonIdsFiltered = roles.Count > 0
                    ? persons.Where(x => roles.Contains(x.RoleRef)).Select(x => x.Id).ToList()
                    : persons.Select(x => x.Id).ToList();
                msgs = msgs.Where(x => toPersonIdsFiltered.Contains(x.ToPersonRef) && !x.DeletedByRecipient);
            }

            if (read.HasValue)
                msgs = msgs.Where(x => x.Read == read);

            msgs = !isIncome ? msgs.Where(x => x.FromPersonRef == personId) : msgs.Where(x => x.ToPersonRef == personId);

            var msgDetails = msgs.Skip(start).Take(count).Select(x => new PrivateMessageDetails
            {
                Id = x.Id,
                Body = x.Body,
                DeletedByRecipient = x.DeletedByRecipient,
                DeletedBySender = x.DeletedBySender,
                FromPersonRef = x.FromPersonRef,
                Read = x.Read,
                Recipient = ServiceLocator.PersonService.GetPerson(x.ToPersonRef),
                Sender = ServiceLocator.PersonService.GetPerson(x.FromPersonRef),
                Sent = x.Sent,
                Subject = x.Subject,
                ToPersonRef = x.ToPersonRef
            });

            return msgDetails;
        }

        public PaginatedList<PrivateMessageDetails> GetOutComeMessage(IList<int> roles, string keyword, int personId,
            int start, int count)
        {

            var msgs = GetPrivateMessagesQuery(roles, keyword, null, personId, false, start, count);
            return new PaginatedList<PrivateMessageDetails>(msgs.ToList(), start / count, count, PrivateMessageStorage.GetData().Count);
        }

        public PaginatedList<PrivateMessageDetails> GetIncomeMessages(IList<int> roles, string keyword, bool? read,
            int personId, int start, int count)
        {
            var msgs = GetPrivateMessagesQuery(roles, keyword, read, personId, true, start, count);
            return new PaginatedList<PrivateMessageDetails>(msgs.ToList(), start / count, count);
        }

        public IList<PrivateMessage> GetNotDeleted(int callerId)
        {
            return
               PrivateMessageStorage.GetData().Where(
                   x =>
                       (x.Value.FromPersonRef == callerId && x.Value.DeletedBySender == false) ||
                       x.Value.ToPersonRef == callerId && x.Value.DeletedByRecipient == false)
                   .Select(x => x.Value)
                   .ToList();
        }

    }
}
