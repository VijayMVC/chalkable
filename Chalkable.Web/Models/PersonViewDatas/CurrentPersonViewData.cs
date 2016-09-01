using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Common.Web;
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
        public string StateCode { get; set; }
        public string Email { get; set; }

        public IList<PersonClaimViewData> Claims { get; set; } 

        protected CurrentPersonViewData(PersonDetails person, District district, School school, SchoolYear schoolYear, IList<ClaimInfo> claimInfos)
            : base(person)
        {
            CurrentSchoolYearId = schoolYear.Id;
            CurrentSchoolYearName = schoolYear.Name;

            SchoolLocalId = school.LocalId;
            SchoolName = school.Name;
            DistrictId = district.Id;
            DistrictTimeZone = DateTimeTools.WindowsToIana(district.TimeZone);
            DistrictName = district.Name;
            StateCode = district.StateCode;
            Email = person.Email;
            Claims = PersonClaimViewData.Create(claimInfos);
        }


        public static CurrentPersonViewData Create(PersonDetails person, District district, School school, SchoolYear schoolYear, IList<ClaimInfo> claims)
        {
            return new CurrentPersonViewData(person, district, school, schoolYear, claims);
        }
    }
}