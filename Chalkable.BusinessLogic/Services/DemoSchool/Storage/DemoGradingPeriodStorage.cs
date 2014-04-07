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
    }
}
