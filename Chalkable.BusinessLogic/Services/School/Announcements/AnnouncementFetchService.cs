﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.School.Announcements
{
    public interface IAnnouncementFetchService
    {
        IList<AnnouncementComplex> GetAnnouncementsForFeed(bool? complete, int start, int count, int? classId);
        AnnouncementComplexList GetAnnouncementComplexList(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, int? studentId = null);
        IList<Announcement> GetAnnouncementsByFilter(string filter);
        Announcement GetLastDraft();
        AnnouncementType GetAnnouncementType(int announcementId);
    }

    public class AnnouncementFetchService : SchoolServiceBase, IAnnouncementFetchService
    {
        public AnnouncementFetchService(IServiceLocatorSchool serviceLocator)
            : base(serviceLocator)
        {
        }


        public IList<AnnouncementComplex> GetAnnouncementsForFeed(bool? complete, int start, int count, int? classId)
        {
            if (BaseSecurity.IsDistrictAdmin(Context))
                return ServiceLocator.AdminAnnouncementService.GetAdminAnnouncementsForFeed(complete, null, null, null, start, count);

            var res = new List<AnnouncementComplex>();
            if (BaseSecurity.IsTeacher(Context) || Context.Role == CoreRoles.STUDENT_ROLE)
            {

                var classAnns = ServiceLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(null, null, classId, complete, true, null, start, count + 1);

                var fromDate = start != 0 && classAnns.Count > 0 ? classAnns.Min(x => x.ClassAnnouncementData.Expires) : DateTime.MinValue;
                var toDate = classAnns.Count > 0 ? classAnns.Max(x => x.ClassAnnouncementData.Expires) : DateTime.MaxValue;
                if (classAnns.Count == count + 1)
                    classAnns.RemoveAt(classAnns.Count - 1);

                res.AddRange(classAnns);
                res.AddRange(ServiceLocator.LessonPlanService.GetLessonPlansForFeed(fromDate, toDate, null, classId, complete, true));

                if (Context.Role == CoreRoles.STUDENT_ROLE)
                    res.AddRange(ServiceLocator.AdminAnnouncementService.GetAdminAnnouncementsForFeed(complete, null, fromDate, toDate, start, count));
            }
            return res.OrderBy(x =>
            {
                if (x.AdminAnnouncementData != null) return x.AdminAnnouncementData.Expires;
                if (x.ClassAnnouncementData != null) return x.ClassAnnouncementData.Expires;
                return x.LessonPlanData != null ? x.LessonPlanData.StartDate : x.Created;
            }).ToList();
        }

        public AnnouncementComplexList GetAnnouncementComplexList(DateTime? fromDate, DateTime? toDate, bool onlyOwners = false, int? classId = null, int? studentId = null)
        {
            var res = new AnnouncementComplexList()
                {
                    AdminAnnouncements = new List<AdminAnnouncement>(),
                    ClassAnnouncements = new List<ClassAnnouncement>(),
                    LessonPlans = new List<LessonPlan>()
                };
            if (BaseSecurity.IsDistrictAdmin(Context) || CoreRoles.STUDENT_ROLE == Context.Role)
                res.AdminAnnouncements = ServiceLocator.AdminAnnouncementService.GetAdminAnnouncements(null, fromDate, toDate, studentId);
            if (BaseSecurity.IsTeacher(Context) || CoreRoles.STUDENT_ROLE == Context.Role)
            {
                res.LessonPlans = ServiceLocator.LessonPlanService.GetLessonPlans(fromDate, toDate, classId, null);
                res.ClassAnnouncements = ServiceLocator.ClassAnnouncementService.GetClassAnnouncements(fromDate, toDate, classId, onlyOwners);
            }
            return res;
        }

        public IList<Announcement> GetAnnouncementsByFilter(string filter)
        {
            var res = new List<Announcement>();
            if (BaseSecurity.IsDistrictAdmin(Context) || CoreRoles.STUDENT_ROLE == Context.Role)
                res.AddRange(ServiceLocator.AdminAnnouncementService.GetAdminAnnouncementsByFilter(filter));
            if (BaseSecurity.IsTeacher(Context) || CoreRoles.STUDENT_ROLE == Context.Role)
            {
                res.AddRange(ServiceLocator.LessonPlanService.GetLessonPlansbyFilter(filter));
                res.AddRange(ServiceLocator.ClassAnnouncementService.GetClassAnnouncementsByFilter(filter));
            }
            return res.OrderBy(x => x.Created).ToList();
        }

        public Announcement GetLastDraft()
        {
            Announcement res = null;
            if (BaseSecurity.IsDistrictAdmin(Context))
                return ServiceLocator.AdminAnnouncementService.GetLastDraft();
            return ServiceLocator.ClassAnnouncementService.GetLastDraft() ?? (Announcement) ServiceLocator.LessonPlanService.GetLastDraft();
        }

        public AnnouncementType GetAnnouncementType(int announcementId)
        {
            return DoRead(u => new AnnouncementDataAccess(u).GetAnnouncementType(announcementId));
        }
    }


    public class AnnouncementComplexList
    {
        public IList<LessonPlan> LessonPlans { get; set; }
        public IList<ClassAnnouncement> ClassAnnouncements { get; set; }
        public IList<AdminAnnouncement> AdminAnnouncements { get; set; } 
    }
}
