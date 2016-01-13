using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.Reporting
{

    public class FeedDetailsReportHandler : IReportHandler<FeedReportInputModel>
    {
        private const string FEED_DETAILS_REPORT_FILE_NAME = "Reports\\FeedReport\\FeedDetailsReport.rdlc";

        public string GetReportDefinitionFile()
        {
            return FEED_DETAILS_REPORT_FILE_NAME;
        }
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
            var onlyOwner = !isStudent && !serviceLocator.Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN); ;
            IList<AnnouncementDetails> anns;
            if (settings.LessonPlanOnly && !BaseSecurity.IsDistrictAdmin(serviceLocator.Context))
                anns = serviceLocator.LessonPlanService.GetAnnouncementDetailses(settings.StartDate, settings.EndDate, inputModel.ClassId, inputModel.Complete, onlyOwner);
            else
                anns = serviceLocator.AnnouncementFetchService.GetAnnouncementDetailses(settings.StartDate, settings.EndDate, inputModel.ClassId, inputModel.Complete);

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
                    x.AnnouncementAttributes = x.AnnouncementAttributes.Where(a => a.VisibleForStudents).ToList();
                    return x;
                }).ToList();

            var appIds = anns.SelectMany(x => x.AnnouncementApplications.Select(y => y.ApplicationRef)).Distinct().ToList();
            var apps = masterLocator.ApplicationService.GetApplicationsByIds(appIds);
            IDictionary<Guid, byte[]> appsImages = new Dictionary<Guid, byte[]>();
            foreach (var app in apps)
            {
                if (appsImages.ContainsKey(app.Id)) continue;
                var image = masterLocator.ApplicationPictureService.GetPicture(app.BigPictureRef.Value, 170, 110);
                appsImages.Add(app.Id, image);
            }

            return FeedDetailsExportModel.Create(person, school.Name, sy.Name, serviceLocator.Context.NowSchoolTime, anns, classes, dayTypes, staffs, apps, appsImages);
        }

    }
}
