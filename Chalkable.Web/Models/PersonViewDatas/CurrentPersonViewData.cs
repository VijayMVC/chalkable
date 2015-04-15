﻿using System;
using Chalkable.Common;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using School = Chalkable.Data.Master.Model.School;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class CurrentPersonViewData : PersonViewData
    {
        public int CurrentSchoolYearId { get; set; }
        public string CurrentSchoolYearName { get; set; }

        public int SchoolLocalId { get; set; }
        public string SchoolName { get; set; }

        public Guid DistrictId { get; set; }
        public string DistrictTimeZone { get; set; }
        public string DistrictName { get; set; }

        protected CurrentPersonViewData(Person person, District district, School school, SchoolYear schoolYear)
            : base(person)
        {
            CurrentSchoolYearId = schoolYear.Id;
            CurrentSchoolYearName = schoolYear.Name;

            SchoolLocalId = school.LocalId;
            SchoolName = school.Name;
            DistrictId = district.Id;
            DistrictTimeZone = DateTimeTools.WindowsToIana(district.TimeZone);
            DistrictName = district.Name;
        }


        public static CurrentPersonViewData Create(Person person, District district, School school, SchoolYear schoolYear)
        {
            return new CurrentPersonViewData(person, district, school, schoolYear);
        }
    }
}