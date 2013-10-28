using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Person
    {
        private const string GENDER_MALE = "m";
        private const string MR = "Mr. ";
        private const string MS = "Ms. ";

        public const string ID_FIELD = "Id";
        public const string FIRST_NAME_FIELD = "FirstName";
        public const string LAST_NAME_FIELD = "LastName";
        public const string GENDER_FIELD = "Gender";
        public const string ROLE_REF_FIELD = "RoleRef";
        public const string SALUTATION_FIELD = "Salutation";
        public const string ADDRESS_REF_FIELD = "AddressRef";

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Salutation { get; set; }
        public bool Active { get; set; }
        public DateTime? LastPasswordReset { get; set; }
        public DateTime? FirstLoginDate { get; set; }
        public DateTime? LastMailNotification { get; set; }
        public string Email { get; set; }
        public int? AddressRef { get; set; }

        [NotDbFieldAttr]
        public string FullName
        {
            get
            {
                if (HasName)
                {
                    var res = "";
                    if (HasFirstName)
                        res += FirstName + " ";
                    if (HasLastName)
                        res += LastName;
                    return res;
                }
                return Email;
            }
        }
        [NotDbFieldAttr]
        public string SalutationName
        {
            get { return GetSalutation + FullName; }
        }
        [NotDbFieldAttr]
        public string ShortSalutationName
        {
            get { return GetSalutation + (HasLastName ? LastName : FirstName); }
        }

        private string GetSalutation
        {
            get
            {
                string res;
                if (!HasSalutation)
                    res = !string.IsNullOrEmpty(Gender) && Gender.ToLower() == GENDER_MALE ? MR : MS;
                else
                    res = Salutation + (Salutation.EndsWith(".") ? " " : ". ");
                return res;
            }
        }
        private bool HasSalutation
        {
            get { return !(string.IsNullOrEmpty(Salutation) || Salutation.Trim() == ""); }
        }
        private bool HasName
        {
            get { return HasFirstName || HasLastName; }
        }
        private bool HasFirstName
        {
            get { return !string.IsNullOrEmpty(FirstName) && FirstName.Trim() != ""; }
        }
        private bool HasLastName
        {
            get { return !string.IsNullOrEmpty(LastName) && LastName.Trim() != ""; }
        }
    }

    public class PersonDetails : Person
    {
        public Address address;
        public Address Addresses
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
    }
}
