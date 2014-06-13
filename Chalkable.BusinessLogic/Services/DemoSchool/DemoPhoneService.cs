﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPhoneService : DemoSchoolServiceBase, IPhoneService
    {
        public DemoPhoneService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        private static string DigitsOnly(string phone)
        {
            var res = "";
            for (int i = 0; i < phone.Length; i++)
                if (char.IsDigit(phone[i]) || phone[i] == '+')
                    res += phone[i];
            return res;
        }

        public IList<Phone> GetPhones()
        {
            return Storage.PhoneStorage.GetAll();
        }

        public IList<Phone> GetPhones(int personId)
        {
            return Storage.PhoneStorage.GetAll(personId);
            
        }

        public Phone Add(string digitOnlyValue, int personId, string value, PhoneType type, bool isPrimary)
        {
            if (!(BaseSecurity.IsDistrict(Context)))
                throw new ChalkableSecurityException();

            return Storage.PhoneStorage.Add(new Phone
            {
                Value = value,
                DigitOnlyValue = DigitsOnly(value),
                PersonRef = personId,
                IsPrimary = isPrimary,
                Type = type
            });
        }

        public IList<Phone> AddPhones(IList<Phone> phones)
        {
            if (!(BaseSecurity.IsDistrict(Context)))
                throw new ChalkableSecurityException();
            return Storage.PhoneStorage.Add(phones);
        }

        public IList<Phone> EditPhones(IList<Phone> phones)
        {
            if (!(BaseSecurity.IsDistrict(Context)))
                throw new ChalkableSecurityException();

            return Storage.PhoneStorage.Update(phones);
        }

        public Phone Edit(string digitOnlyValue, int personId, string value, PhoneType type, bool isPrimary)
        {
            var phone = Storage.PhoneStorage.GetPhone(personId, digitOnlyValue);
            if (!(BaseSecurity.IsDistrict(Context)))
                throw new ChalkableSecurityException();
            phone.DigitOnlyValue = DigitsOnly(value);
            phone.Value = value;
            phone.IsPrimary = isPrimary;
            phone.Type = type;
            Storage.PhoneStorage.Update(phone, digitOnlyValue, personId);
            return phone;

        }

        public void Delete(string digitOnlyValue, int personId)
        {
            var phone = Storage.PhoneStorage.GetPhone(personId, digitOnlyValue);
            if (!(BaseSecurity.IsDistrict(Context)))
                throw new ChalkableSecurityException();
            Storage.PhoneStorage.Delete(phone);
        }

        public void Delete(IList<Phone> phones)
        {
            if (!(BaseSecurity.IsDistrict(Context)))
                throw new ChalkableSecurityException();

            Storage.PhoneStorage.Delete(phones);
        }
        
        public IList<Person> GetUsersByPhone(string phone)
        {
            return Storage.PhoneStorage.GetUsersByPhone(phone);
        }
    }
}
