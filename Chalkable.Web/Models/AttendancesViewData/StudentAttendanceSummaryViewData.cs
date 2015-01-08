using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.AttendancesViewData
{
    public class StudentAttendanceSummaryViewData
    {
        public StudentViewData Student { get; set; }
        public IList<AttendanceTotalPerTypeViewData> AttendanceTotalPerType { get; set; }

        public static IList<StudentAttendanceSummaryViewData> Create(IList<ClassAttendanceDetails> attendances)
        {
            var res = new List<StudentAttendanceSummaryViewData>();
            var personsIds = attendances.GroupBy(x => x.Student.Id).Select(x=>x.Key).ToList();
            foreach (var personId in personsIds)
            {
                var records = attendances.Where(x => x.Student.Id == personId).ToList();
                var recDic = records.GroupBy(x => x.Level).ToDictionary(x => x.Key, x => x.Count());
                res.Add(new StudentAttendanceSummaryViewData
                    {
                        Student = StudentViewData.Create(records.First().Student),
                        AttendanceTotalPerType = AttendanceTotalPerTypeViewData.Create(recDic)
                    });
            }
            return res;
        }
    }

    public class AttendanceTotalPerTypeViewData
    {
        public string Level { get; set; }
        public int AttendanceCount { get; set; }

        public static IList<AttendanceTotalPerTypeViewData> Create(IDictionary<string, int> atteDic)
        {
            return atteDic.Select(x => new AttendanceTotalPerTypeViewData
                {
                    AttendanceCount = x.Value,
                    Level = x.Key
                }).ToList();
        }
    }
}