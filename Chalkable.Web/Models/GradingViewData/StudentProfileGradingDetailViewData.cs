using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models.GradingViewData
{
    public class StudentProfileGradingDetailViewData : StudentViewData
    {
        public GradingPeriodViewData CurrentGradingPeriod { get; set; }
        public List<ClassAvg> ClassAvgs { get; set; } 

        protected StudentProfileGradingDetailViewData(StudentDetails person)
            : base(person)
        {
        }


        public static StudentProfileGradingDetailViewData Create(StudentDetails student, StudentGradingDetails gradingDetails, GradingPeriod gp,
            IList<AnnouncementComplex> announcements, IEnumerable<ClassAnnouncementType> classAnnouncementTypes)
        {
            var classAnnouncementGroups = announcements.GroupBy(x => x.ClassRef).Select(y => new
            {
                ClassId = y.Key,
                Announcements = y.ToList()
            });


            var res = new StudentProfileGradingDetailViewData(student)
            {
                CurrentGradingPeriod = GradingPeriodViewData.Create(gp),
                ClassAvgs = new List<ClassAvg>()
            };

            foreach (var classAnnouncementGroup in classAnnouncementGroups)
            {
                var categoryTypes =
                    classAnnouncementGroup.Announcements.GroupBy(x => x.ClassAnnouncementData.ClassAnnouncementTypeRef)
                        .Select(y => new ClassCategoryAvg()
                        {
                            AnnouncementType = classAnnouncementTypes.FirstOrDefault(x => x.Id == y.Key),
                            Items = y.Select(x =>  new AnnouncementItem()
                            {
                                Announcement = x
                            }).ToList()
                        }).ToList();

                foreach (var categoryType in categoryTypes)
                {
                    var ids = categoryType.Items.Select(x => x.Announcement.ClassAnnouncementData.SisActivityId).Distinct();
                    var studentAnnouncements =
                        gradingDetails.StudentAnnouncements.Where(x => ids.Contains(x.ActivityId)).ToList();

                    categoryType.Items.ToList().ForEach(
                        x =>
                        {
                            x.StudentAnnouncement =
                                studentAnnouncements.FirstOrDefault(
                                    y => y.ActivityId == x.Announcement.ClassAnnouncementData.SisActivityId);
                        });
                    var avg = studentAnnouncements.Average(x => x.NumericScore);
                    categoryType.Avg = avg;
                }

                var classAvg = new ClassAvg
                {
                    ClassId = classAnnouncementGroup.ClassId,
                    Items = categoryTypes,
                    Avg = categoryTypes.Average(x => x.Avg)

                };

                res.ClassAvgs.Add(classAvg);
            }
            return res;
        }
    }

    public class ClassCategoryAvg
    {
        public IList<AnnouncementItem> Items { get; set; }
        public decimal? Avg { get; set; }
        public ClassAnnouncementType AnnouncementType { get; set; }
    }

    public class ClassAvg
    {
        public decimal? Avg { get; set; }
        public int? ClassId { get; set; }
        public IList<ClassCategoryAvg> Items { get; set; } 
    }


    public class AnnouncementItem
    {
        public AnnouncementComplex Announcement { get; set; }
        public StudentAnnouncement StudentAnnouncement { get; set; }
    }
}