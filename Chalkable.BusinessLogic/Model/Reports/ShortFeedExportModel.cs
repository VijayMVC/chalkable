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

        public int? StandardId { get; set; }
        public string StandardName { get; set; }
        public int? StandardNumber { get; set; }

        protected ShortFeedExportModel(Person person, string schoolName, string sy, DateTime nowSchoolTime, DateTime? fromReport, DateTime? toReport)
        {
            SchoolName = schoolName;
            SchoolYear = sy;
            ExecutionDateTime = nowSchoolTime;
            StartReportDate = fromReport;
            EndReportDate = toReport;
        }

        protected ShortFeedExportModel(Person person, string schoolName, string sy, DateTime nowSchoolTime, DateTime? fromReport, DateTime? toReport, ClassDetails c, IList<DayType> dayTypes, IList<Staff> teachers
            , AnnouncementComplex announcement, Standard standard, int? standardNumber = null)
            : this(person, schoolName, sy, nowSchoolTime, fromReport, toReport)
        {
            if (c != null)
            {
                ClassId = c.Id;
                ClassName = c.Name;
                ClassNumber = c.ClassNumber;
                if (c.PrimaryTeacherRef.HasValue)
                {
                    PersonFirstName = c.PrimaryTeacher.FirstName;
                    PersonLastName = c.PrimaryTeacher.LastName;
                    Owners = BuildTeachersNames(c.PrimaryTeacherRef.Value, c.ClassTeachers, teachers);
                }
                DayTypes = dayTypes.Where(x => c.ClassPeriods.Any(y => y.DayTypeRef == x.Id))
                                            .Select(x => x.Name.ToString())
                                            .Distinct()
                                            .JoinString(",");

                Periods = c.ClassPeriods.Select(x => x.Period.Name).Distinct().JoinString(",");
            }
            AnnouncementId = announcement.Id;
            AnnouncementName = announcement.Title;
            AnnouncementType = GetTypeName(announcement);
            CategoryName = GetCategoryName(announcement);
            Complete = announcement.Complete;

            HasStandards = announcement.StandardsCount > 0;
            HasAttachments = announcement.AttachmentNames.Count + announcement.ApplicationCount > 0;

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
                PersonFirstName = adminLastName;
                PersonLastName = adminLastName;
                AdminId = announcement.AdminRef;
            }

            if (standard != null)
            {
                StandardId = standard.Id;
                StandardName = standard.Name;
                StandardNumber = standardNumber;
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
            , IList<ClassDetails> classes, IList<Staff> staffs, IList<DayType> dayTypes, IList<AnnouncementDetails> announcements, bool groupByStandards)
        {
            var res = new List<ShortFeedExportModel>(); 
            
            foreach (var c in classes)
            {
                var anns = announcements.Where(x => x.ClassRef == c.Id).ToList();
                if (groupByStandards)
                {
                    var standards = anns.SelectMany(x => x.AnnouncementStandards.Select(s => s.Standard)).Distinct().OrderBy(s => s.Name).ToList();
                    var standrdNumber = 1;
                    foreach (var standard in standards)
                    {
                        var annsWithStandard = anns.Where(x => x.AnnouncementStandards.Any(s => s.StandardRef == standard.Id)).ToList();
                        res.AddRange(BuildClassItems(person, schoolName, sy, nowTime, fromReport, toReport, c, dayTypes, staffs, annsWithStandard, standard, standrdNumber));
                        standrdNumber++;
                    }
                    var annsWithNoStandards = anns.Where(x => x.AnnouncementStandards.Count == 0).ToList();
                    var emptyStandard = new Standard {Id = -1};
                    res.AddRange(BuildClassItems(person, schoolName, sy, nowTime, fromReport, toReport, c, dayTypes, staffs, annsWithNoStandards, emptyStandard, standrdNumber));
                }
                else res.AddRange(BuildClassItems(person, schoolName, sy, nowTime, fromReport, toReport, c, dayTypes, staffs, anns, null));
            }
            var adminAnns = announcements.Where(x => x.AdminAnnouncementData != null)
                                         .OrderBy(x=>x.AdminAnnouncementData.Expires)
                                         .ToList();
            res.AddRange(adminAnns.Select(x => new ShortFeedExportModel(person, schoolName, sy, nowTime, fromReport, toReport, null, dayTypes, staffs, x, null)));     
            
            return res;
        }

        private static IList<ShortFeedExportModel> BuildClassItems(Person person, string schoolName, string sy, DateTime nowTime,
            DateTime? fromReport, DateTime? toReport, ClassDetails @class,  IList<DayType> dayTypes, IList<Staff> staffs,
            IList<AnnouncementDetails> announcements, Standard standard, int? standardNumber = null)
        {
            var res = new List<ShortFeedExportModel>();
            if (announcements.Count == 0)
                return res;
            announcements = announcements.OrderBy(x =>
            {
                if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                if (x.SupplementalAnnouncementData != null) return x.SupplementalAnnouncementData.Expires;
                return x.LessonPlanData != null ? x.LessonPlanData.StartDate : x.Created;
            }).ToList();
            res.AddRange(announcements.Select(a => new ShortFeedExportModel(person, schoolName, sy, nowTime, fromReport, toReport, @class, dayTypes, staffs, a, standard, standardNumber)).ToList());
            return res;
        }
    }
}
