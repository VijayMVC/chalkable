using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chalkable.BusinessLogic.Common;
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
        public DateTime ExecutionDateTime { get; set; }
        public DateTime? StartReportDate { get; set; }
        public DateTime? EndReportDate { get; set; }
        public bool IsAdminAnnouncement { get; set; }
        public int? ClassId { get; set; }
        public int? AdminId { get; set; }
        public string ClassName { get; set; }
        public string ClassNumber { get; set; }
        public string Owners { get; set; }
        public string DayTypes { get; set; }
        public string Periods { get; set; }
        public int AnnouncementId { get; set; }
        public string AnnouncementName { get; set; }
        public string AnnouncementType { get; set; }
        public string CategoryName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Complete { get; set; }
        public bool IsHidden { get; set; }
        public double? TotalPoint { get; set; }
        public bool HasStandards { get; set; }
        public bool HasAttachments { get; set; }


        protected ShortFeedExportModel(Person person, string schoolName, string sy, DateTime nowSchoolTime, DateTime? fromReport, DateTime? toReport)
        {
            PersonId = person.Id;
            PersonFirstName = person.FirstName;
            PersonLastName = person.LastName;
            SchoolName = schoolName;
            SchoolYear = sy;
            ExecutionDateTime = nowSchoolTime;
            StartReportDate = fromReport;
            EndReportDate = toReport;
        }

        protected ShortFeedExportModel(Person person, string schoolName, string sy, DateTime nowSchoolTime, DateTime? fromReport, DateTime? toReport, ClassDetails c, IList<DayType> dayTypes, IList<Staff> teachers
            , AnnouncementComplex announcement)
            : this(person, schoolName, sy, nowSchoolTime, fromReport, toReport)
        {
            if (c != null)
            {
                ClassId = c.Id;
                ClassName = c.Name;
                ClassNumber = c.ClassNumber;
                if (c.PrimaryTeacherRef.HasValue)
                    Owners = BuildTeachersNames(c.PrimaryTeacherRef.Value, c.ClassTeachers, teachers);

                DayTypes = dayTypes.Where(x => c.ClassPeriods.Any(y => y.DayTypeRef == x.Id))
                                            .Select(x => x.Name.ToString())
                                            .Distinct()
                                            .JoinString(",");
                Periods = c.ClassPeriods.Select(x => x.Period.Order.ToString()).Distinct().JoinString(",");
            }
            AnnouncementId = announcement.Id;
            AnnouncementName = announcement.Title;
            AnnouncementType = GetTypeName(announcement);
            CategoryName = GetCategoryName(announcement);
            Complete = announcement.Complete;

            HasStandards = true;
            HasAttachments = true;
            //HasStandards = announcement.AnnouncementStandards.Count > 0;
            //HasAttachments = announcement.AnnouncementAttachments.Count + announcement.AnnouncementApplications.Count > 0;

            if (announcement.ClassAnnouncementData != null)
            {
                StartDate = null;
                EndDate = announcement.ClassAnnouncementData.Expires;
                IsHidden = !announcement.ClassAnnouncementData.VisibleForStudent;
                TotalPoint = (double?)announcement.ClassAnnouncementData.MaxScore;
            }
            if (announcement.LessonPlanData != null)
            {
                StartDate = announcement.LessonPlanData.StartDate;
                EndDate = announcement.LessonPlanData.EndDate;
                IsHidden = !announcement.LessonPlanData.VisibleForStudent;
            }
            if (announcement.SupplementalAnnouncementData != null)
            {
                StartDate = null;
                EndDate = announcement.SupplementalAnnouncementData.Expires;
                IsHidden = !announcement.SupplementalAnnouncementData.VisibleForStudent;
            }

            var adminAnn = announcement.AdminAnnouncementData;
            if (adminAnn != null)
            {
                //TODO: Add to model first and last admin name
                string adminFirstName = null, adminLastName = null;
                if (adminAnn.AdminName != null)
                {
                    string[] words = adminAnn.AdminName.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    adminFirstName = words[0];
                    if (words.Count() > 1)
                        adminLastName = words[1];
                }
                EndDate = adminAnn.Expires;
                IsAdminAnnouncement = true;
                Owners =  NameHelper.FullName(adminFirstName ?? "", adminLastName ?? "", "", false, adminAnn.AdminGender);
                AdminId = announcement.AdminRef;
            }
        }

        protected string GetTypeName(AnnouncementComplex announcement)
        {
            switch (announcement.Type)
            {
                case AnnouncementTypeEnum.Class: return "Assignment";
                case AnnouncementTypeEnum.LessonPlan: return "Lesson Plan";
                case AnnouncementTypeEnum.Supplemental: return "Supplemental";
                case AnnouncementTypeEnum.Admin: return "Admin Announcement";
            }
            return null;
        }

        protected string GetCategoryName(AnnouncementComplex announcement)
        {
            if (announcement.ClassAnnouncementData != null)
                return announcement.ClassAnnouncementData.ClassAnnouncementTypeName;
            if (announcement.LessonPlanData != null)
                return announcement.LessonPlanData.CategoryName;
            return announcement.SupplementalAnnouncementData?.ClassAnnouncementTypeName;
        }

        protected string BuildTeachersNames(int primeryTeacherId, IList<ClassTeacher> classTeachers, IList<Staff> staffs)
        {
            var primaryTeacher = staffs.FirstOrDefault(x => x.Id == primeryTeacherId);
            if (primaryTeacher == null) return null;
            var b = new StringBuilder();
            var secondaryTeachers = staffs.Where(t => t.Id != primaryTeacher.Id && classTeachers.Any(ct=>ct.PersonRef == t.Id))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => x.FullName(false, true)).ToList();
            b.Append(primaryTeacher.FullName(false, true));
            if(secondaryTeachers.Count > 0)
                b.Append(", ").Append(secondaryTeachers.JoinString(", "));
            return b.ToString();
        }
        
        public ShortFeedExportModel() { }

        public static IList<ShortFeedExportModel> Create(Person person, string schoolName, string sy, DateTime nowTime, DateTime? fromReport, DateTime? toReport
            , IList<ClassDetails> classes, IList<Staff> staffs, IList<DayType> dayTypes, IList<AnnouncementComplex> announcements)
        {
            var res = new List<ShortFeedExportModel>(); 
            foreach (var c in classes)
            {
                var anns = announcements.Where(x => x.ClassRef == c.Id).ToList();
                if (anns.Count == 0) continue;
                anns = anns.OrderBy(x =>
                {
                    if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                    if (x.SupplementalAnnouncementData != null) return x.SupplementalAnnouncementData.Expires;
                    return x.LessonPlanData != null ? x.LessonPlanData.StartDate : x.Created;
                }).ToList();
                res.AddRange(anns.Select(a=> new ShortFeedExportModel(person, schoolName, sy, nowTime, fromReport, toReport, c, dayTypes, staffs, a)).ToList());
            }
            var adminAnns = announcements.Where(x => x.AdminAnnouncementData != null)
                                         .OrderBy(x=>x.AdminAnnouncementData.Expires)
                                         .ToList();
            res.AddRange(adminAnns.Select(x => new ShortFeedExportModel(person, schoolName, sy, nowTime, fromReport, toReport, null, dayTypes, staffs, x)));            
            return res;
        }
    }
}
