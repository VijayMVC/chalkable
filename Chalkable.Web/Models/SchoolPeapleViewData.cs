using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class SchoolPeapleViewData : SchoolInfoViewData
    {
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int StaffsCount { get; set; }
        public int InvitesCount { get; set; }

        public PaginatedList<PersonViewData> Persons { get; set; }

        protected SchoolPeapleViewData(School school, SisSync sisData, int studentsCount, int teachersCount, int saffsCount) 
            : base(school, sisData)
        {
            StudentsCount = studentsCount;
            TeachersCount = teachersCount;
            StaffsCount = saffsCount;
        }

        public static SchoolPeapleViewData Create(School school, SisSync sisData, int studentsCount, int teachersCount, int staffsCount)
        {
            return new SchoolPeapleViewData(school, sisData, studentsCount, teachersCount, staffsCount);
        }
    }
}