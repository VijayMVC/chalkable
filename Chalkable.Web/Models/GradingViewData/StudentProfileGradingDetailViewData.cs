using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.GradingViewData
{
    public class StudentProfileGradingDetailViewData : StudentProfileViewData
    {
        public GradingPeriodViewData CurrentGradingPeriod { get; set; }
        public List<ClassAvgViewData> ClassAvgs { get; set; } 

        protected StudentProfileGradingDetailViewData(Student person, IList<StudentCustomAlertDetail> customAlerts,
            IList<StudentHealthCondition> healthConditions, IList<StudentHealthFormInfo> healthForms)
            : base(person, customAlerts, healthConditions, healthForms)
        {
        }

        public static StudentProfileGradingDetailViewData Create(Student student, StudentGradingDetails gradingDetails, GradingPeriod gp,
            IList<AnnouncementComplex> announcements, IEnumerable<ClassAnnouncementType> classAnnouncementTypes
            , IList<StudentCustomAlertDetail> customAlerts, IList<StudentHealthCondition> healthConditions,
            IList<ClaimInfo> claims, IList<StudentHealthFormInfo> healthForms)
        {
            var classAnnouncementGroups = announcements.GroupBy(x => x.ClassRef).Select(y => new
            {
                ClassId = y.Key,
                Announcements = y.ToList()
            });


            var res = new StudentProfileGradingDetailViewData(student, customAlerts, healthConditions, healthForms)
            {
                CurrentGradingPeriod = GradingPeriodViewData.Create(gp),
                ClassAvgs = new List<ClassAvgViewData>()
            };

            foreach (var classAnnouncementGroup in classAnnouncementGroups)
            {
                var categoryTypes =
                    classAnnouncementGroup.Announcements.GroupBy(x => x.ClassAnnouncementData.ClassAnnouncementTypeRef)
                        .Select(y => new 
                        {
                            AnnouncementType = classAnnouncementTypes.FirstOrDefault(x => x.Id == y.Key),
                            Items = y
                        }).ToList();


                var catTypes = new List<ClassCategoryAvgViewData>();
                foreach (var categoryType in categoryTypes)
                {
                    var ids = categoryType.Items.Select(x => x.ClassAnnouncementData.SisActivityId).Distinct();
                    var studentAnnouncements = gradingDetails.StudentAnnouncements.Where(x => ids.Contains(x.ActivityId)).ToList();
                    var catType = new ClassCategoryAvgViewData()
                    {
                        AnnouncementType = ClassAnnouncementTypeViewData.Create(categoryType.AnnouncementType),
                        Items = categoryType.Items.Select(x => ShortAnnouncementGradeViewData.Create(
                            x.ClassAnnouncementData, 
                            studentAnnouncements.Where(sa=>sa.ActivityId == x.ClassAnnouncementData.SisActivityId).ToList(), 
                            student.Id, claims)).ToList(),
                        Avg = studentAnnouncements.Average(x => x.NumericScore)
                    };
                    catTypes.Add(catType);
                }
                var classAvg = new ClassAvgViewData
                {
                    ClassId = classAnnouncementGroup.ClassId,
                    Items = catTypes,
                    Avg = catTypes.Average(x=>x.Avg)
                };
                res.ClassAvgs.Add(classAvg);
            }
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
    }
}