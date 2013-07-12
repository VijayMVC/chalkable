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
            SisSystemType = district.SisSystemType;
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

}