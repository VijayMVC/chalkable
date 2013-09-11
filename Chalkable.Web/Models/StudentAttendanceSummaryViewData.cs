using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class StudentAttendanceSummaryViewData
    {
        public ShortPersonViewData Student { get; set; }
        public IList<AttendanceTotalPerTypeViewData> AttendanceTotalPerType { get; set; }

        public static IList<StudentAttendanceSummaryViewData> Create(IList<ClassAttendanceDetails> attendances)
        {
            var res = new List<StudentAttendanceSummaryViewData>();
            var personsIds = attendances.GroupBy(x => x.Student.Id).Select(x=>x.Key).ToList();
            foreach (var personId in personsIds)
            {
                var records = attendances.Where(x => x.Student.Id == personId).ToList();
                var recDic = records.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.Count());
                res.Add(new StudentAttendanceSummaryViewData
                    {
                        Student = ShortPersonViewData.Create(records.First().Student),
                        AttendanceTotalPerType = AttendanceTotalPerTypeViewData.Create(recDic)
                    });
            }
            return res;
        }
    }
}