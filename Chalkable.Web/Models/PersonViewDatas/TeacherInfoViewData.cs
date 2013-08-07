using System;
using Chalkable.Data.School.Model;


namespace Chalkable.Web.Models.PersonViewDatas
{
    public class TeacherInfoViewData : PersonInfoViewData
    {
        protected TeacherInfoViewData(PersonDetails person)
            : base(person)
        {
            Salutation = person.Salutation;
            FullName = person.SalutationName;
            DisplayName = person.ShortSalutationName;
        }
        public static new  TeacherInfoViewData Create(PersonDetails person)
        {
            return new TeacherInfoViewData(person);
        }
    }

    /*
     * Guid personId, IntList addressIndexes, IntList phoneIndexes, string email, 
            string gender, DateTime? birthdayDate, string salutation, string firstName, string lastName
     */
    public class AddressInputModel
    {
        public Guid? Id { get; set; }
        public string Value { get; set; }
        public int Type { get; set; }
    }

    public class PhoneInputModel
    {
        public Guid? Id { get; set; }
        public string Value { get; set; }
        public int Type { get; set; }
        public bool IsPrimary { get; set; }
    }

    public class TeacherInputModel
    {
        public Guid PersonId { get; set; }
        public AddressInputModel[] Addresses { get; set; }
        public PhoneInputModel[] Phones { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthdayDate { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}