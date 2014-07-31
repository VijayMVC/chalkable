using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models.SchoolsViewData
{
    public class SchoolViewData : ShortSchoolViewData
    {
        public Guid? DistrictId { get; set; }
        public string SchoolType { get; set; }
        //public string SchoolUrl { get; set; }
        public bool SendEmailNotifications { get; set; }
        //public int ImportSystemType { get; set; }
        
        protected SchoolViewData(School school) : base(school)
        {
            DistrictId = school.DistrictRef;
            //ImportSystemType = (int) ImportSystemType.Sti;
        }

        public static new SchoolViewData Create(School school)
        {
            return new SchoolViewData(school);
        }
        public static IList<SchoolViewData> Create(IList<School> schools)
        {
            return schools.Select(Create).ToList();
        } 
    }

}