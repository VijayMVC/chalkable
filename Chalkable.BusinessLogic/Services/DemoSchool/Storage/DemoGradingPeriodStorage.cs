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

        private IList<GradingPeriod> Convert(IEnumerable<GradingPeriod> gps)
        {
            return gps.Select(gp => new GradingPeriod
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
            }).ToList();
        } 

        public IList<GradingPeriod> GetGradingPeriodsDetails(GradingPeriodQuery query)
        {

            var gps = data.Select(x => x.Value);

            if (query.GradingPeriodId.HasValue)
                gps = gps.Where(x => x.Id == query.GradingPeriodId);
            if (query.SchoolYearId.HasValue)
                gps = gps.Where(x => x.SchoolYearRef == query.SchoolYearId);
            if (query.ClassId.HasValue)
                gps = gps.Where(x => Storage.MarkingPeriodClassStorage.GetByClassId(query.ClassId).Any(y => y.MarkingPeriodRef == x.MarkingPeriodRef));

            return Convert(gps.ToList());
        }
    }
}
