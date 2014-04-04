using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPrivateMessageStorage
    {
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
            throw new NotImplementedException();
        }

        public PrivateMessageDetails GetDetailsById(int id, int userLocalId)
        {
            throw new NotImplementedException();
        }

        public IList<PrivateMessage> GetNotDeleted(int callerId)
        {
            throw new NotImplementedException();
        }

        public void Update(IList<PrivateMessage> messages)
        {
            
        }
    }
}
