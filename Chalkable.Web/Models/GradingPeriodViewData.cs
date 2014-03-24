using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{

    public class GradingPeriodViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int MarkingPeriodRef { get; set; }
        public int SchoolYearRef { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        public string SchoolAnnouncement { get; set; }
        public bool AllowGradePosting { get; set; } 


        private GradingPeriodViewData(GradingPeriod gradingPeriod)
        {
            Id = gradingPeriod.Id;
            Name = gradingPeriod.Name;
            Code = gradingPeriod.Code;
            Description = gradingPeriod.Description;
            MarkingPeriodRef = gradingPeriod.MarkingPeriodRef;
            SchoolYearRef = gradingPeriod.SchoolYearRef;
            StartDate = gradingPeriod.StartDate;
            EndDate = gradingPeriod.EndDate;
            EndTime = gradingPeriod.EndTime;
            SchoolAnnouncement = gradingPeriod.SchoolAnnouncement;
            AllowGradePosting = gradingPeriod.AllowGradePosting;
        }

        public static GradingPeriodViewData Create(GradingPeriod gradingPeriod)
        {
            return new GradingPeriodViewData(gradingPeriod);
        }

        public static IList<GradingPeriodViewData> Create(IList<GradingPeriod> gradingPeriods)
        {
            return gradingPeriods.Select(Create).ToList();
        } 
    }
}