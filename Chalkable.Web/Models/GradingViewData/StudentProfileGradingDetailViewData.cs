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


        public static StudentProfileGradingDetailViewData Create(StudentDetails student, StudentGradingDetails gradingDetails, GradingPeriod gp, IList<AnnouncementComplex> announcements)
        {
            var classAnnouncements = announcements.GroupBy(x => x.ClassRef).Select(y => new
            {
                ClassId = y.Key,
                Announcements = y.ToList()
            });


            var res = new StudentProfileGradingDetailViewData(student)
            {
                CurrentGradingPeriod = GradingPeriodViewData.Create(gp),
                ClassAvgs = new List<ClassAvg>()
            };

            foreach (var classAnnouncement in classAnnouncements)
            {
                var categoryTypes =
                    announcements.GroupBy(x => x.ClassAnnouncementData.ClassAnnouncementTypeName)
                        .Select(y => new ClassCategoryAvg()
                        {
                            AnnouncementTypeName = y.Key,
                            Items = y
                        }).ToList();

                foreach (var categoryType in categoryTypes)
                {
                    var ids = categoryType.Items.Select(x => x.ClassAnnouncementData.SisActivityId).Distinct();
                    var studentAnnouncements =
                        gradingDetails.StudentAnnouncements.Where(x => ids.Contains(x.ActivityId));

                    var avg = studentAnnouncements.Average(x => x.NumericScore);
                    categoryType.Avg = avg;
                }

                var classAvg = new ClassAvg
                {
                    ClassId = classAnnouncement.ClassId,
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
        public IEnumerable<AnnouncementComplex> Items { get; set; }
        public decimal? Avg { get; set; }
        public string AnnouncementTypeName { get; set; }
    }

    public class ClassAvg
    {
        public decimal? Avg { get; set; }
        public int? ClassId { get; set; }
        public IEnumerable<ClassCategoryAvg> Items { get; set; } 
    }
}