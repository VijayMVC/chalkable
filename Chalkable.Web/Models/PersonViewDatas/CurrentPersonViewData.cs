using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class CurrentPersonViewData : PersonViewData
    {
        public int CurrentSchoolYearId { get; set; }
        public int SchoolLocalId { get; set; }
        public Guid DistrictId { get; set; }
        public string DistrictTimeZone { get; set; }

        protected CurrentPersonViewData(Person person, Guid districtId, int schoolYearId, int schoolLocalId, string districtTimeZone)
            : base(person)
        {
            CurrentSchoolYearId = schoolYearId;
            SchoolLocalId = schoolLocalId;
            DistrictId = districtId;
            DistrictTimeZone = districtTimeZone;
        }

        public static CurrentPersonViewData Create(Person person, Guid districtId, int schoolYearId, int schoolLocalId, string districtTimeZone)
        {
            return new CurrentPersonViewData(person, districtId, schoolYearId, schoolLocalId, districtTimeZone);
        }
    }
}