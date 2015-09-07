﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPhoneService
    {
        IList<Phone> GetPhones();
        IList<Phone> GetPhones(int personId);
        Phone Add(string digitOnlyValue, int personId, string value, PhoneType type, bool isPrimary);
        IList<Phone> AddPhones(IList<Phone> phones);
        IList<Phone> EditPhones(IList<Phone> phones);
        Phone Edit(string digitOnlyValue, int personId, string value, PhoneType type, bool isPrimary);
        void Delete(string digitOnlyValue, int personId);
        void Delete(IList<Phone> phones);
        IList<Person> GetUsersByPhone(string phone);
    }

    public class PhoneService : SchoolServiceBase, IPhoneService
    {
        public PhoneService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
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
            using (var uow = Read())
            {
                return new PhoneDataAccess(uow).GetAll();
            }
        }

        public IList<Phone> GetPhones(int personId)
        {
            using (var uow = Read())
            {
                return new PhoneDataAccess(uow)
                    .GetAll(new AndQueryCondition{{Phone.PERSON_REF_FIELD, personId}});
            }
        }

        public Phone Add(string digitOnlyValue, int personId, string value, PhoneType type, bool isPrimary)
        {
            return AddPhones(new List<Phone>
                {
                    new Phone
                        {
                            Value = value,
                            DigitOnlyValue = DigitsOnly(value),
                            PersonRef = personId,
                            IsPrimary = isPrimary,
                            Type = type
                        }
                }).First();
        }

        public Phone Edit(string digitOnlyValue, int personId, string value, PhoneType type, bool isPrimary)
        {
            if (!(BaseSecurity.IsDistrictAdmin(Context)))
                throw new ChalkableSecurityException(); 
            using (var uow = Update())
            {
                var da = new PhoneDataAccess(uow);
                var phone = da.GetPhone(personId, digitOnlyValue);
                phone.DigitOnlyValue = DigitsOnly(value);
                phone.Value = value;
                phone.IsPrimary = isPrimary;
                phone.Type = type;
                da.Update(phone, new AndQueryCondition
                    {
                        {Phone.DIGIT_ONLY_VALUE_FIELD, digitOnlyValue},
                        {Phone.PERSON_REF_FIELD, personId}
                    });
                uow.Commit();
                return phone;
            }
        }

        public void Delete(string digitOnlyValue, int personId)
        {
            Delete(new List<Phone> { new Phone{PersonRef = personId, DigitOnlyValue = digitOnlyValue} });
        }

        public IList<Person> GetUsersByPhone(string phone)
        {
            using (var uow = Read())
            {
                return new PhoneDataAccess(uow).GetUsersByPhone(phone);
            }
        }


        public IList<Phone> AddPhones(IList<Phone> phones)
        {
            if (!(BaseSecurity.IsDistrictAdmin(Context)))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new PhoneDataAccess(uow).Insert(phones);
                uow.Commit();
                return phones;
            }
        }

        public void Delete(IList<Phone> phones)
        {
            if (!(BaseSecurity.IsDistrictAdmin(Context)))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new PhoneDataAccess(uow);
                da.Delete(phones);
                uow.Commit();
            }
        }


        public IList<Phone> EditPhones(IList<Phone> phones)
        {
            if (!(BaseSecurity.IsDistrictAdmin(Context)))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new PhoneDataAccess(uow).Update(phones);
                uow.Commit();
                return phones;
            }
        }
    }
}
