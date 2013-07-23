using System;

namespace Chalkable.Data.School.Model
{
    public class PersonsForApplicationInstall
    {
        public int Type { get; set; }

        public string GroupId { get; set; }

        public Guid SchoolPersonId { get; set; }
    }

    public class PersonsForApplicationInstallCount
    {
        public int Type { get; set; }

        public string GroupId { get; set; }

        public int? Count { get; set; }
    }
}