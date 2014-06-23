using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class DistrictViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DbName { get; set; }
        
        public int SisSystemType { get; set; }
        public string SisUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
        
        protected DistrictViewData(District district)
        {
            Id = district.Id;
            Name = district.Name;
            DbName = district.DbName;
            SisUrl = district.SisUrl;
            SisUserName = district.SisUserName;
        }

        public static DistrictViewData Create(District school)
        {
            return new DistrictViewData(school);
        }
        public static IList<DistrictViewData> Create(IList<District> districts)
        {
            return districts.Select(Create).ToList();
        } 
    }

    public class DistrictRegisterViewData
    {
        /*
         * linkKey: "4d2d35a9-c14d-478f-b949-9dd92316060c",
  districtGuid: "59295830-5853-4a22-9585-9768145ae3ff",
  apiUrl: "http://sandbox.sti-k12.com/chalkable/api/",
  sisUsername: "Chalkable",
  sisPassword: "R8bW2yF8h",
  districtTimeZone: "Central Standard Time",
  userName: "informationnow",
  password: "somepassword"
         */
        public Guid LinkKey { get; set; }
        public Guid DistrictGuid { get; set; }
        public string ApiUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string SisUserName { get; set; }
        public string SisPassword { get; set; }
        public string DistrictTimeZone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

}