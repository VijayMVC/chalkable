using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPhoneStorage: BaseDemoStorage<int, Phone>
    {
        public DemoPhoneStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<Person> GetUsersByPhone(string phone)
        {
            return Storage.PersonStorage.GetPersonsByPhone(phone);
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

        public IList<Phone> GetAll(int personId)
        {
            return data.Where(x => x.Value.PersonRef == personId).Select(x => x.Value).ToList();
        }

        public void Delete(Phone phone)
        {
            var item = data.First(x => x.Value == phone);
            Delete(item.Key);
        }

        public void Delete(List<Phone> toList)
        {
            foreach (var phone in toList)
            {
                Delete(phone);
            }
        }
    }
}
