﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class SchoolYearViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SchoolId { get; set; }
        
        public static SchoolYearViewData Create(SchoolYear schoolYear)
        {
            var res = new SchoolYearViewData
            {
                Id = schoolYear.Id,
                Name = schoolYear.Name,
                Description = schoolYear.Description,
                StartDate = schoolYear.StartDate,
                EndDate = schoolYear.EndDate,
                SchoolId = schoolYear.SchoolRef
            };
            return res;
        }

        public static IList<SchoolYearViewData> Create(IList<SchoolYear> schoolYears)
        {
            return schoolYears.Select(Create).ToList();
        } 
    }
}