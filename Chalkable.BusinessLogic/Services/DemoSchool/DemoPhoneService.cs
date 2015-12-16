using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoPhoneStorage : BaseDemoIntStorage<Phone>
    {
        public DemoPhoneStorage()
            : base(null, true)
        {
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
    }

    public class DemoPhoneService : DemoSchoolServiceBase, IPhoneService
    {
        private DemoPhoneStorage PhoneStorage { get; set; }
        public DemoPhoneService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            PhoneStorage = new DemoPhoneStorage();
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
            return PhoneStorage.GetAll();
        }

        public IList<Phone> GetPhones(int personId)
        {
            return PhoneStorage.GetAll(personId);
        }

        public Phone Add(string digitOnlyValue, int personId, string value, PhoneType type, bool isPrimary)
        {
            return PhoneStorage.Add(new Phone
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
            return PhoneStorage.Add(phones);
        }

        public IList<Phone> EditPhones(IList<Phone> phones)
        {
            return PhoneStorage.Update(phones);
        }

        public Phone Edit(string digitOnlyValue, int personId, string value, PhoneType type, bool isPrimary)
        {
            throw new NotImplementedException();
        }

        public void Delete(string digitOnlyValue, int personId)
        {
            throw new NotImplementedException();
        }

        public void Delete(IList<Phone> phones)
        {
            throw new NotImplementedException();
        }
        
        public IList<Person> GetUsersByPhone(string phone)
        {
            return new List<Person>();
        }
    }
}
