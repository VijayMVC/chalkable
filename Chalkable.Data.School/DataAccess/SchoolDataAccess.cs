using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class SchoolDataAccess : DataAccessBase<Model.School, int>
    {
        public SchoolDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public void Delete(IList<int> ids)
        {
            SimpleDelete(ids.Select(x => new Model.School {Id = x}).ToList());
        }

        public StartupData GetStartupData(int schoolYearId, int personId, int roleId, DateTime now)
        {
            var ps = new Dictionary<string, object>
            {
                {"@schoolYearId", schoolYearId},
                {"@personId", personId},
                {"@roleId", roleId},
                {"@now", now},
            };
            var res = new StartupData();
            IOrderedEnumerable<ScheduleItem> schedule;
            IList<ClassDetails> allClasses;
            IList<GradingPeriod> gps;
            using (var reader = ExecuteStoredProcedureReader("spGetStartupData", ps))
            {
                res.AlphaGrades = reader.ReadList<AlphaGrade>();
                reader.NextResult();
                res.AlternateScores = reader.ReadList<AlternateScore>();
                reader.NextResult();
                res.MarkingPeriods = reader.ReadList<MarkingPeriod>();
                reader.NextResult();
                gps = reader.ReadList<GradingPeriod>();
                reader.NextResult();
                res.Person = PersonDataAccess.ReadPersonDetailsData(reader);
                reader.NextResult();
                allClasses = ClassDataAccess.ReadClasses(reader);
                reader.NextResult();
                res.Rooms = reader.ReadList<Room>();
                reader.NextResult();
                schedule = reader.ReadList<ScheduleItem>().OrderBy(x => x.PeriodOrder).ThenBy(x => x.ClassName);
                reader.NextResult();
                res.SchoolOption = reader.Read() ? reader.Read<SchoolOption>() : null;
                reader.NextResult();
                res.GradingComments = reader.ReadList<GradingComment>();
                reader.NextResult();
                res.AttendanceReasons = AttendanceReasonDataAccess.ReadGetAttendanceReasonResult(reader);
                reader.NextResult();
                reader.Read();
                res.UnshownNotificationsCount = SqlTools.ReadInt32(reader, "UnshownNotificationsCount");
                reader.NextResult();
                res.AlphaGradesForClasses = AlphaGradeDataAccess.ReadAlphaGradesForClasses(reader, allClasses.Select(x=>x.Id).ToList());
                reader.NextResult();
                res.AlphaGradesForClassStandards = AlphaGradeDataAccess.ReadAlphaGradesForClasses(reader, allClasses.Select(x=>x.Id).ToList());
                reader.NextResult();
                res.AlphaGradesForSchoolStandards = reader.ReadList<AlphaGradeDataAccess.SchoolAlphaGrade>();
                
            }
            res.GradingPeriod = gps.FirstOrDefault(x => x.StartDate <= now && x.EndDate >= now);
            if (res.GradingPeriod == null)
                res.GradingPeriod = gps.OrderByDescending(x => x.StartDate).FirstOrDefault();

            var todayClasses = new List<ClassDetails>();
            foreach (var classPeriod in schedule)
            {
                var c = allClasses.FirstOrDefault(x => x.Id == classPeriod.ClassId);
                if (c != null && todayClasses.All(x => x.Id != c.Id))
                    todayClasses.Add(c);
            }
            var otherClasses = allClasses.Where(x => todayClasses.All(y => y.Id != x.Id)).OrderBy(x => x.Name).ToList();
            res.Classes = todayClasses.Concat(otherClasses).ToList();


            //res.AlphaGradesForClasses = new Dictionary<int, IList<AlphaGrade>>();
            //res.AlphaGradesForClassStandards = new Dictionary<int, IList<AlphaGrade>>();

            //foreach (var classDetail in allClasses)
            //{
            //    res.AlphaGradesForClasses.Add(classDetail.Id, new List<AlphaGrade>());
            //    res.AlphaGradesForClassStandards.Add(classDetail.Id, new List<AlphaGrade>());
            //}
            //var agDic = res.AlphaGrades.ToDictionary(x => x.Id);
            //foreach (var classAlphaGrade in agForClasses)
            //{
            //    res.AlphaGradesForClasses[classAlphaGrade.ClassId].Add(agDic[classAlphaGrade.AlphaGradeId]);
            //}
            //foreach (var classAlphaGrade in agForClassStandards)
            //{
            //    res.AlphaGradesForClassStandards[classAlphaGrade.ClassId].Add(agDic[classAlphaGrade.AlphaGradeId]);
            //}
            res.AttendanceReasons = res.AttendanceReasons.Where(x => x.AttendanceLevelReasons.Count > 0).ToList();
            return res;
        }

        private const string SP_GET_SHORT_SCHOOL_SUMMARIES = "spGetShortSchoolSummaries";

        public IList<ShortSchoolSummary> GetShortSchoolSummaries(int start, int count, string filter, int? sortType)
        {
            var param = new Dictionary<string, object>()
            {
                ["filter"] = "%" + filter + "%",
                ["start"] = start,
                ["count"] = count,
                ["sortType"] = sortType
            };

            var res = ExecuteStoredProcedureList<ShortSchoolSummary>(SP_GET_SHORT_SCHOOL_SUMMARIES, param);

            return res;
        }

        public int GetShoolsCount(string filter)
        {   
            var param = new Dictionary<string, object>()
            {
                ["filter"] = "%" + filter + "%",
                ["start"] = 0,
                ["count"] = 1
            };

            var res = ExecuteStoredProcedurePaginated<ShortSchoolSummary>(SP_GET_SHORT_SCHOOL_SUMMARIES, param, 0, 1);

            return res.TotalCount;
        }
    }

    
}