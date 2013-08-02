using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPrivateMessageService
    {
        PrivateMessageDetails SendMessage(Guid toPersonId, string subject, string body);
        PaginatedList<PrivateMessageDetails> GetMessages(int start, int count, bool? read, PrivateMessageType type, string role, string keyword);
        void MarkAsRead(IList<Guid> ids, bool read);
        void Delete(IList<Guid> id);
    }

    public enum PrivateMessageType
    {
        Income,
        Outcome
    }

    //todo: needs tests

    public class PrivateMessageService : SchoolServiceBase, IPrivateMessageService
    {
        public PrivateMessageService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public PrivateMessageDetails SendMessage(Guid toPersonId, string subject, string body)
        {
            using (var uow = Update())
            {
                var da = new PrivateMessageDataAccess(uow);
                var message = new PrivateMessage
                    {
                        Id = Guid.NewGuid(),
                        Body = body,
                        Subject = subject,
                        FromPersonRef = Context.UserId,
                        ToPersonRef = toPersonId,
                        Sent = Context.NowSchoolTime
                    };
                da.Insert(message);
                uow.Commit();
                ServiceLocator.NotificationService.AddPrivateMessageNotification(message.Id);
                return da.GetDetailsById(message.Id, Context.UserId);
            }
        }

        public PaginatedList<PrivateMessageDetails> GetMessages(int start, int count, bool? read, PrivateMessageType type, string role, string keyword)
        {
            using (var uow = Read())
            {
                var da = new PrivateMessageDataAccess(uow);
                var roles = role.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries).ToList();
                var rolesIds = roles.Select(x => CoreRoles.GetByName(x).Id).ToList();
                switch (type)
                {
                    case PrivateMessageType.Income:
                        return da.GetIncomeMessages(rolesIds, keyword, read, Context.UserId, start, count);
                    case PrivateMessageType.Outcome:
                        return da.GetOutComeMessage(rolesIds, keyword, Context.UserId, start, count);
                    default:
                        throw new ChalkableException(ChlkResources.ERR_PRIVATE_MESSAGE_INVALID_TYPE);
                }    
            }
        }

        public void MarkAsRead(IList<Guid> ids, bool read)
        {
            using (var uow = Update())
            {
                var da = new PrivateMessageDataAccess(uow);
                var messages = da.GetNotDeleted(Context.UserId).Where(x=>ids.Contains(x.Id)).ToList();
                if(messages.Count == 0)
                    throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_MARK_INVALID_RIGHTS);

                foreach (var message in messages)
                {
                    if (!PrivateMessageSecurity.CanMarkMessage(message, Context))
                        throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_MARK_INVALID_RIGHTS);
                    message.Read = read;
                }
                da.Update(messages);
                uow.Commit();
            }
        }

        public void Delete(IList<Guid> ids)
        {
            using (var uow = Update())
            {
                var da = new PrivateMessageDataAccess(uow);
                var messages = da.GetNotDeleted(Context.UserId).Where(x => ids.Contains(x.Id)).ToList();
                if (messages.Count == 0)
                    throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_DELETE_INVALID_RIGHTS);

                foreach (var message in messages)
                {
                    if (!PrivateMessageSecurity.CanDeleteMessage(message, Context))
                        throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_DELETE_INVALID_RIGHTS);

                    message.DeletedByRecipient = message.ToPersonRef == Context.UserId;
                    message.DeletedBySender = message.FromPersonRef == Context.UserId;
                }
                da.Update(messages);
                uow.Commit();
            }
        }
    }
}
