using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Mapping;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models.PersonViewDatas
{
    public class StudentGradingViewData : ShortPersonViewData
    {
        protected StudentGradingViewData(Person person) : base(person)
        {
        }

        public IList<ClassPersonGradingStatsViewData> StudentGradings { get; set; }

        public static StudentGradingViewData Create(Person person, IList<ClassPersonGradingStats> gradingStats, IGradingStyleMapper mapper)
        {
            var res = new StudentGradingViewData(person);
            var studentGradings = new List<ClassPersonGradingStatsViewData>();
            foreach (var gSt in gradingStats)
            {
                var stGrading = studentGradings.FirstOrDefault(x => x.ClassPersonId == gSt.Id);
                if (stGrading == null)
                {
                    stGrading = new ClassPersonGradingStatsViewData
                    {
                        ClassPersonId = gSt.Id,
                        PersonId = gSt.PersonRef,
                        ClassId = gSt.ClassRef,
                        ClassName = gSt.ClassName,
                        ClassAvg = gSt.ClassAvg,
                        CourseId = gSt.CourseId,
                        StudentAvg = gSt.StudentAvg,
                        ItemTypesStats = new List<AnnouncementTypeGradingStatsViewData>()
                    };
                }
                var annTypeStats = stGrading.ItemTypesStats.FirstOrDefault(x => x.Id == gSt.AnnouncementTypeId);
                if (annTypeStats == null)
                {
                    var annType = new AnnouncementType {Id = gSt.AnnouncementTypeId, Name = gSt.AnnouncementTypeName};
                    annTypeStats = AnnouncementTypeGradingStatsViewData.Create(annType, gSt.StudentItemTypeAvg, gSt.ClassItemTypeAvg);
                    annTypeStats.Items = new List<AnnouncementShortGradeViewData>();
                    stGrading.ItemTypesStats.Add(annTypeStats);
                }
                if (annTypeStats.Items.Any(x=>x.Id == gSt.AnnouncementId))
                {
                    var annComplex = new AnnouncementComplex
                        {
                            Id = gSt.AnnouncementId,
                            Order = gSt.AnnouncementOrder,
                            Dropped = gSt.AnnouncementDropped,
                            Avg = gSt.ItemAvg
                        };
                   annTypeStats.Items.Add(AnnouncementShortGradeViewData.Create(annComplex, mapper));
                }
            }
            return res;
        }
    }

    public class ClassPersonGradingStatsViewData
    {
        public Guid ClassPersonId { get; set; }
        public Guid PersonId { get; set; }
        public Guid ClassId { get; set; }
        public Guid ClassName { get; set; }
        public Guid CourseId { get; set; }
        public int? ClassAvg { get; set; }
        public int? StudentAvg { get; set; }
        public IList<AnnouncementTypeGradingStatsViewData> ItemTypesStats { get; set; }
    }

    public class AnnouncementTypeGradingStatsViewData : AnnouncementTypeViewData
    {
        protected AnnouncementTypeGradingStatsViewData(AnnouncementType announcementType) : base(announcementType)
        {
        }

        public int? StudentItemTypeAvg { get; set; }
        public int? ClassItemTypeAvg { get; set; }
        public IList<AnnouncementShortGradeViewData> Items { get; set; }
 
        public static AnnouncementTypeGradingStatsViewData Create(AnnouncementType annType, int? studentItemTypeAvg, int? classItemTypeAvg)
        {
            return new AnnouncementTypeGradingStatsViewData(annType)
                {
                    StudentItemTypeAvg = studentItemTypeAvg,
                    ClassItemTypeAvg = classItemTypeAvg,
                };
        }

    }
}