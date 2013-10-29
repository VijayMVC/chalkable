using System;
using System.Collections.Generic;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonInfoViewData : PersonViewData
    {
        public int? Age { get; set; }
        [SensitiveData]
        public IList<PhoneViewData> Phones { get; set; }
        [SensitiveData]
        public AddressViewData Addresses { get; set; }

        protected PersonInfoViewData(PersonDetails person) : base(person)
        {
            int? age = null;
            if (person.BirthDate.HasValue)
                age = DateTime.UtcNow.Year - person.BirthDate.Value.Year; //TODO: think about

            Age = age;
            Addresses = AddressViewData.Create(person.Address);
            Phones = PhoneViewData.Create(person.Phones);
        }
        public static PersonInfoViewData Create(PersonDetails person)
        {
            return new PersonInfoViewData(person);
        }
        
    }
}