using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGradingPeriodService : DemoSchoolServiceBase, IGradingPeriodService
    {
        public DemoGradingPeriodService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<GradingPeriodDetails> GetGradingPeriodsDetails(int schoolYearId, int? markingPeriodId = null)
        {
            return Storage.GradingPeriodStorage.GetGradingPeriodDetails(schoolYearId, markingPeriodId);
        }

        public GradingPeriodDetails GetGradingPeriodDetails(int schoolYearId, DateTime tillDate)
        {
            return Storage.GradingPeriodStorage.GetGradingPeriodDetails(schoolYearId, tillDate);
        }

        public GradingPeriodDetails GetGradingPeriodById(int id)
        {
            var gp = Storage.GradingPeriodStorage.GetById(id);
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
                StartDate = gp.StartDate,
                SchoolAnnouncement = gp.SchoolAnnouncement,
                MarkingPeriod = Storage.MarkingPeriodStorage.GetById(gp.MarkingPeriodRef)
            };

            return gpDetails;
        }

        public void Add(IList<GradingPeriod> gradingPeriods)
        {
            Storage.GradingPeriodStorage.Add(gradingPeriods);
        }

        public void Edit(IList<GradingPeriod> gradingPeriods)
        {
            Storage.GradingPeriodStorage.Update(gradingPeriods);
        }

        public void Delete(IList<int> ids)
        {
            Storage.GradingPeriodStorage.Delete(ids);
        }

       
    }
}
