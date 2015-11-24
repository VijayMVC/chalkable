using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.BusinessLogic.Common;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

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
        public string ClassTeacherNames { get; set; }
        public string DayTypes { get; set; }
        public string Periods { get; set; }
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

        protected ShortFeedExportModel(Person person, string schoolName, string sy, ClassDetails c, IList<Staff> teachers
            , Announcement announcement, bool complete)
            : this(person, schoolName, sy)
        {
            ClassId = c.Id;
            ClassName = c.Name;
            if(c.PrimaryTeacherRef.HasValue)
                ClassTeacherNames = BuildTeachersNames(c.PrimaryTeacherRef.Value, c.ClassTeachers, teachers);
            AnnouncementId = announcement.Id;
            AnnouncementName = announcement.Title;
            Complete = complete;
        }

        protected string BuildTeachersNames(int primeryTeacherId, IList<ClassTeacher> classTeachers, IList<Staff> staffs)
        {
            var primaryTeacher = staffs.FirstOrDefault(x => x.Id == primeryTeacherId);
            if (primaryTeacher == null) return null;
            var b = new StringBuilder();
            var secondaryTeachers = staffs.Where(t => t.Id != primaryTeacher.Id && classTeachers.Any(ct=>ct.PersonRef == t.Id))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => x.FullName());
            b.Append(primaryTeacher.FullName()).Append(",")
                .Append(secondaryTeachers.JoinString(","));
            return b.ToString();
        }

        public ShortFeedExportModel() { }

        public static IList<ShortFeedExportModel> Create(Person person, string schoolName, string sy
            , IList<ClassDetails> classes, IList<ScheduleItem> schedules, AnnouncementComplexList announcements)
        {
            var res = new List<ShortFeedExportModel>();
            foreach (var classDetailse in classes)
            {
                var classAnns = announcements.ClassAnnouncements.Where(x => x.ClassRef == classDetailse.Id).ToList();
                var lps = announcements.LessonPlans.Where(x => x.ClassRef == classDetailse.Id).ToList();
                if(lps.Count == 0 && classAnns.Count == 0) continue;

                foreach (var lessonPlan in lps)
                {
                    res.Add(new ShortFeedExportModel(person, schoolName, sy, classDetailse, new List<Staff>(), lessonPlan, true)
                    {
                        StartDate = lessonPlan.StartDate,
                        EndDate = lessonPlan.EndDate,
                        IsHidden = !lessonPlan.VisibleForStudent,
                    });
                }
                foreach (var ann in classAnns)
                {
                    res.Add(new ShortFeedExportModel(person, schoolName, sy, classDetailse, new List<Staff>(), ann, true)
                    {
                        StartDate = null,
                        EndDate = ann.Expires,
                        IsHidden = !ann.VisibleForStudent
                    });
                }            
            }
            return res;
        }
    }
}
