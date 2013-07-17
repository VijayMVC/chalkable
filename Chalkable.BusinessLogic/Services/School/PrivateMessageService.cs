using System;
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
        PrivateMessage SendMessage(Guid toPersonId, string subject, string body);
        PaginatedList<PrivateMessageDetails> GetMessages(int start, int count, bool? read, PrivateMessageType type, string role, string keyword);
        PrivateMessage MarkAsRead(Guid id, bool read);
        void Delete(Guid id);
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

        public PrivateMessage SendMessage(Guid toPersonId, string subject, string body)
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
                return message;
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

        public PrivateMessage MarkAsRead(Guid id, bool read)
        {
            using (var uow = Update())
            {
                var da = new PrivateMessageDataAccess(uow);
                var message = da.GetById(id);
                if(!PrivateMessageSecurity.CanMarkMessage(message, Context))
                    throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_MARK_INVALID_RIGHTS);

                message.Read = read;
                da.Update(message);
                uow.Commit();
                return message;
            }
        }

        public void Delete(Guid id)
        {
            using (var uow = Update())
            {
                var da = new PrivateMessageDataAccess(uow);
                var message = da.GetById(id);
                if(!PrivateMessageSecurity.CanDeleteMessage(message, Context))
                    throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_DELETE_INVALID_RIGHTS);

                message.DeletedByRecipient = message.ToPersonRef == Context.UserId;
                message.DeletedBySender = message.FromPersonRef == Context.UserId;
                da.Update(message);
                uow.Commit();
            }
        }
    }
}
