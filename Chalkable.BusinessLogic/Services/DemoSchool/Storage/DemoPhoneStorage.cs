using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPhoneStorage: BaseDemoStorage
    {
        private Dictionary<Guid, Phone> phonesData = new Dictionary<Guid, Phone>();

        public DemoPhoneStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<Phone> GetAll()
        {
            return phonesData.Select(x => x.Value).ToList();
        }

        public IList<Phone> GetAll(int personId)
        {
            return phonesData.Where(x => x.Value.PersonRef == personId).Select(x => x.Value).ToList();
        }

        public IList<Person> GetUsersByPhone(string phone)
        {
            return Storage.PersonStorage.GetPersonsByPhone(phone);
            throw new System.NotImplementedException();
        }

        public void Delete(Phone phone)
        {
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

        public void Delete(IList<Phone> toList)
        {
            throw new System.NotImplementedException();
        }

        public IList<Phone> Update(IList<Phone> phones)
        {
            throw new System.NotImplementedException();
        }
    }
}
