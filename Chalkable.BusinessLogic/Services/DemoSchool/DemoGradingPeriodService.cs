using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoGradingPeriodStorage : BaseDemoIntStorage<GradingPeriod>
    {
        public DemoGradingPeriodStorage()
            : base(x => x.Id)
        {
        }
    }

    public class DemoGradingPeriodService : DemoSchoolServiceBase, IGradingPeriodService
    {
        private DemoGradingPeriodStorage GradingPeriodStorage { get; set; }
        public DemoGradingPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            GradingPeriodStorage = new DemoGradingPeriodStorage();
        }

        public IList<GradingPeriod> GetGradingPeriodsDetails(int schoolYearId)
        {
            using (var uow = Read())
            {
                return new GradingPeriodDataAccess(uow).GetGradingPeriodsDetails(new GradingPeriodQuery
                {
                    SchoolYearId = schoolYearId
                });
            }
        }

        public IList<GradingPeriod> GetGradingPeriodsDetailsByClassId(int classId)
        {
            using (var uow = Read())
            {
                return new GradingPeriodDataAccess(uow).GetGradingPeriodsDetails(new GradingPeriodQuery
                {
                    ClassId = classId
                });
            }
        }

        private static IList<GradingPeriod> Convert(IEnumerable<GradingPeriod> gps)
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

            var gps = GradingPeriodStorage.GetData().Select(x => x.Value);

            if (query.GradingPeriodId.HasValue)
                gps = gps.Where(x => x.Id == query.GradingPeriodId);
            if (query.SchoolYearId.HasValue)
                gps = gps.Where(x => x.SchoolYearRef == query.SchoolYearId);
            if (query.ClassId.HasValue)
                gps = gps.Where(x => ((DemoMarkingPeriodService)ServiceLocator.MarkingPeriodService)
                    .GetMarkingPeriodClassById(query.ClassId.Value).Any(y => y.MarkingPeriodRef == x.MarkingPeriodRef));

            return Convert(gps.ToList());
        }

        public GradingPeriod GetGradingPeriodDetails(int schoolYearId, DateTime date)
        {
            var gps = GetGradingPeriodsDetails(new GradingPeriodQuery
            {
                SchoolYearId = schoolYearId
            }); 
            var res = gps.FirstOrDefault(x => x.StartDate <= date && x.EndDate >= date);
            if (res == null)
                res = gps.OrderByDescending(x => x.StartDate).FirstOrDefault();
            return res;
        }

        public GradingPeriod GetGradingPeriodById(int id)
        {
            return GetGradingPeriodsDetails(new GradingPeriodQuery
                   {
                       GradingPeriodId = id
                   }).First();
        }

        public void Add(IList<GradingPeriod> gradingPeriods)
        {
            GradingPeriodStorage.Add(gradingPeriods);
        }

        public void Edit(IList<GradingPeriod> gradingPeriods)
        {
            GradingPeriodStorage.Update(gradingPeriods);
        }

        public void Delete(IList<int> ids)
        {
            GradingPeriodStorage.Delete(ids);
        }

        public IList<GradingPeriod> GetGradingPeriods()
        {
            return GradingPeriodStorage.GetAll();
        }

        public void GetDateRangeByGpID(int? gradingPeriodId, out DateTime startDate, out DateTime endDate)
        {
            if (gradingPeriodId.HasValue)
            {
                var gp = ServiceLocator.GradingPeriodService.GetGradingPeriodById(gradingPeriodId.Value);
                startDate = gp.StartDate;
                endDate = gp.EndDate;
            }
            else
            {
                var sy = ServiceLocator.SchoolYearService.GetCurrentSchoolYear();
                startDate = sy.StartDate.Value;
                endDate = sy.EndDate.Value;
            }
        }
    }
}
