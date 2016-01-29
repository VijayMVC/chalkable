using System;
using Chalkable.Data.Master.Model;

namespace Chalkable.Web.Models
{
    public class ShortDistrictSummaryViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int StudentsCount { get; set; }
        public int SchoolsCounts { get; set; }

        public static ShortDistrictSummaryViewData Create(District district, int students, int schools)
        {
            return new ShortDistrictSummaryViewData()
            {
                Id = district.Id,
                Name = district.Name,
                StudentsCount = students,
                SchoolsCounts = schools
            };
        }
    }
}