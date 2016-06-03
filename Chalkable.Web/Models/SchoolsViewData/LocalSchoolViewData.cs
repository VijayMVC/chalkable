

using System.Collections.Generic;
using System.Linq;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class LocalSchoolViewData
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public static LocalSchoolViewData Create(Data.School.Model.School school)
        {
            return new LocalSchoolViewData
            {
                Id = school.Id,
                Name = school.Name
            };
        }

        public static IList<LocalSchoolViewData> Create(IList<Data.School.Model.School> schools)
        {
            return schools.Select(Create).ToList();
        }
    }
}