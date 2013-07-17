using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IPhoneService
    {
        IList<Phone> GetPhones();
        Phone Add(Guid personId, string value, PhoneType type, bool isPrimary);
        Phone Edit(Guid id, string value, PhoneType type, bool isPrimary);
        void Delete(Guid id);
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

        public Phone Add(Guid personId, string value, PhoneType type, bool isPrimary)
        {
            if(!(BaseSecurity.IsAdminEditor(Context) || Context.UserId == personId))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new PhoneDataAccess(uow);
                var phone = new Phone
                    {
                        Id = Guid.NewGuid(),
                        Value = value,
                        DigitOnlyValue = DigitsOnly(value),
                        PersonRef = personId,
                        IsPrimary = isPrimary,
                        Type = type
                    };
                da.Insert(phone);
                uow.Commit();
                return phone;
            }
        }

        public Phone Edit(Guid id, string value, PhoneType type, bool isPrimary)
        {
            using (var uow = Update())
            {
                var da = new PhoneDataAccess(uow);
                var phone = da.GetById(id);
                if (!(BaseSecurity.IsAdminEditor(Context) || Context.UserId == phone.PersonRef))
                    throw new ChalkableSecurityException();
                phone.DigitOnlyValue = DigitsOnly(value);
                phone.Value = value;
                phone.IsPrimary = isPrimary;
                phone.Type = type;
                da.Update(phone);
                uow.Commit();
                return phone;
            }
        }

        public void Delete(Guid id)
        {
            using (var uow = Update())
            {
                var da = new PhoneDataAccess(uow);
                var phone = da.GetById(id);
                if (!(BaseSecurity.IsAdminEditor(Context) || Context.UserId == phone.PersonRef))
                    throw new ChalkableSecurityException();
                da.Delete(phone.Id);
                uow.Commit();
            }
        }

        public IList<Person> GetUsersByPhone(string phone)
        {
            using (var uow = Read())
            {
               return  new PhoneDataAccess(uow).GetUsersByPhone(phone);
            }
        }
    }
}
