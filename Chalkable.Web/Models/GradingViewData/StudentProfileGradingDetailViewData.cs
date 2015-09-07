﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;
using Chalkable.Data.School.Model.Announcements.Sis;
using Chalkable.Data.School.Model.Chlk;
using Chalkable.Data.School.Model.Sis;
using Chalkable.Web.Models.AnnouncementsViewData;
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
                        .Select(y => new 
                        {
                            AnnouncementType = classAnnouncementTypes.FirstOrDefault(x => x.Id == y.Key),
                            Items = y
                        }).ToList();


                var catTypes = new List<ClassCategoryAvg>();
                foreach (var categoryType in categoryTypes)
                {
                    var ids = categoryType.Items.Select(x => x.ClassAnnouncementData.SisActivityId).Distinct();
                    var studentAnnouncements =
                        gradingDetails.StudentAnnouncements.Where(x => ids.Contains(x.ActivityId)).ToList();

                    var avg = studentAnnouncements.Average(x => x.NumericScore);

                   

                    var catType = new ClassCategoryAvg()
                    {
                        AnnouncementType = categoryType.AnnouncementType,
                        Items = categoryType.Items.Select(x => ShortAnnouncementGradeViewData.Create(x.ClassAnnouncementData, studentAnnouncements, student.Id)).ToList(),
                        Avg = avg
                    };

                    catTypes.Add(catType);
                }

                var classAvg = new ClassAvg
                {
                    ClassId = classAnnouncementGroup.ClassId,
                    Items = catTypes,
                    Avg = catTypes.Average(x => x.Avg)
                };

                res.ClassAvgs.Add(classAvg);
            }
            return res;
        }
    }

    public class ClassCategoryAvg
    {
        public IList<ShortAnnouncementGradeViewData> Items { get; set; }
        public decimal? Avg { get; set; }
        public ClassAnnouncementType AnnouncementType { get; set; }
    }

    public class ClassAvg
    {
        public decimal? Avg { get; set; }
        public int? ClassId { get; set; }
        public IList<ClassCategoryAvg> Items { get; set; } 
    }
}