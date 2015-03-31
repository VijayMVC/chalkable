﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPrivateMessageStorage:BaseDemoIntStorage<PrivateMessage>
    {
        public DemoPrivateMessageStorage() : base(x => x.Id, true)
        {
        }

        private IEnumerable<PrivateMessageDetails> GetPrivateMessagesQuery(IList<int> roles, string keyword, bool? read, int personId, bool isIncome, int start, int count)
        {
            var msgs = data.Select(x => x.Value);
      
            

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                msgs = msgs.Where(x => x.Body.Contains(keyword) || x.Subject.Contains(keyword));    
            }

            if (roles == null)
                roles = new List<int>();

            if (!isIncome)
            {

                var fromPersonIds = msgs.Select(x => x.FromPersonRef);
                var persons = fromPersonIds.Select(fromPersonId => StorageLocator.PersonStorage.GetById(fromPersonId)).ToList();

                var fromPersonIdsFiltered = roles.Count > 0
                    ? persons.Where(x => roles.Contains(x.RoleRef)).Select(x => x.Id).ToList()
                    : persons.Select(x => x.Id).ToList();

                msgs = msgs.Where(x => fromPersonIdsFiltered.Contains(x.FromPersonRef) && !x.DeletedBySender);
            }
            else
            {
                var toPersonIds = msgs.Select(x => x.ToPersonRef);

                var persons = toPersonIds.Select(toPersonId => StorageLocator.PersonStorage.GetById(toPersonId)).ToList();

                var toPersonIdsFiltered = roles.Count > 0 
                    ? persons.Where(x => roles.Contains(x.RoleRef)).Select(x => x.Id).ToList()
                    : persons.Select(x => x.Id).ToList();
                msgs = msgs.Where(x => toPersonIdsFiltered.Contains(x.ToPersonRef) &&  !x.DeletedByRecipient);
            }
         
            if (read.HasValue)
                msgs = msgs.Where(x => x.Read == read);

            if (!isIncome)
                msgs = msgs.Where(x => x.FromPersonRef == personId);
            else
                msgs = msgs.Where(x => x.ToPersonRef == personId);

            var msgDetails = msgs.Skip(start).Take(count).Select(x => new PrivateMessageDetails
            {
                Id = x.Id,
                Body = x.Body,
                DeletedByRecipient = x.DeletedByRecipient,
                DeletedBySender = x.DeletedBySender,
                FromPersonRef = x.FromPersonRef,
                Read = x.Read,
                Recipient = StorageLocator.PersonStorage.GetById(x.ToPersonRef),
                Sender = StorageLocator.PersonStorage.GetById(x.FromPersonRef),
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

            msgDetails.Sender = StorageLocator.PersonStorage.GetById(msgDetails.FromPersonRef);
            msgDetails.Recipient = StorageLocator.PersonStorage.GetById(msgDetails.ToPersonRef);
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
    }
}
