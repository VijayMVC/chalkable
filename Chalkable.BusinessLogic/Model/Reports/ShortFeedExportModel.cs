using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model.Reports
{

    public class ShortFeedExportModel
    {
        public int PersonId { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public string SchoolName { get; set; }
        public string SchoolYear { get; set; }

        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int ClassTeacherId { get; set; }
        public string ClassTeacherName { get; set; }
        public bool IsPrimaryTeacher { get; set; }
        public string DayType { get; set; }
        public string Period { get; set; }
        public int AnnouncementId { get; set; }
        public string AnnouncementName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Complete { get; set; }
        public bool IsHidden { get; set; }

        protected ShortFeedExportModel(Person person, string schoolName, string sy)
        {
            PersonId = person.Id;
            PersonFirstName = person.FirstName;
            PersonLastName = person.LastName;
            SchoolName = schoolName;
            SchoolYear = sy;
        }

        public static IList<ShortFeedExportModel> Create(Person person, string schoolName, string sy
            , IList<ClassDetails> classes, IList<ScheduleItem> schedules, AnnouncementComplexList announcements)
        {
            var res = new List<ShortFeedExportModel>();
            foreach (var classDetailse in classes)
            {
                var classAnns = announcements.ClassAnnouncements.Where(x => x.ClassRef == classDetailse.Id).ToList();
                var lps = announcements.LessonPlans.Where(x => x.ClassRef == classDetailse.Id).ToList();
                if(lps.Count == 0 && classAnns.Count == 0) continue;

                foreach (var teacher in classDetailse.ClassTeachers)
                {
                    foreach (var lessonPlan in lps)
                    {
                        res.Add(new ShortFeedExportModel(person, schoolName, sy)
                        {
                            ClassId = classDetailse.Id,
                            ClassName = classDetailse.Name,
                            AnnouncementId = lessonPlan.Id,
                            AnnouncementName = lessonPlan.Title,
                            ClassTeacherId = teacher.PersonRef,
                            StartDate = lessonPlan.StartDate,
                            EndDate = lessonPlan.EndDate,
                            IsHidden = !lessonPlan.VisibleForStudent,
                            
                        });
                    }
                    foreach (var ann in classAnns)
                    {
                        res.Add(new ShortFeedExportModel(person, schoolName, sy)
                        {
                            ClassId = classDetailse.Id,
                            ClassName = classDetailse.Name,
                            AnnouncementId = ann.Id,
                            AnnouncementName = ann.Title,
                            StartDate = null,
                            EndDate = ann.Expires,
                            ClassTeacherId = teacher.PersonRef,
                            IsHidden = !ann.VisibleForStudent
                        });
                    }
                }               
            }
            return res;
        }
    }


    public class FeedDetailsExportModel : ShortFeedExportModel
    {
        public string AnnouncementDescription { get; set; }
        public string AnnouncementType { get; set; }
        public double? TotalPoint { get; set; }

        public int? StandardId { get; set; }
        public string StandardName { get; set; }
        public string StandardDescription { get; set; }

        public int? AnnouncementAttachmentId { get; set; }
        public byte[] AnnouncementAttachmentImage { get; set; }
        public string AnnouncementAttachmentName { get; set; }

        public int? AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeDescription { get; set; }
        public byte[] AttributeAttachmentImage { get; set; }
        public string AttributeAttachmentName { get; set; }

        protected FeedDetailsExportModel(Person person, string schoolName, string sy) 
            : base(person, schoolName, sy)
        {
        }
    }
}
