﻿using System;
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
            return
                data.Where(x => x.Value.PersonRef == personId && x.Value.DigitOnlyValue == digitOnlyValue)
                    .Select(x => x.Value)
                    .First();
        }

        public Phone Add(Phone phone)
        {
            data.Add(GetNextFreeId(), phone);
            return phone;
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

        public IList<Phone> Add(IList<Phone> phones)
        {
            foreach (var phone in phones)
            {
                Add(phone);
            }
            return phones;
        }

        public IList<Phone> Update(IList<Phone> phones)
        {
            foreach (var phone in phones)
            {
                var item = data.First(x => x.Value == phone);
                data[item.Key] = phone;
            }
            return phones;
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

        public void Delete(IList<Phone> toList)
        {
            foreach (var phone in toList)
            {
                Delete(phone);
            }
        }

        public override void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
