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
