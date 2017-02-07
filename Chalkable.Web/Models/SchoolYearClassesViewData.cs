using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Models
{
    public class SchoolYearClassesViewData
    {
        public int SchoolId { get; set; }
        public string SchoolName { get; set; }
        public SchoolYearViewData SchoolYear { get; set; }
        public IList<ShortClassViewData> Classes { get; set; }

        public static SchoolYearClassesViewData Create(SchoolYear schoolYear, IList<Class> classes, School school)
        {
            return new SchoolYearClassesViewData
            {
                SchoolYear = SchoolYearViewData.Create(schoolYear),
                Classes = classes.Select(ShortClassViewData.Create).ToList(),
                SchoolId = school.Id,
                SchoolName = school.Name
            };
        }

        public static IList<SchoolYearClassesViewData> Create(IList<SchoolYear> schoolYears, IList<Class> classes, IList<School> schools)
        {
            var res = new List<SchoolYearClassesViewData>();
            IList<Class> classesVDs;

            foreach (var schoolYear in schoolYears)
            {
                var school = schools.FirstOrDefault(x => x.Id == schoolYear.SchoolRef);
                if(school == null) continue;
                classesVDs = classes.Where(x => x.SchoolYearRef.HasValue && x.SchoolYearRef.Value == schoolYear.Id).ToList();
                res.Add(Create(schoolYear, classesVDs, school));
            }

            return res;
        } 
    }
}