using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class AttendanceSummaryViewData
    {
        public IList<ShortPersonViewData> Trouble { get; set; }
        public IList<ShortPersonViewData> Well { get; set; }
        public IList<AbsentStatForMonthViewData> AbsentStat { get; set; }

        public static AttendanceSummaryViewData Create(IList<Person> trouble, IList<Person> well, IList<ClassAttendanceDetails> attendances, MarkingPeriod mp)
        {
            var res = new AttendanceSummaryViewData();
            res.Trouble = trouble.Select(ShortPersonViewData.Create).ToList();
            res.Well = well.Select(ShortPersonViewData.Create).ToList();
            res.AbsentStat = AbsentStatForMonthViewData.Create(attendances, mp);
            return res;
        }
    }



    public class AbsentStatForMonthViewData
    {
        public List<AbsentStatForClassViewData> AbsentClasses { get; set; }
        public DateTime Month { get; set; }
        private AbsentStatForMonthViewData() { }

        public static IList<AbsentStatForMonthViewData> Create(IList<ClassAttendanceDetails> attendances, MarkingPeriod mp)
        {

            var res = new List<AbsentStatForMonthViewData>();

            DateTime d = mp.StartDate;
            while (d < mp.EndDate)
            {
                var atts = attendances.Where(
                    x => x.Date.Month == d.Month && x.Date.Year == d.Year && x.IsAbsentOrLate).ToList();

                var dic = new Dictionary<Pair<int, string>, int>();
                foreach (var classAttendance in atts)
                {
                    var p = new Pair<int, string>(classAttendance.Class.Id, classAttendance.Class.Name);
                    if (!dic.ContainsKey(p))
                        dic.Add(p, 0);
                    dic[p] = dic[p] + 1;
                }
                var item = new AbsentStatForMonthViewData();
                item.AbsentClasses = new List<AbsentStatForClassViewData>();
                foreach (var pair in dic)
                {
                    var absentSt = AbsentStatForClassViewData.Create(pair.Key.First, pair.Key.Second, pair.Value);
                    item.AbsentClasses.Add(absentSt);
                }
                item.Month = d;
                res.Add(item);

                d = d.AddMonths(1);
            }
            return res;
        }
    }

    public class AbsentStatForClassViewData
    {
        public int AbsentCount { get; set; }
        public string ClassName { get; set; }
        public int ClassId { get; set; }
        private AbsentStatForClassViewData() { }

        public static AbsentStatForClassViewData Create(int classId, string className, int count)
        {
            var res = new AbsentStatForClassViewData();
            res.AbsentCount = count;
            res.ClassName = className;
            res.ClassId = classId;
            return res;
        }
    }
}