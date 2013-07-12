using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class SchoolPeopleViewData : SchoolInfoViewData
    {
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int StaffsCount { get; set; }
        public int InvitesCount { get; set; }

        public PaginatedList<PersonViewData> Persons { get; set; }

        protected SchoolPeopleViewData(School school, SisSync sisData, int studentsCount, int teachersCount, int staffsCount) 
            : base(school, sisData)
        {
            StudentsCount = studentsCount;
            TeachersCount = teachersCount;
            StaffsCount = staffsCount;
        }

        public static SchoolPeopleViewData Create(School school, SisSync sisData, int studentsCount, int teachersCount, int staffsCount)
        {
            return new SchoolPeopleViewData(school, sisData, studentsCount, teachersCount, staffsCount);
        }
    }
}