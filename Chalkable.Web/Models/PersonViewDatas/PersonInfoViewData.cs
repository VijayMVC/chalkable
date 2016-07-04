using System;
using System.Collections.Generic;
using Chalkable.Common.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class PersonInfoViewData : PersonViewData
    {
        public string Login { get; set; }
        public bool CanEditLogin { get; set; }
        public string Email { get; set; }
        public int? Age { get; set; }
        public EthnicityViewData Ethnicity { get; set; }
        [SensitiveData]
        public IList<PhoneViewData> Phones { get; set; }
        [SensitiveData]
        public AddressViewData Address { get; set; }

        protected PersonInfoViewData(PersonDetails person) : base(person)
        {
            Email = person.Email;

            if (person.BirthDate.HasValue)
                Age = CalculateAge(person.BirthDate.Value);

            if (person.Address != null)
                Address = AddressViewData.Create(person.Address);

            if (person.Phones != null)
                Phones = PhoneViewData.Create(person.Phones);           
        }

        public static PersonInfoViewData Create(PersonDetails person)
        {
            return new PersonInfoViewData(person);
        }

        private static int CalculateAge(DateTime birthDate)
        {
            var age = 0;
            while ((birthDate = birthDate.AddYears(1)) < DateTime.UtcNow)
                age++;

            return age;
        }   
    }
}