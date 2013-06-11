using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.Master.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        [NotDbFieldAttr]
        public IList<SchoolUser> SchoolUsers { get; set; }
        public bool IsSysAdmin { get; set;}
        public bool IsDeveloper { get; set; }
    }

    public class School
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ServerUrl { get; set; }
        public Guid DistrictRef { get; set; }
    }

    public class SchoolUser
    {
        public Guid Id { get; set; }
        public Guid UserRef { get; set; }
        public Guid SchoolRef { get; set; }
        public int Role { get; set; }
        [DataEntityAttr]
        public User User { get; set; }
        [DataEntityAttr]
        public School School { get; set; }
    }

}
