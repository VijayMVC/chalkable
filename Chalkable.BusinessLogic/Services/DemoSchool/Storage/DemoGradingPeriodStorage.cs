using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradingPeriodStorage:BaseDemoStorage<int, GradingPeriod>
    {
        public DemoGradingPeriodStorage(DemoStorage storage) : base(storage)
        {
        }

        public IList<GradingPeriodDetails> GetGradingPeriodDetails(int schoolYearId, int? markingPeriodId)
        {
            var gradingPeriods =
                data.Where(x => x.Value.SchoolYearRef == schoolYearId && x.Value.MarkingPeriodRef == markingPeriodId).Select(x => x.Value)
                    .ToList();

            return GetGradingPeriodDetailsList(gradingPeriods);
        }

        private IList<GradingPeriodDetails> GetGradingPeriodDetailsList(IEnumerable<GradingPeriod> gradingPeriods)
        {
            var gpDetailsList = new List<GradingPeriodDetails>();

            foreach (var gp in gradingPeriods)
            {
                var gpDetails = new GradingPeriodDetails
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
                    SchoolAnnouncement = gp.SchoolAnnouncement,
                    StartDate = gp.StartDate
                };

                gpDetails.MarkingPeriod = Storage.MarkingPeriodStorage.GetById(gpDetails.MarkingPeriodRef);
                gpDetailsList.Add(gpDetails);
            }
            return gpDetailsList;
        }

        public void Add(IList<GradingPeriod> gradingPeriods)
        {
            foreach (var gradingPeriod in gradingPeriods)
            {
                if (!data.ContainsKey(gradingPeriod.Id))
                    data[gradingPeriod.Id] = gradingPeriod;
            }
        }

        public void Update(IList<GradingPeriod> gradingPeriods)
        {
            foreach (var gradingPeriod in gradingPeriods)
            {
                data[gradingPeriod.Id] = gradingPeriod;
            }
        }

        public GradingPeriodDetails GetGradingPeriodDetails(int schoolYearId, DateTime tillDate)
        {
            var gradingPeriods =
                data.Where(x => x.Value.SchoolYearRef == schoolYearId && x.Value.EndDate <= tillDate).Select(x => x.Value)
                    .ToList();
            return GetGradingPeriodDetailsList(gradingPeriods).First();
        }

        public void Setup()
        {
            var gpList = new List<GradingPeriod>();

            gpList.Add(new GradingPeriod
            {
                Id = 1,
                Name = "Quarter 1",
                AllowGradePosting = false,
                Code = "Q1",
                Description = "",
                MarkingPeriodRef = 1,
                SchoolAnnouncement = "",
                StartDate = new DateTime(2014, 1, 21),
                EndDate = new DateTime(2014, 3, 30),
                EndTime = new DateTime(2014, 3, 30, 23, 59, 0),
                SchoolYearRef = 12
            });

            gpList.Add(new GradingPeriod
            {
                Id = 2,
                Name = "Quarter 2",
                AllowGradePosting = false,
                Code = "Q1",
                Description = "",
                MarkingPeriodRef = 1,
                SchoolAnnouncement = "",
                StartDate = new DateTime(2014, 3, 30),
                EndDate = new DateTime(2014, 5, 30),
                EndTime = new DateTime(2014, 5, 30, 23, 59, 0),
                SchoolYearRef = 12
            });

            gpList.Add(new GradingPeriod
            {
                Id = 3,
                Name = "Quarter 3",
                AllowGradePosting = false,
                Code = "Q3",
                Description = "",
                MarkingPeriodRef = 2,
                SchoolAnnouncement = "",
                StartDate = new DateTime(2014, 6, 30),
                EndDate = new DateTime(2014, 8, 30),
                EndTime = new DateTime(2014, 8, 30, 23, 59, 0),
                SchoolYearRef = 12
            });

            gpList.Add(new GradingPeriod
            {
                Id = 4,
                Name = "Quarter 4",
                AllowGradePosting = false,
                Code = "Q4",
                Description = "",
                MarkingPeriodRef = 2,
                SchoolAnnouncement = "",
                StartDate = new DateTime(2014, 8, 30),
                EndDate = new DateTime(2014, 10, 30),
                EndTime = new DateTime(2014, 10, 30, 23, 59, 0),
                SchoolYearRef = 12
            });

            Add(gpList);
        }
    }
}
