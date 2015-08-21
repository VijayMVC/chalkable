﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.GradingViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentExplorerViewData
    {
        public ShortPersonViewData Student { get; set; }
        public IList<StudentClassExplorerViewData> ClassesGradingInfo { get; set; } 

        public static StudentExplorerViewData Create(StudentExplorerInfo studentExplorerInfo)
        {
            var res =  new StudentExplorerViewData
                {
                    Student = StudentViewData.Create(studentExplorerInfo.Student),
                    ClassesGradingInfo = StudentClassExplorerViewData.Create(studentExplorerInfo.ClassesGradingInfo)
                };
            res.ClassesGradingInfo = res.ClassesGradingInfo
                                    .OrderBy(x => x.Avg.HasValue ? x.Avg : int.MaxValue)
                                    .ThenBy(x=>x.Class.Name).ToList();
            return res;
        }
    }

    public class StudentClassExplorerViewData
    {
        public ShortClassViewData Class { get; set; }
        public decimal? Avg { get; set; }
        public ShortAnnouncementViewData ImportantAnnouncement { get; set; }
        public IList<StudentStandardGradeViewData> Standards { get; set; }

        public static StudentClassExplorerViewData Create(StudentClassExplorerInfo classExplorerInfo)
        {
            var res = new StudentClassExplorerViewData();
            if (classExplorerInfo.ClassInfo != null)
                res.Class = ShortClassViewData.Create(classExplorerInfo.ClassInfo);
            res.Avg = classExplorerInfo.Avg;
            if (classExplorerInfo.MostImportantAnnouncement != null)
                res.ImportantAnnouncement = ClassAnnouncementViewData.Create(classExplorerInfo.MostImportantAnnouncement.ClassAnnouncementData);
            res.Standards = classExplorerInfo.Standards.Select(StudentStandardGradeViewData.Create).ToList();
            return res;
        }

        public static IList<StudentClassExplorerViewData> Create(IList<StudentClassExplorerInfo> classExplorerInfos)
        {
            return classExplorerInfos.Select(Create).ToList();
        } 
    }

    public class StudentStandardGradeViewData : StandardViewData
    {
        public StandardGradingItemViewData StandardGrading { get; set; }

        public StudentStandardGradeViewData(Standard standard) : base(standard) {}

        public static StudentStandardGradeViewData Create(GradingStandardInfo gradingStandard)
        {
            return new StudentStandardGradeViewData(gradingStandard.Standard)
                {
                    StandardGrading = StandardGradingItemViewData.Create(gradingStandard)
                };
        }
    }
}