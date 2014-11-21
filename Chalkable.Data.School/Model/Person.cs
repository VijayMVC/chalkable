using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Person
    {
        public const string ID_FIELD = "Id";
        public const string FIRST_NAME_FIELD = "FirstName";
        public const string LAST_NAME_FIELD = "LastName";
        public const string GENDER_FIELD = "Gender";
        public const string ROLE_REF_FIELD = "RoleRef";
        public const string SCHOOL_REF_FIELD = "SchoolRef";

        public const string SALUTATION_FIELD = "Salutation";
        public const string ADDRESS_REF_FIELD = "AddressRef";
        //public const string EMAIL_FIELD = "Email";
        
        public const string USER_ID_FIELD = "UserId";

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
        private Address address;
        public Address Address
        {
            get { return address; } 
            set
            {
                address = value;
                if (value != null && value.Id != 0)
                    AddressRef = address.Id;
            }
        }
        public IList<Phone> Phones { get; set; }
        public IList<StudentSchoolYear> StudentSchoolYears { get; set; }
        
      
        public string Email { get; set; }
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
