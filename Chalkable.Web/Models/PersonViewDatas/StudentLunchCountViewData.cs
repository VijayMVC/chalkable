using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentLunchCountViewData : StudentViewData
    {
        public StudentLunchCountViewData(StudentLunchCount studentLunchCount) : base(studentLunchCount)
        {
            IsAbsent = studentLunchCount.IsAbsent;
            HasCustomAlerts = studentLunchCount.HasCustomAlerts;
        }

        public bool IsAbsent { get; set; }
        public bool HasCustomAlerts { get; set; }

        public static StudentLunchCountViewData Create(StudentLunchCount studentLunchCount)
        {
            return new StudentLunchCountViewData(studentLunchCount);
        }

        public static IList<StudentLunchCountViewData> Create(IList<StudentLunchCount> studentLunchCounts)
        {
            return studentLunchCounts.Select(Create).ToList();
        }
    }
}