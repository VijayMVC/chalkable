using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPhoneStorage: BaseDemoIntStorage<Phone>
    {
        public DemoPhoneStorage(DemoStorage storage) : base(storage, null, true)
        {
        }

        public IList<Person> GetUsersByPhone(string phone)
        {
            return Storage.PersonStorage.GetPersonsByPhone(phone);
        }

        public Phone GetPhone(int personId, string digitOnlyValue)
        {
            return
                data.Where(x => x.Value.PersonRef == personId && x.Value.DigitOnlyValue == digitOnlyValue)
                    .Select(x => x.Value)
                    .First();
        }

        public void Update(Phone phone, string digitOnlyValue, int personId)
        {
            var phones =
                data.Where(x => x.Value.DigitOnlyValue == digitOnlyValue && x.Value.PersonRef == personId).Select(x => x.Key).ToList();
            foreach (var i in phones)
            {
                data[i].Type = phone.Type;
                data[i].IsPrimary = phone.IsPrimary;
                data[i].Value = phone.Value;
            }
        }

        public IList<Phone> GetAll(int personId)
        {
            return data.Where(x => x.Value.PersonRef == personId).Select(x => x.Value).ToList();
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
