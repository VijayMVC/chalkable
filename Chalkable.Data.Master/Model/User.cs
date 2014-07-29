﻿using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{
    public class User
    {
        public const string ID_FIELD = "Id";
        public const string CONFIRMATION_KEY_FIELD = "ConfirmationKey";
        public const string DISTRICT_REF_FIELD = "DistrictRef";
        public const string IS_SYS_ADMIN_FIELD = "IsSysAdmin";
        public const string LOGIN_FIELD = "Login";
        public const string PASSWORD_FIELD = "Password";
        public const string SIS_USER_NAME_FIELD = "SisUserName";
        public const string LOCAL_ID = "LocalId";

        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public int? LocalId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        [NotDbFieldAttr]
        public string OriginalPassword { get; set; }
        [NotDbFieldAttr]
        public IList<SchoolUser> SchoolUsers { get; set; }
        [NotDbFieldAttr]
        public bool IsSchoolUser
        {
            get { return DistrictRef.HasValue && SchoolUsers != null && SchoolUsers.Count > 0; }
        }
        [NotDbFieldAttr]
        public bool IsDemoUser { get; set; }
        public bool IsSysAdmin { get; set;}
        public bool IsDeveloper { get; set; }
        public string ConfirmationKey { get; set; }
        public string SisUserName { get; set; }
        public Guid? DistrictRef { get; set; }
        [NotDbFieldAttr]
        public District District { get; set; }
        public string SisToken { get; set; }
        public DateTime? SisTokenExpires { get; set; }
        [NotDbFieldAttr]
        public bool IsSisUser
        {
            get { return !string.IsNullOrEmpty(SisUserName); }
        }
    }
    
    public class School
    {
        public const string ID_FIELD = "Id";
        public const string DISTRICT_REF_FIELD = "DistrictRef";
        public const string LOCAL_ID_FIELD = "LocalId";
     
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public Guid DistrictRef { get; set; }
        public string Name { get; set; }
        public int LocalId { get; set; }
        [NotDbFieldAttr]
        public District District { get; set; }
    }

    public class SchoolUser
    {
        public const string ROLE_FIELD = "Role";
        public const string SCHOOL_REF_FIELD = "SchoolRef";
        public const string USER_REF_FIELD = "UserRef";

        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public Guid SchoolRef { get; set; }
        public Guid UserRef { get; set; }
        public int Role { get; set; }
        [DataEntityAttr]
        public User User { get; set; }
        [DataEntityAttr]
        public School School { get; set; }
    }
}
