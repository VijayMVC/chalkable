using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPhoneStorage: BaseDemoStorage<Guid, Phone>
    {
        public DemoPhoneStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<Person> GetUsersByPhone(string phone)
        {
            return Storage.PersonStorage.GetPersonsByPhone(phone);
            throw new System.NotImplementedException();
        }

        public Phone GetPhone(int personId, string digitOnlyValue)
        {
            throw new System.NotImplementedException();
        }

        public Phone Add(Phone phone)
        {
            throw new System.NotImplementedException();
        }

        public void Update(Phone phone, AndQueryCondition andQueryCondition)
        {
            throw new System.NotImplementedException();
        }

        public IList<Phone> Add(IList<Phone> phones)
        {
            throw new System.NotImplementedException();
        }

        public IList<Phone> Update(IList<Phone> phones)
        {
            throw new System.NotImplementedException();
        }
    }
}
