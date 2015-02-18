using System;
using System.Collections.Generic;
using System.Linq;
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
        public AnnouncementApplicationViewData Application { get; set; }

        protected AutoGradeViewData(AutoGrade autoGrade) : base(autoGrade)
        {
        }
        public static IList<AutoGradeViewData> Create(IList<AutoGrade> autoGrades, IList<AnnouncementApplicationViewData> announcementApplications)
        {
            var res = new List<AutoGradeViewData>();
            foreach (var autoGrade in autoGrades)
            {
                var annApp = announcementApplications.FirstOrDefault(x=>x.AnnouncementApplicationId == autoGrade.AnnouncementApplicationRef);
                res.Add(new AutoGradeViewData(autoGrade) {Application = annApp});
            }
            return res;
        }
    }
}