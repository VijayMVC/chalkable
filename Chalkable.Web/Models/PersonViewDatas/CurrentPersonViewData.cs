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

        protected CurrentPersonViewData(Person person, Guid districtId, int schoolYearId, int schoolLocalId)
            : base(person)
        {
            CurrentSchoolYearId = schoolYearId;
            SchoolLocalId = schoolLocalId;
            DistrictId = districtId;
        }

        public static new CurrentPersonViewData Create(Person person, Guid districtId, int schoolYearId, int schoolLocalId)
        {
            return new CurrentPersonViewData(person, districtId, schoolYearId, schoolLocalId);
        }
    }
}