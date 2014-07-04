using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPrivateMessageService : DemoSchoolServiceBase, IPrivateMessageService
    {
        public DemoPrivateMessageService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public PrivateMessageDetails SendMessage(int toPersonId, string subject, string body)
        {
            if(!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();

            var toSchoolPerson = Storage.SchoolPersonStorage.GetSchoolPerson(toPersonId, Context.SchoolLocalId, null);
                if(toSchoolPerson == null)
                    throw new ChalkableSecurityException();

            var message = new PrivateMessage
            {
                Body = body,
                Subject = subject,
                FromPersonRef = Context.UserLocalId.Value,
                ToPersonRef = toPersonId,
                Sent = Context.NowSchoolTime,
                DeletedByRecipient = false,
                DeletedBySender = false
            };

            Storage.PrivateMessageStorage.Add(message);
            message =
                Storage.PrivateMessageStorage.GetOutComeMessage(null, null, Context.UserLocalId.Value, 0, int.MaxValue)
                    .First();

            ServiceLocator.NotificationService.AddPrivateMessageNotification(message.Id);
            return Storage.PrivateMessageStorage.GetDetailsById(message.Id, Context.UserLocalId.Value);
        }

        public PaginatedList<PrivateMessageDetails> GetMessages(int start, int count, bool? read, PrivateMessageType type, string role, string keyword)
        {
            var roles = role.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var rolesIds = roles.Select(x => CoreRoles.GetByName(x).Id).ToList();

            switch (type)
            {
                case PrivateMessageType.Income:
                    return Storage.PrivateMessageStorage.GetIncomeMessages(rolesIds, keyword, read, Context.UserLocalId ?? 0, start, count);
                case PrivateMessageType.Outcome:
                    return Storage.PrivateMessageStorage.GetOutComeMessage(rolesIds, keyword, Context.UserLocalId ?? 0, start, count);
                default:
                    throw new ChalkableException(ChlkResources.ERR_PRIVATE_MESSAGE_INVALID_TYPE);
            }    
        }

        public void MarkAsRead(IList<int> ids, bool read)
        {
            if (ids != null)
            {
                var messages =
                    Storage.PrivateMessageStorage.GetNotDeleted(Context.UserLocalId ?? 0)
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
                Storage.PrivateMessageStorage.Update(messages);
            }
        }

        public void Delete(IList<int> ids)
        {
            if (ids != null)
            {

                var messages = Storage.PrivateMessageStorage.GetNotDeleted(Context.UserLocalId ?? 0).Where(x => ids.Contains(x.Id)).ToList();
                if (messages.Count == 0)
                    throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_DELETE_INVALID_RIGHTS);

                foreach (var message in messages)
                {
                    if (!PrivateMessageSecurity.CanDeleteMessage(message, Context))
                        throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_DELETE_INVALID_RIGHTS);

                    if (message.ToPersonRef == Context.UserLocalId)
                        message.DeletedByRecipient = true;
                    if (message.FromPersonRef == Context.UserLocalId)
                        message.DeletedBySender = true;
                }
                Storage.PrivateMessageStorage.Update(messages);
            }
        }

        public PrivateMessageDetails GetMessage(int id)
        {
            return Storage.PrivateMessageStorage.GetDetailsById(id, Context.UserLocalId ?? 0);
        }
    }
}
