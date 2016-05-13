using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationSchoolOptionViewData
    {
        public Guid SchoolId { get; set; }
        public string SchoolName { get; set; }
        public bool Banned { get; set; }

        protected static ApplicationSchoolOptionViewData Create(ApplicationSchoolOption appSchoolOpt, School school)
        {
            return new ApplicationSchoolOptionViewData
            {
                SchoolId = school.Id,
                Banned = appSchoolOpt != null && appSchoolOpt.Banned,
                SchoolName = school.Name
            };
        }

        public static IList<ApplicationSchoolOptionViewData> Create(IList<ApplicationSchoolOption> appSchoolOptions,
            IList<School> schools)
        {
            return schools.Select(x => Create(appSchoolOptions.FirstOrDefault(y => y.SchoolRef == x.Id), x)).ToList();
        } 
    }
}