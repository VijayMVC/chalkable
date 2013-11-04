using System;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class ShortSchoolViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int StatusNumber { get; set; }
        
        protected ShortSchoolViewData(School school)
        {
            Id = school.Id;
            Name = school.Name;
        }
        public static ShortSchoolViewData Create(School school)
        {
            return new ShortSchoolViewData(school);
        }
        public static ShortSchoolViewData Create(Guid id, string name, string timeZone, string prefix)
        {
            return Create(new School
            {
                Id = id,
                Name = name
            });
        }
    }
}