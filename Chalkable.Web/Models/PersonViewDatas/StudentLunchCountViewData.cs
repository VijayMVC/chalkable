using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentLunchCountViewData : StudentViewData
    {
        public StudentLunchCountViewData(StudentLunchCount studentLunchCount) : base(studentLunchCount)
        {
            IsAbsent = studentLunchCount.IsAbsent;
        }

        public bool IsAbsent { get; set; }

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