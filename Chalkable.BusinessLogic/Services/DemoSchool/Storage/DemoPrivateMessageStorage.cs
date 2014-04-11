using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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



            /* if (!string.IsNullOrEmpty(keyword))
            {
                keyword = "%" + keyword + "%";
                b.AppendFormat(@" and (PrivateMessage_Subject like @keyword or PrivateMessage_Body like @keyword
                                        or lower(PrivateMessage_{0}FirstName) like @keyword or lower(PrivateMessage_{0}LastName) like @keyword)"
                                        , prefix);
                conds.Add("keyword", keyword);

           
            if (!string.IsNullOrWhiteSpace(keyword))
            {

                if (isIncome)
                {
                    var fromPersonIds = msgs.Select(x => x.FromPersonRef);
                    var fromPersonTexts = new List<string>();
                    foreach (var fromPersonId in fromPersonIds)
                    {
                        var person = Storage.PersonStorage.GetById(fromPersonId);
                        fromPersonTexts.Add(person.FirstName);
                        fromPersonTexts.Add(person.LastName);
                    }
                }
            }
             *  */
            msgs = msgs.Where(x => x.Body.Contains(keyword));

            if (roles == null)
                roles = new List<int>();

            if (roles.Count > 0)
            {
                if (isIncome)
                {

                    var fromPersonIds = msgs.Select(x => x.FromPersonRef);
                    var persons = fromPersonIds.Select(fromPersonId => Storage.PersonStorage.GetById(fromPersonId)).ToList();

                    var fromPersonIdsFiltered = persons.Where(x => roles.Contains(x.RoleRef)).Select(x => x.Id).ToList();

                    msgs = msgs.Where(x => fromPersonIdsFiltered.Contains(x.FromPersonRef));
                }
                else
                {
                    var toPersonIds = msgs.Select(x => x.ToPersonRef);
                    var persons = toPersonIds.Select(toPersonId => Storage.PersonStorage.GetById(toPersonId)).ToList();

                    var toPersonIdsFiltered = persons.Where(x => roles.Contains(x.RoleRef)).Select(x => x.Id).ToList();
                    msgs = msgs.Where(x => toPersonIdsFiltered.Contains(x.ToPersonRef));
                }
            }
            
         
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
