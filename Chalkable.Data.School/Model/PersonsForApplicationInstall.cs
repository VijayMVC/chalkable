using System;

namespace Chalkable.Data.School.Model
{
    public class PersonsForApplicationInstall
    {
        public PersonsForAppInstallTypeEnum Type { get; set; }

        public string GroupId { get; set; }

        public int PersonId { get; set; }
    }

    public class PersonsForApplicationInstallCount
    {
        public PersonsForAppInstallTypeEnum Type { get; set; }

        public string GroupId { get; set; }

        public int? Count { get; set; }
    }

    public class StudentCountToAppInstallByClass
    {
        public int ClassId { get; set; }

        public string ClassName { get; set; }

        public int NotInstalledStudentCount { get; set; }
    }

    public enum PersonsForAppInstallTypeEnum
    {
        Role = 0,
        Department = 1,
        GradeLevel = 2,
        Class = 3,
        Person = 4,
        Total = 5
    }
}