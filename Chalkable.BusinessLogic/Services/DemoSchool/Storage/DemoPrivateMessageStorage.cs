using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPrivateMessageStorage:BaseDemoStorage<int ,PrivateMessage>
    {
        public DemoPrivateMessageStorage(DemoStorage storage) : base(storage)
        {
        }

        public PaginatedList<PrivateMessageDetails> GetOutComeMessage(IList<int> roles, string keyword, int personId,
            int start, int count)
        {
            throw new NotImplementedException();
        }

        public PaginatedList<PrivateMessageDetails> GetIncomeMessages(IList<int> roles, string keyword, bool? read,
            int personId, int start, int count)
        {
            throw new NotImplementedException();
        }

        public void Add(PrivateMessage message)
        {
            if (!data.ContainsKey(message.Id))
                data[message.Id] = message;
        }

        public PrivateMessageDetails GetDetailsById(int id, int userLocalId)
        {
            var msg =
                data.Where(
                    x =>
                        x.Value.Id == id && (x.Value.ToPersonRef == userLocalId || x.Value.FromPersonRef == userLocalId))
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

            msgDetails.Sender = Storage.PersonStorage.GetById(msgDetails.FromPersonRef);
            msgDetails.Recipient = Storage.PersonStorage.GetById(msgDetails.ToPersonRef);
            return msgDetails;
        }

        public IList<PrivateMessage> GetNotDeleted(int callerId)
        {
             return
                data.Where(
                    x =>
                        (x.Value.FromPersonRef == callerId && x.Value.DeletedBySender == false) ||
                        x.Value.ToPersonRef == callerId && x.Value.DeletedByRecipient == false)
                    .Select(x => x.Value)
                    .ToList();
        }

        public void Update(IList<PrivateMessage> messages)
        {
            foreach (var privateMessage in messages)
            {
                if (data.ContainsKey(privateMessage.Id))
                    data[privateMessage.Id] = privateMessage;

            }          
        }
    }
}
