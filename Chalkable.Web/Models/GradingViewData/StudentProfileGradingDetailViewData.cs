using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.GradingViewData
{
    public class StudentProfileGradingDetailViewData : StudentProfileViewData
    {
        public GradingPeriodViewData CurrentGradingPeriod { get; set; }
        public IList<ClassAvgViewData> ClassAvgs { get; set; } 

        protected StudentProfileGradingDetailViewData(Student person, IList<StudentCustomAlertDetail> customAlerts,
            IList<StudentHealthCondition> healthConditions, IList<StudentHealthFormInfo> healthForms)
            : base(person, customAlerts, healthConditions, healthForms)
        {
        }

        public static StudentProfileGradingDetailViewData Create(StudentGradingDetails gradingDetails
            , IList<StudentCustomAlertDetail> customAlerts, IList<StudentHealthCondition> healthConditions,
            IList<ClaimInfo> claims, IList<StudentHealthFormInfo> healthForms)
        {

            var res = new StudentProfileGradingDetailViewData(gradingDetails.Student, customAlerts, healthConditions, healthForms)
            {
                CurrentGradingPeriod = GradingPeriodViewData.Create(gradingDetails.GradingPeriod),
                ClassAvgs = ClassAvgViewData.Create(gradingDetails.GradingsByClass, gradingDetails.Student.Id, claims)
            };
            return res;
        }
    }

    public class ClassCategoryAvgViewData
    {
        public IList<ShortAnnouncementGradeViewData> Items { get; set; }
        public decimal? Avg { get; set; }
        public ClassAnnouncementTypeViewData AnnouncementType { get; set; }
    }

    public class ClassAvgViewData
    {
        public decimal? Avg { get; set; }
        public int? ClassId { get; set; }
        public IList<ClassCategoryAvgViewData> Items { get; set; }

        public static IList<ClassAvgViewData> Create(IList<StudentGradingByClass> studentGradingsByClass, int studentId, IList<ClaimInfo> claims)
        {
            return studentGradingsByClass.Select(gragingByClass => new ClassAvgViewData
            {
                Avg = gragingByClass.Avg,
                ClassId = gragingByClass.ClassId,
                Items = gragingByClass.GradingsByAnnType.Select(gradingByType => new ClassCategoryAvgViewData
                {
                    AnnouncementType = ClassAnnouncementTypeViewData.Create(gradingByType.AnnouncementType),
                    Avg = gradingByType.Avg,
                    Items = gradingByType.ClassAnnouncements.Select(ca =>
                    {
                        var stAnns = gradingByType.StudentAnnouncements.Where(sa => sa.ActivityId == ca.SisActivityId).ToList();
                        return ShortAnnouncementGradeViewData.Create(ca, stAnns, studentId, claims);
                    }).ToList()
                }).ToList()
            }).ToList();
        } 
    }
}