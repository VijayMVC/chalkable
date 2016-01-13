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

            var onlyOwner = !isStudent && !serviceLocator.Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN);
            var anns = new List<AnnouncementComplex>();
            if (settings.LessonPlanOnly && !BaseSecurity.IsDistrictAdmin(serviceLocator.Context))
            {
                anns.AddRange(serviceLocator.LessonPlanService.GetLessonPlansForFeed(settings.StartDate, settings.EndDate, null, inputModel.ClassId, inputModel.Complete, onlyOwner));
            }
            else
            {
                if ((BaseSecurity.IsDistrictAdmin(serviceLocator.Context) || isStudent) && !inputModel.ClassId.HasValue)
                    anns.AddRange(serviceLocator.AdminAnnouncementService.GetAnnouncementsComplex(settings.StartDate, settings.EndDate, null, inputModel.Complete, onlyOwner));
                if (BaseSecurity.IsTeacher(serviceLocator.Context) || isStudent || inputModel.ClassId.HasValue)
                {
                    onlyOwner = !isStudent && !serviceLocator.Context.Claims.HasPermission(ClaimInfo.VIEW_CLASSROOM_ADMIN);
                    anns.AddRange(serviceLocator.ClassAnnouncementService.GetClassAnnouncementsForFeed(settings.StartDate, settings.EndDate, inputModel.ClassId, inputModel.Complete, onlyOwner));
                    anns.AddRange(serviceLocator.LessonPlanService.GetLessonPlansForFeed(settings.StartDate, settings.EndDate, null, inputModel.ClassId, inputModel.Complete, onlyOwner));
                }
            }
            if (!settings.IncludeHiddenActivities)
                anns = anns.Where(x => x.ClassAnnouncementData == null || x.ClassAnnouncementData.VisibleForStudent).ToList();

            return ShortFeedExportModel.Create(person, school.Name, sy.Name, serviceLocator.Context.NowSchoolTime, classes, staffs, dayTypes, anns);
        }

        public string GetReportDefinitionFile()
        {
            return SHORT_FEED_REPORT_FILE_NAME;
        }
        
    }
}
