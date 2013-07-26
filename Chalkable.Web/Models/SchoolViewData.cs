using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class SchoolViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? DistrictId { get; set; }
        public string SchoolType { get; set; }
        //public string SchoolUrl { get; set; }
        public string TimeZoneId { get; set; }
        public bool SendEmailNotifications { get; set; }
        public int ImportSystemType { get; set; }
        
        protected SchoolViewData(School school)
        {
            Id = school.Id;
            DistrictId = school.DistrictRef;
            Name = school.Name;
            ImportSystemType = (int) school.ImportSystemType;
            TimeZoneId = school.TimeZone;
        }

        public static SchoolViewData Create(School school)
        {
            return new SchoolViewData(school);
        }
        public static IList<SchoolViewData> Create(IList<School> schools)
        {
            return schools.Select(Create).ToList();
        } 
    }

}