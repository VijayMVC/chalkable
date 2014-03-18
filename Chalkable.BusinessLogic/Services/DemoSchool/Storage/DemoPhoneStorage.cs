using System.Collections.Generic;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoPhoneStorage
    {
        public IList<Phone> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public IList<Phone> GetAll(int personId)
        {
            throw new System.NotImplementedException();
        }

        public IList<Person> GetUsersByPhone(string phone)
        {
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
