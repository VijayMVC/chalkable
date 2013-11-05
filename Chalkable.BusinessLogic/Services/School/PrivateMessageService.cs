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
        PrivateMessageDetails SendMessage(int toPersonId, string subject, string body);
        PaginatedList<PrivateMessageDetails> GetMessages(int start, int count, bool? read, PrivateMessageType type, string role, string keyword);
        void MarkAsRead(IList<int> ids, bool read);
        void Delete(IList<int> id);
        PrivateMessageDetails GetMessage(int id);
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

        public PrivateMessageDetails SendMessage(int toPersonId, string subject, string body)
        {
            if(!Context.UserLocalId.HasValue)
                throw new UnassignedUserException();
            using (var uow = Update())
            {
                var da = new PrivateMessageDataAccess(uow);
                var toSchoolPerson = new SchoolPersonDataAccess(uow).GetSchoolPerson(toPersonId, Context.SchoolLocalId, null);
                if(toSchoolPerson == null)
                    throw new ChalkableSecurityException();
                var message = new PrivateMessage
                    {
                        Body = body,
                        Subject = subject,
                        FromPersonRef = Context.UserLocalId.Value,
                        ToPersonRef = toPersonId,
                        Sent = Context.NowSchoolTime
                    };
                da.Insert(message);
                uow.Commit();
                message = da.GetOutComeMessage(null, null, Context.UserLocalId.Value, 0, int.MaxValue).Last();
                //TODO: notification sending 
                ServiceLocator.NotificationService.AddPrivateMessageNotification(message.Id);
                return da.GetDetailsById(message.Id, Context.UserLocalId.Value);
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
                        return da.GetIncomeMessages(rolesIds, keyword, read, Context.UserLocalId ?? 0, start, count);
                    case PrivateMessageType.Outcome:
                        return da.GetOutComeMessage(rolesIds, keyword, Context.UserLocalId ?? 0, start, count);
                    default:
                        throw new ChalkableException(ChlkResources.ERR_PRIVATE_MESSAGE_INVALID_TYPE);
                }    
            }
        }

        public void MarkAsRead(IList<int> ids, bool read)
        {
            if (ids != null)
                using (var uow = Update())
                {
                    var da = new PrivateMessageDataAccess(uow);
                    var messages = da.GetNotDeleted(Context.UserLocalId ?? 0).Where(x=>ids.Contains(x.Id)).ToList();
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

        public void Delete(IList<int> ids)
        {
            if (ids != null)
                using (var uow = Update())
                {
                    var da = new PrivateMessageDataAccess(uow);
                    var messages = da.GetNotDeleted(Context.UserLocalId ?? 0).Where(x => ids.Contains(x.Id)).ToList();
                    if (messages.Count == 0)
                        throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_DELETE_INVALID_RIGHTS);

                    foreach (var message in messages)
                    {
                        if (!PrivateMessageSecurity.CanDeleteMessage(message, Context))
                            throw new ChalkableSecurityException(ChlkResources.ERR_PRIVATE_MESSAGE_DELETE_INVALID_RIGHTS);

                        if(message.ToPersonRef == Context.UserLocalId)
                            message.DeletedByRecipient = true;
                        if(message.FromPersonRef == Context.UserLocalId)
                            message.DeletedBySender = true;
                    }
                    da.Update(messages);
                    uow.Commit();
                }
        }

        public PrivateMessageDetails GetMessage(int id)
        {
            using (var uow = Read())
            {
                var da = new PrivateMessageDataAccess(uow);
                var res = da.GetDetailsById(id, Context.UserLocalId ?? 0);
                return res;
            }
        }
    }
}
