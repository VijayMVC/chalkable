using System;
using System.Linq;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class AnnouncementType
    {
        public const string ID_FIELD = "Id";
        public const string GRADABLE_FIELD_NAME = "Gradable";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public bool IsSystem { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Gradable { get; set; }
        public int Percentage { get; set; }

        [NotDbFieldAttr]
        public bool CanCreate { get; set; }
        [NotDbFieldAttr]
        public SystemAnnouncementType SystemType
        {
            get
            {
                var list = Enum.GetValues(typeof(SystemAnnouncementType)).OfType<SystemAnnouncementType>().Select(x => (int)x);
                if (list.Any(x => (x) == Id))
                    return (SystemAnnouncementType)Id;
                return SystemAnnouncementType.Custom;
            }
        }

    }

    public enum SystemAnnouncementType
    {
        Custom = 0,
        Standard = 1,
        HW = 2,
        Essay = 3,
        Quiz = 4,
        Test = 5,
        Project = 6,
        Final = 7,
        Midterm = 8,
        BookReport = 9,
        TermPaper = 10,
        Admin = 11
    }
}
