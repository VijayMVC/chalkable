﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
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

            var gps = data.Select(x => x.Value);

            if (query.GradingPeriodId.HasValue)
                gps = gps.Where(x => x.Id == query.GradingPeriodId);
            if (query.SchoolYearId.HasValue)
                gps = gps.Where(x => x.SchoolYearRef == query.SchoolYearId);
            if (query.ClassId.HasValue)
                gps = gps.Where(x => StorageLocator.MarkingPeriodClassStorage.GetByClassId(query.ClassId).Any(y => y.MarkingPeriodRef == x.MarkingPeriodRef));

            return Convert(gps.ToList());
        }
    }

    public class DemoGradingPeriodService : DemoSchoolServiceBase, IGradingPeriodService
    {
        private DemoGradingPeriodStorage GradingPeriodStorage { get; set; }
        public DemoGradingPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
            GradingPeriodStorage = new DemoGradingPeriodStorage();
        }

        public IList<GradingPeriod> GetGradingPeriodsDetails(int schoolYearId, int? classId = null)
        {
            return GradingPeriodStorage.GetGradingPeriodsDetails(new GradingPeriodQuery
            {
                SchoolYearId = schoolYearId,
                ClassId = classId
            });
        }

        public GradingPeriod GetGradingPeriodDetails(int schoolYearId, DateTime date)
        {
            var gps = GradingPeriodStorage.GetGradingPeriodsDetails(new GradingPeriodQuery
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
            return GradingPeriodStorage
                   .GetGradingPeriodsDetails(new GradingPeriodQuery
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

       
    }
}
