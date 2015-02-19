using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Models
{
    public class ShortAutoGradeViewData
    {
        public int AnnouncementApplicationId { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public bool Posted { get; set; }
        public string Grade { get; set; }

        protected ShortAutoGradeViewData(AutoGrade autoGrade)
        {
            AnnouncementApplicationId = autoGrade.AnnouncementApplicationRef;
            StudentId = autoGrade.StudentRef;
            Date = autoGrade.Date;
            Grade = autoGrade.Grade;
            Posted = autoGrade.Posted;
        }
    }

    public class AutoGradeViewData : ShortAutoGradeViewData
    {
        public BaseApplicationViewData Application { get; set; }

        protected AutoGradeViewData(AutoGrade autoGrade) : base(autoGrade)
        {
        }
        public static AutoGradeViewData Create(AutoGrade autoGrade, BaseApplicationViewData application)
        {
            return new AutoGradeViewData(autoGrade) {Application = application};
        }
       
        public static IList<AutoGradeViewData> Create(IList<AutoGrade> autoGrades, IList<Application> applications)
        {
            var res = new List<AutoGradeViewData>();
            foreach (var autoGrade in autoGrades)
            {
                var app = applications.FirstOrDefault(x=>x.Id == autoGrade.AnnouncementApplication.ApplicationRef);
                res.Add(Create(autoGrade, BaseApplicationViewData.Create(app)));
            }
            return res;
        }
    }
}