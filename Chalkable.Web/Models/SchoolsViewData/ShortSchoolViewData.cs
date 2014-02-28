using System;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class ShortSchoolViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int StatusNumber { get; set; }
        public Guid DistrictId { get; set; }
        public string DemoPrefix { get; set; }


        protected ShortSchoolViewData(School school)
        {
            Id = school.Id;
            Name = school.Name;
            DistrictId = school.DistrictRef;
            if (school.District != null)
                DemoPrefix = school.District.DemoPrefix;
            
        }
        public static ShortSchoolViewData Create(School school)
        {
            return new ShortSchoolViewData(school);
        }
        public static ShortSchoolViewData Create(Guid id, string name, string timeZone, string prefix)
        {
            var res = Create(new School
            {
                Id = id,
                Name = name
            });
            res.DemoPrefix = prefix;
            return res;
        }
    }
}