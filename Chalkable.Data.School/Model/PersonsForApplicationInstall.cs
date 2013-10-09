using System;

namespace Chalkable.Data.School.Model
{
    public class PersonsForApplicationInstall
    {
        public PersonsFroAppInstallTypeEnum Type { get; set; }

        public string GroupId { get; set; }

        public Guid PersonId { get; set; }
    }

    public class PersonsForApplicationInstallCount
    {
        public PersonsFroAppInstallTypeEnum Type { get; set; }

        public string GroupId { get; set; }

        public int? Count { get; set; }
    }

    public class StudentCountToAppInstallByClass
    {
        public Guid ClassId { get; set; }

        public string ClassName { get; set; }

        public int NotInstalledStudentCount { get; set; }
    }

    public enum PersonsFroAppInstallTypeEnum
    {
        Role = 0,
        Department = 1,
        GradeLevel = 2,
        Class = 3,
        Person = 4,
        Total = 5
    }
}