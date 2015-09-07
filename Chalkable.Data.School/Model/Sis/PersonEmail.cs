﻿using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class PersonEmail
    {
        public const string PERSON_REF_FIELD = "PersonRef";
        public const string EMAIL_ADDRESS_FIELD = "EmailAddress";

        [PrimaryKeyFieldAttr]
        public int PersonRef { get; set; }
        [PrimaryKeyFieldAttr]
        public string EmailAddress { get; set; }
        public string Description { get; set; }
        public bool IsListed { get; set; }
        public bool IsPrimary { get; set; }
    }
}
