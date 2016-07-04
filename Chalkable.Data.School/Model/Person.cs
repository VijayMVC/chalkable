using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Person
    {
        public const string ID_FIELD = nameof(Id);
        public const string FIRST_NAME_FIELD = nameof(FirstName);
        public const string LAST_NAME_FIELD = nameof(LastName);
        public const string GENDER_FIELD = nameof(Gender);
        public const string ROLE_REF_FIELD = nameof(RoleRef);
        public const string SCHOOL_REF_FIELD = nameof(SchoolRef);

        public const string SALUTATION_FIELD = nameof(Salutation);
        public const string ADDRESS_REF_FIELD = nameof(AddressRef);
        public const string BIRTH_DATE = nameof(BirthDate);
        public const string PHOTO_MODIFIED_DATE = nameof(PhotoModifiedDate);
        //public const string EMAIL_FIELD = "Email";
        
        public const string USER_ID_FIELD = "UserId";

        public const string VW_PERSON = "vwPerson";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Salutation { get; set; }
        public bool Active { get; set; }        
        public DateTime? FirstLoginDate { get; set; }
        public DateTime? LastMailNotification { get; set; }
        public int? AddressRef { get; set; }
        public DateTime? PhotoModifiedDate { get; set; }
        public int? UserId { get; set; }

        [NotDbFieldAttr]
        public int RoleRef { get; set; }

        [NotDbFieldAttr]
        public int SchoolRef { get; set; }
    }

    public class PersonDetails : Person
    {
        public IList<Phone> Phones { get; set; }
        public IList<StudentSchoolYear> StudentSchoolYears { get; set; }
        public IList<PersonEmail> PersonEmails { get; set; }

        private Address _address;
        public string Email
        {
            get
            {
                if (PersonEmails == null)
                    return null;
                var res =
                    PersonEmails.FirstOrDefault(x => x.IsPrimary) ??
                    PersonEmails.FirstOrDefault();
                return res?.EmailAddress;
            }
            
        }
        public Address Address
        {
            get { return _address; }
            set
            {
                _address = value;
                if (value != null && value.Id != 0)
                    AddressRef = _address.Id;
            }
        }
    }

    public class StudentHealthCondition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAlert { get; set; }
        public string MedicationType { get; set; }
        public string Treatment { get; set; }
    }
}
