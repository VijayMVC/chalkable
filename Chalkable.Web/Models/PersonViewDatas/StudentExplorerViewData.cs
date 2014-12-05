using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ClassesViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentExplorerViewData
    {
        public ShortPersonViewData Student { get; set; }
        public IList<StudentClassExplorerViewData> ClassesGradingInfo { get; set; } 

        public static StudentExplorerViewData Create(StudentExplorerInfo studentExplorerInfo)
        {
            return new StudentExplorerViewData
                {
                    Student = StudentViewData.Create(studentExplorerInfo.Student),
                    ClassesGradingInfo = StudentClassExplorerViewData.Create(studentExplorerInfo.ClassesGradingInfo)
                };
        }
    }

    public class StudentClassExplorerViewData
    {
        public ShortClassViewData Class { get; set; }
        public decimal? Avg { get; set; }
        public AnnouncementShortViewData ImportantAnnouncement { get; set; }
        public IList<StandardGradingItemViewData> Standards { get; set; }

        public static StudentClassExplorerViewData Create(StudentClassExplorerInfo classExplorerInfo)
        {
            var res = new StudentClassExplorerViewData();
            if (classExplorerInfo.ClassInfo != null)
                res.Class = ShortClassViewData.Create(classExplorerInfo.ClassInfo);
            res.Avg = classExplorerInfo.Avg;
            if (classExplorerInfo.MostImportantAnnouncement != null)
                res.ImportantAnnouncement = AnnouncementShortViewData.Create(classExplorerInfo.MostImportantAnnouncement);
            res.Standards = classExplorerInfo.Standards.Select(StandardGradingItemViewData.Create).ToList();
            return res;
        }

        public static IList<StudentClassExplorerViewData> Create(IList<StudentClassExplorerInfo> classExplorerInfos)
        {
            return classExplorerInfos.Select(Create).ToList();
        } 
    }
}