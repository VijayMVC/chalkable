using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public class ShortFeedReportHandler : IReportHandler<FeedReportInputModel>
    {
        private const string SHORT_FEED_REPORT_FILE_NAME = "Reports\\FeedReport\\FeedShortReport.rdlc";

        public object PrepareDataSource(FeedReportInputModel inputModel, ReportingFormat format, IServiceLocatorSchool serviceLocator, IServiceLocatorMaster masterLocator)
        {
            var settings = inputModel.Settings;
            var context = serviceLocator.Context;
            Trace.Assert(context.SchoolYearId.HasValue);
            Trace.Assert(context.PersonId.HasValue);
            Trace.Assert(context.SchoolLocalId.HasValue);

            var person = serviceLocator.PersonService.GetPerson(context.PersonId.Value);
            var sy = serviceLocator.SchoolYearService.GetCurrentSchoolYear();
            var school = serviceLocator.SchoolService.GetSchool(context.SchoolLocalId.Value);
            var isStudent = context.Role == CoreRoles.STUDENT_ROLE;
            var teacherId = isStudent ? null : (int?)person.Id;
            var studentId = isStudent ? (int?) person.Id : null;
            var dayTypes = serviceLocator.DayTypeService.GetDayTypes();

            var classes = inputModel.ClassId.HasValue
                ? new List<ClassDetails> { serviceLocator.ClassService.GetClassDetailsById(inputModel.ClassId.Value)}
                : serviceLocator.ClassService.GetClasses(sy.Id, studentId, teacherId);
                  
            var classTeachers = classes.SelectMany(x => x.ClassTeachers.Select(y => y)).ToList();
            var staffIds = classTeachers.Select(x => x.PersonRef).Distinct().ToList();
            var staffs = staffIds.Select(y => serviceLocator.StaffService.GetStaff(y)).ToList();

            
            var anns = new List<AnnouncementComplex>();
            if (settings.LessonPlanOnly && !BaseSecurity.IsDistrictAdmin(serviceLocator.Context))
            {
                anns.AddRange(serviceLocator.LessonPlanService.GetLessonPlansForFeed(settings.StartDate, settings.EndDate, null, inputModel.ClassId, null));
            }
            else
            {
                if (BaseSecurity.IsDistrictAdmin(serviceLocator.Context) || serviceLocator.Context.Role == CoreRoles.STUDENT_ROLE)
                    anns.AddRange(serviceLocator.AdminAnnouncementService.GetAnnouncementsComplex(settings.StartDate, settings.EndDate, null, null, false));
                if (BaseSecurity.IsTeacher(serviceLocator.Context) || serviceLocator.Context.Role == CoreRoles.STUDENT_ROLE)
                {
                    anns.AddRange(serviceLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(settings.StartDate, settings.EndDate, inputModel.ClassId, null));
                    anns.AddRange(serviceLocator.LessonPlanService.GetLessonPlansForFeed(settings.StartDate, settings.EndDate, null, inputModel.ClassId, null));
                }
            }
            if (!settings.IncludeHiddenActivities)
                anns = anns.Where(x => x.ClassAnnouncementData == null || x.ClassAnnouncementData.VisibleForStudent).ToList();

            return ShortFeedExportModel.Create(person, school.Name, sy.Name, classes, staffs, dayTypes, anns);
        }

        public string GetReportDefinitionFile()
        {
            return SHORT_FEED_REPORT_FILE_NAME;
        }
        
    }

    public class FeedDetailsReportHandler : IReportHandler<FeedReportInputModel>
    {
        private const string FEED_DETAILS_REPORT_FILE_NAME = "Reports\\FeedReport\\FeedDetailsReport.rdlc";
        
        public object PrepareDataSource(FeedReportInputModel inputModel, ReportingFormat format, IServiceLocatorSchool serviceLocator,
            IServiceLocatorMaster masterLocator)
        {
            var settings = inputModel.Settings;
            var context = serviceLocator.Context;
            Trace.Assert(context.SchoolYearId.HasValue);
            Trace.Assert(context.PersonId.HasValue);
            Trace.Assert(context.SchoolLocalId.HasValue);

            var person = serviceLocator.PersonService.GetPerson(context.PersonId.Value);
            var sy = serviceLocator.SchoolYearService.GetCurrentSchoolYear();
            var school = serviceLocator.SchoolService.GetSchool(context.SchoolLocalId.Value);
            var dayTypes = serviceLocator.DayTypeService.GetDayTypes();

            var isStudent = context.Role == CoreRoles.STUDENT_ROLE;
            var teacherId = isStudent ? null : (int?)person.Id;
            var studentId = isStudent ? (int?)person.Id : null;
            
            var classes = inputModel.ClassId.HasValue
             ? new List<ClassDetails> { serviceLocator.ClassService.GetClassDetailsById(inputModel.ClassId.Value) }
             : serviceLocator.ClassService.GetClasses(sy.Id, studentId, teacherId);

            var classTeachers = classes.SelectMany(x => x.ClassTeachers.Select(y => y)).ToList();
            var staffIds = classTeachers.Select(x => x.PersonRef).Distinct().ToList();
            var staffs = staffIds.Select(y => serviceLocator.StaffService.GetStaff(y)).ToList();
            
            //Getting and Preparing Announcements details info 
            IList<AnnouncementDetails> anns;
            if (settings.LessonPlanOnly && !BaseSecurity.IsDistrictAdmin(serviceLocator.Context))
                anns = serviceLocator.LessonPlanService.GetAnnouncementDetailses(settings.StartDate, settings.EndDate, inputModel.ClassId);
            else 
                anns = serviceLocator.AnnouncementFetchService.GetAnnouncementDetailses(settings.StartDate, settings.EndDate, false, inputModel.ClassId);

            if (!settings.IncludeHiddenActivities)
                anns = anns.Where(x => x.ClassAnnouncementData == null || x.ClassAnnouncementData.VisibleForStudent).ToList();

            if (!settings.IncludeAttachments)
                anns = anns.Select(x =>
                {
                    x.AnnouncementAttachments = new List<AnnouncementAttachment>();
                    x.AnnouncementApplications = new List<AnnouncementApplication>();
                    return x;
                }).ToList();

            if (!settings.IncludeHiddenAttributes)
                anns = anns.Select(x =>
                {
                    x.AnnouncementAttributes = x.AnnouncementAttributes.Where(a=>a.VisibleForStudents).ToList();
                    return x;
                }).ToList();

            var appIds = anns.SelectMany(x => x.AnnouncementApplications.Select(y => y.ApplicationRef)).Distinct().ToList();
            var apps = masterLocator.ApplicationService.GetApplicationsByIds(appIds);
            IDictionary<Guid, byte[]> appsImages = new Dictionary<Guid, byte[]>();
            foreach (var app in apps)
            {
                if(appsImages.ContainsKey(app.Id)) continue;
                var image = masterLocator.ApplicationPictureService.GetPicture(app.BigPictureRef.Value, 170, 110);
                appsImages.Add(app.Id, image);
            }
           
            return FeedDetailsExportModel.Create(person, school.Name, sy.Name, anns, classes, dayTypes, staffs, apps, appsImages);
        }

        public string GetReportDefinitionFile()
        {
            return FEED_DETAILS_REPORT_FILE_NAME;
        }
    }
}
