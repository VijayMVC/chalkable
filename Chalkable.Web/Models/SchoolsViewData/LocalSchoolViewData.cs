

using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model.Sis;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class LocalSchoolViewData
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public static LocalSchoolViewData Create(School school)
        {
            return new LocalSchoolViewData
            {
                Id = school.Id,
                Name = school.Name
            };
        }

        public static IList<LocalSchoolViewData> Create(IList<School> schools)
        {
            return schools.Select(Create).ToList();
        }
    }
}