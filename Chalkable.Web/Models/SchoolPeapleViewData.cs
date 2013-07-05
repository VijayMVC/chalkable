using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class SchoolPeapleViewData : SchoolViewData
    {
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int StaffsCount { get; set; }
        public int InvitesCount { get; set; }

        public PaginatedList<PersonViewData> Persons { get; set; }

        protected SchoolPeapleViewData(School school, int studentsCount, int teachersCount
            , int saffsCount, PaginatedList<PersonViewData> persons) : base(school)
        {
            StudentsCount = studentsCount;
            TeachersCount = teachersCount;
            StaffsCount = saffsCount;
            Persons = persons;
        }

        public static SchoolPeapleViewData Create(School school, int studentsCount, int teachersCount
               , int staffsCount, PaginatedList<PersonViewData> persons)
        {
            return new SchoolPeapleViewData(school, studentsCount, teachersCount, staffsCount, persons);
        }
    }
}