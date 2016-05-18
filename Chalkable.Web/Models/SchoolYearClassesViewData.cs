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
        public SchoolYearViewData SchoolYear { get; set; }
        public IList<ShortClassViewData> Classes { get; set; }

        public static SchoolYearClassesViewData Create(SchoolYear schoolYear, IList<Class> classes)
        {
            return new SchoolYearClassesViewData
            {
                SchoolYear = SchoolYearViewData.Create(schoolYear),
                Classes = classes.Select(ShortClassViewData.Create).ToList()
            };
        }

        public static IList<SchoolYearClassesViewData> Create(IList<SchoolYear> schoolYears, IList<Class> classes)
        {
            var res = new List<SchoolYearClassesViewData>();
            IList<Class> classesVDs;

            foreach (var schoolYear in schoolYears)
            {
                classesVDs = classes.Where(x => x.SchoolYearRef.HasValue && x.SchoolYearRef.Value == schoolYear.Id).ToList();
                res.Add(Create(schoolYear, classesVDs));
            }

            return res;
        } 
    }
}