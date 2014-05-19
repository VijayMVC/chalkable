using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradingPeriodStorage:BaseDemoIntStorage<GradingPeriod>
    {
        public DemoGradingPeriodStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public override void Setup()
        {
            var currentYear = DateTime.Now.Year;

            var gpList = new List<GradingPeriod>
            {
                new GradingPeriod
                {
                    Id = 1,
                    Name = "Quarter 1",
                    AllowGradePosting = false,
                    Code = "Q1",
                    Description = "",
                    MarkingPeriodRef = 1,
                    SchoolAnnouncement = "",
                    StartDate = new DateTime(currentYear, 1, 21),
                    EndDate = new DateTime(currentYear, 3, 30),
                    EndTime = new DateTime(currentYear, 3, 30, 23, 59, 0),
                    SchoolYearRef = 1
                },
                new GradingPeriod
                {
                    Id = 2,
                    Name = "Quarter 2",
                    AllowGradePosting = false,
                    Code = "Q1",
                    Description = "",
                    MarkingPeriodRef = 1,
                    SchoolAnnouncement = "",
                    StartDate = new DateTime(currentYear, 3, 30),
                    EndDate = new DateTime(currentYear, 5, 30),
                    EndTime = new DateTime(currentYear, 5, 30, 23, 59, 0),
                    SchoolYearRef = 1
                },
                new GradingPeriod
                {
                    Id = 3,
                    Name = "Quarter 3",
                    AllowGradePosting = false,
                    Code = "Q3",
                    Description = "",
                    MarkingPeriodRef = 2,
                    SchoolAnnouncement = "",
                    StartDate = new DateTime(currentYear, 6, 30),
                    EndDate = new DateTime(currentYear, 8, 30),
                    EndTime = new DateTime(currentYear, 8, 30, 23, 59, 0),
                    SchoolYearRef = 1
                },
                new GradingPeriod
                {
                    Id = 4,
                    Name = "Quarter 4",
                    AllowGradePosting = false,
                    Code = "Q4",
                    Description = "",
                    MarkingPeriodRef = 2,
                    SchoolAnnouncement = "",
                    StartDate = new DateTime(currentYear, 8, 30),
                    EndDate = new DateTime(currentYear, 10, 30),
                    EndTime = new DateTime(currentYear, 10, 30, 23, 59, 0),
                    SchoolYearRef = 1
                }
            };

            Add(gpList);
        }


        private IList<GradingPeriodDetails> Convert(IEnumerable<GradingPeriod> gps)
        {
            return gps.Select(gp => new GradingPeriodDetails
            {
                AllowGradePosting = gp.AllowGradePosting,
                Code = gp.Code,
                Description = gp.Description,
                EndDate = gp.EndDate,
                EndTime = gp.EndTime,
                Id = gp.Id,
                MarkingPeriodRef = gp.MarkingPeriodRef,
                Name = gp.Name,
                SchoolYearRef = gp.SchoolYearRef,
                StartDate = gp.StartDate,
                SchoolAnnouncement = gp.SchoolAnnouncement,
                MarkingPeriod = Storage.MarkingPeriodStorage.GetById(gp.MarkingPeriodRef)
            }).ToList();
        } 

        public IList<GradingPeriodDetails> GetGradingPeriodsDetails(GradingPeriodQuery query)
        {

            var gps = data.Select(x => x.Value);

            if (query.GradingPeriodId.HasValue)
                gps = gps.Where(x => x.Id == query.GradingPeriodId);
            if (query.MarkingPeriodId.HasValue)
                gps = gps.Where(x => x.MarkingPeriodRef == query.MarkingPeriodId);
            if (query.SchoolYearId.HasValue)
                gps = gps.Where(x => x.SchoolYearRef == query.SchoolYearId);
            if (query.FromDate.HasValue)
                gps = gps.Where(x => x.StartDate <= query.FromDate);
            if (query.ToDate.HasValue)
                gps = gps.Where(x => x.EndDate >= query.ToDate);


            return Convert(gps.ToList());
        }
    }
}
