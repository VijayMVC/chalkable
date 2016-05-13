using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.ApplicationsViewData
{
    public class ApplicationSchoolOptionViewData
    {
        public Guid ApplicationId { get; set; }
        public Guid SchoolId { get; set; }
        public string SchoolName { get; set; }
        public bool Banned { get; set; }

        protected static ApplicationSchoolOptionViewData Create(ApplicationSchoolOption appSchoolOpt, School school)
        {
            return new ApplicationSchoolOptionViewData
            {
                ApplicationId = appSchoolOpt.ApplicationRef,
                SchoolId = appSchoolOpt.SchoolRef,
                Banned = appSchoolOpt.Banned,
                SchoolName = school.Name
            };
        }

        public static IList<ApplicationSchoolOptionViewData> Create(IList<ApplicationSchoolOption> appSchoolOptions,
            IList<School> schools)
        {
            var res = new List<ApplicationSchoolOptionViewData>(appSchoolOptions.Count);

            foreach (var item in appSchoolOptions)
            {
                res.Add(Create(item, schools.First(x => x.Id == item.SchoolRef)));
            }

            return res;
        } 
    }
}