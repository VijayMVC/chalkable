using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class AutoGradeViewData
    {
        public int AnnouncementApplicationId { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public bool Posted { get; set; }
        public string Grade { get; set; }

        protected AutoGradeViewData(AutoGrade autoGrade)
        {
            AnnouncementApplicationId = autoGrade.AnnouncementApplicationRef;
            StudentId = autoGrade.StudentRef;
            Date = autoGrade.Date;
            Grade = autoGrade.Grade;
            Posted = autoGrade.Posted;
        }

        public static IList<AutoGradeViewData> Create(IList<AutoGrade> autoGrades)
        {
            return autoGrades.Select(x=>new AutoGradeViewData(x)).ToList();
        }
    }

}