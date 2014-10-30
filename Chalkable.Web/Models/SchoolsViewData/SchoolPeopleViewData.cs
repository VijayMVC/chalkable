using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class SchoolPeopleViewData : SchoolInfoViewData
    {
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int StaffsCount { get; set; }
        public int InvitesCount { get; set; }

        public PaginatedList<PersonViewData> Persons { get; set; }

        protected SchoolPeopleViewData(School school, int studentsCount, int teachersCount, int staffsCount) 
            : base(school)
        {
            StudentsCount = studentsCount;
            TeachersCount = teachersCount;
            StaffsCount = staffsCount;
        }

        public static SchoolPeopleViewData Create(School school, int studentsCount, int teachersCount, int staffsCount)
        {
            return new SchoolPeopleViewData(school, studentsCount, teachersCount, staffsCount);
        }
    }
}