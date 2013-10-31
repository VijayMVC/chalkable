using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{
    public class User
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        [NotDbFieldAttr]
        public IList<SchoolUser> SchoolUsers { get; set; }
        public bool IsSysAdmin { get; set;}
        public bool IsDeveloper { get; set; }
        public const string CONFIRMATION_KEY_FIELD = "ConfirmationKey";
        public string ConfirmationKey { get; set; }
        public int? LocalId { get; set; }
        public const string DISTRICT_REF_FIELD = "DistrictRef";
        public Guid? DistrictRef { get; set; }
        [NotDbFieldAttr]
        public District District { get; set; }
    }

    public class School
    {
        public const string ID_FIELD = "Id";
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public const string DISTRICT_REF_FIELD = "DistrictRef";
        public Guid DistrictRef { get; set; }
        public const string LOCAL_ID_FIELD = "LocalId";
        public int LocalId { get; set; }
        [NotDbFieldAttr]
        public District District { get; set; }
    }

    public class SchoolUser
    {
        [PrimaryKeyFieldAttr]
        public Guid Id { get; set; }
        public Guid UserRef { get; set; }
        public Guid SchoolRef { get; set; }
        public const string ROLE_FIELD = "Role";
        public int Role { get; set; }
        [DataEntityAttr]
        public User User { get; set; }
        [DataEntityAttr]
        public School School { get; set; }
    }
}
