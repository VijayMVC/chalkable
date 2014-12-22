using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class ShortGradingPeriodViewData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int MarkingPeriodId { get; set; }
        public int SchoolYearId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }
        
        protected ShortGradingPeriodViewData(GradingPeriod gradingPeriod)
        {
            Id = gradingPeriod.Id;
            Name = gradingPeriod.Name;
            Code = gradingPeriod.Code;
            Description = gradingPeriod.Description;
            MarkingPeriodId = gradingPeriod.MarkingPeriodRef;
            SchoolYearId = gradingPeriod.SchoolYearRef;
            StartDate = gradingPeriod.StartDate;
            EndDate = gradingPeriod.EndDate;
            EndTime = gradingPeriod.EndTime;
        }
        public static ShortGradingPeriodViewData Create(GradingPeriod gradingPeriod)
        {
            return new ShortGradingPeriodViewData(gradingPeriod);
        }
    }
    
    public class GradingPeriodViewData : ShortGradingPeriodViewData
    {
        public string SchoolAnnouncement { get; set; }
        public bool AllowGradePosting { get; set; } 
        
        private GradingPeriodViewData(GradingPeriod gradingPeriod) : base(gradingPeriod)
        {
            Id = gradingPeriod.Id;
            Name = gradingPeriod.Name;
            Code = gradingPeriod.Code;
            Description = gradingPeriod.Description;
            MarkingPeriodId = gradingPeriod.MarkingPeriodRef;
            SchoolYearId = gradingPeriod.SchoolYearRef;
            StartDate = gradingPeriod.StartDate;
            EndDate = gradingPeriod.EndDate;
            EndTime = gradingPeriod.EndTime;
            SchoolAnnouncement = gradingPeriod.SchoolAnnouncement;
            AllowGradePosting = gradingPeriod.AllowGradePosting;
        }

        public new static GradingPeriodViewData Create(GradingPeriod gradingPeriod)
        {
            return new GradingPeriodViewData(gradingPeriod);
        }

        public static IList<GradingPeriodViewData> Create(IList<GradingPeriod> gradingPeriods)
        {
            return gradingPeriods.Select(Create).ToList();
        } 
    }
}