using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.DisciplinesViewData;

namespace Chalkable.Web.Logic
{
    public class ClassLogic
    {
        public static async Task<ClassAttendanceSummaryViewData> GetClassAttendanceSummary(int classId, DatePeriodTypeEnum datePeriodType, IServiceLocatorSchool serviceLocator)
        {
            DateTime startDate, endDate;
            var handler = GetDailStatsHandler(datePeriodType);
            handler.GetDateRange(out startDate, out endDate, serviceLocator);
            var attSummaries = await serviceLocator.AttendanceService.GetDailyAttendanceSummaries(classId, startDate, endDate);
            var absences = handler.BuildDailyStats(attSummaries.ToDictionary(x => x.Date, x => x.Absences ?? 0), startDate, endDate);
            var lates = handler.BuildDailyStats(attSummaries.ToDictionary(x => x.Date, x => (decimal)(x.Tardies ?? 0)), startDate, endDate);
            var presents = handler.BuildDailyStats(attSummaries.ToDictionary(x => x.Date, x => x.Presents ?? 0), startDate, endDate);
            return ClassAttendanceSummaryViewData.Create(classId, datePeriodType, absences, lates, presents);
        }

        public static async Task<ClassDisciplineSummaryViewData> GetClassDisciplineSummary(int classId, DatePeriodTypeEnum datePeriodType, IServiceLocatorSchool serviceLocator)
        {
            DateTime startDate, endDate;
            var handler = GetDailStatsHandler(datePeriodType);
            handler.GetDateRange(out startDate, out endDate, serviceLocator);
            var disciplineDailySummaries = await serviceLocator.DisciplineService.GetClassDisciplineDailySummary(classId, startDate, endDate);
            var stats = disciplineDailySummaries.ToDictionary(x => x.Date, x => (decimal) x.Occurrences);
            return ClassDisciplineSummaryViewData.Create(classId, datePeriodType, handler.BuildDailyStats(stats, startDate, endDate));
        }


        private static IDictionary<DatePeriodTypeEnum, IDailyStatsHandler> handlers
                    = new Dictionary<DatePeriodTypeEnum, IDailyStatsHandler>
                    {
                        [DatePeriodTypeEnum.Year] = new DailyStatsForYearHandler(),
                        [DatePeriodTypeEnum.GradingPeriod] = new DailyStatsForGradingPeriodHandler(),
                        [DatePeriodTypeEnum.LastMonth] = new DefaultDailyStatsHandler("MMM dd", 30),
                        [DatePeriodTypeEnum.LastWeek] = new DefaultDailyStatsHandler("ddd", 6)
                    };
        private static IDailyStatsHandler GetDailStatsHandler(DatePeriodTypeEnum datePeriodType)
        {
            if(!handlers.ContainsKey(datePeriodType)) 
                throw new ChalkableException("Invalid date period type");
            return handlers[datePeriodType];
        }
        

        public enum DatePeriodTypeEnum
        {
            Year = 0,
            GradingPeriod = 1,
            LastMonth = 2,
            LastWeek = 3
        }        
    }


    public interface IDailyStatsHandler
    {
        void GetDateRange(out DateTime startDate, out DateTime endDate, IServiceLocatorSchool serviceLocator);
        IList<DailyStatsViewData> BuildDailyStats(IDictionary<DateTime, decimal> dailyStats, DateTime startDate, DateTime endDate);
    }
    
    public class DefaultDailyStatsHandler : IDailyStatsHandler
    {
        private int lastDaysCount;
        private string dateFormat;

        public DefaultDailyStatsHandler(string dateFormat, int lastDaysCount)
        {
            this.dateFormat = dateFormat;
            this.lastDaysCount = lastDaysCount;
        }

        public void GetDateRange(out DateTime startDate, out DateTime endDate, IServiceLocatorSchool serviceLocator)
        {
            startDate = serviceLocator.Context.NowSchoolYearTime.AddDays(-lastDaysCount).Date;
            endDate = serviceLocator.Context.NowSchoolYearTime.Date;
        }

        public IList<DailyStatsViewData> BuildDailyStats(IDictionary<DateTime, decimal> dailyStats, DateTime startDate, DateTime endDate)
        {
            var currentDate = startDate.Date;
            var res = new List<DailyStatsViewData>();
            while (currentDate <= endDate)
            {
                if (dailyStats.ContainsKey(currentDate))
                {
                    var sum = dailyStats[currentDate];
                    res.Add(DailyStatsViewData.Create(currentDate, sum, dateFormat));
                }
                currentDate = currentDate.AddDays(1).Date;
            }
            return res;
        }
    }

    public class DailyStatsForYearHandler : IDailyStatsHandler
    {
        public void GetDateRange(out DateTime startDate, out DateTime endDate, IServiceLocatorSchool serviceLocator)
        {
            var sy = serviceLocator.SchoolYearService.GetCurrentSchoolYear();
            if(!sy.StartDate.HasValue)
                throw new ChalkableException("Current school year has no date range ");
            startDate = sy.StartDate.Value;
            endDate = serviceLocator.Context.NowSchoolYearTime.Date;
        }

        public IList<DailyStatsViewData> BuildDailyStats(IDictionary<DateTime, decimal> dailyStats, DateTime startDate, DateTime endDate)
        {
            string dateFormat = "MMM";
            var res  =  new List<DailyStatsViewData>();
            var currentDate = startDate.Date;
            decimal sum = 0;

            while (currentDate <= endDate.Date)
            {
                sum += dailyStats.ContainsKey(currentDate) ? dailyStats[currentDate] : 0;
                if (currentDate.AddDays(-1).Month != currentDate.Month || currentDate == endDate.Date)
                {
                    res.Add(DailyStatsViewData.Create(currentDate.AddDays(-1), sum, dateFormat));
                    sum = 0;
                }
                currentDate = currentDate.AddDays(1).Date;
            }
            return res;
        }
    }

    public class DailyStatsForGradingPeriodHandler : IDailyStatsHandler
    {
        public void GetDateRange(out DateTime startDate, out DateTime endDate, IServiceLocatorSchool serviceLocator)
        {
            Trace.Assert(serviceLocator.Context.SchoolYearId.HasValue);
            var gp = serviceLocator.GradingPeriodService.GetGradingPeriodDetails(serviceLocator.Context.SchoolYearId.Value, serviceLocator.Context.NowSchoolYearTime);
            startDate = gp.StartDate;
            endDate = serviceLocator.Context.NowSchoolYearTime.Date;
        }

        public IList<DailyStatsViewData> BuildDailyStats(IDictionary<DateTime, decimal> dailyStats, DateTime startDate, DateTime endDate)
        {
            decimal sum = 0;
            var res = new List<DailyStatsViewData>();
            var currentDate = startDate.Date;
            var week = 1;
            var isWeekStarted = false;
            
            while (currentDate <= endDate.Date)
            {
                isWeekStarted = true;
                sum += dailyStats.ContainsKey(currentDate) ? dailyStats[currentDate] : 0;
                if (currentDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    res.Add(new DailyStatsViewData {Date = currentDate, Number = sum, Summary = $"Week {week}"});
                    week++;
                    sum = 0;
                    isWeekStarted = false;
                }
                currentDate = currentDate.AddDays(1).Date;
            }

            if(isWeekStarted)
                res.Add(new DailyStatsViewData { Date = currentDate, Number = sum, Summary = $"Week {week}" });

            return res;
        }
    }
    
}