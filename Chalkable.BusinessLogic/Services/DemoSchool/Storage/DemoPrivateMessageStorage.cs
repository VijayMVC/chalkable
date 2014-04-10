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

        private IEnumerable<PrivateMessageDetails> GetPrivateMessagesQuery(IList<int> roles, string keyword, bool? read, int personId, bool isIncome, int start, int count)
        {
            var msgs = data.Select(x => x.Value);
      
            if (!isIncome)
                msgs = msgs.Where(x => x.FromPersonRef == personId);
            else
                msgs = msgs.Where(x => x.ToPersonRef == personId);
            

            if (!string.IsNullOrWhiteSpace(keyword))
                msgs = msgs.Where(x => x.Body.Contains(keyword));
            //filter by roles

            if (read.HasValue)
                msgs = msgs.Where(x => x.Read == read);

            var msgDetails = msgs.Skip(start).Take(count).Select(x => new PrivateMessageDetails
            {
                Id = x.Id,
                Body = x.Body,
                DeletedByRecipient = x.DeletedByRecipient,
                DeletedBySender = x.DeletedBySender,
                FromPersonRef = x.FromPersonRef,
                Read = x.Read,
                Recipient = Storage.PersonStorage.GetById(x.ToPersonRef),
                Sender = Storage.PersonStorage.GetById(x.FromPersonRef),
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
            return new PaginatedList<PrivateMessageDetails>(msgs.ToList(), start / count, count, data.Count);
        }

        public PaginatedList<PrivateMessageDetails> GetIncomeMessages(IList<int> roles, string keyword, bool? read,
            int personId, int start, int count)
        {
            var msgs = GetPrivateMessagesQuery(roles, keyword, read, personId, true, start, count);
            return new PaginatedList<PrivateMessageDetails>(msgs.ToList(), start / count, count);
        }

        public void Add(PrivateMessage message)
        {
            message.Id = GetNextFreeId();
            data.Add(message.Id, message);
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
