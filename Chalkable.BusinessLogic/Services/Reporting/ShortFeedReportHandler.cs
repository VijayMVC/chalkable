using System.Collections.Generic;
using System.Diagnostics;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public class ShortFeedReportHandler : IReportHandler<FeedReportInputModel>
    {
        private const string SHORT_FEED_REPORT_FILE_NAME = "Reports\\ShortFeedReportHandler.rdlc";

        public object PrepareDataSource(FeedReportInputModel settings, ReportingFormat format, IServiceLocatorSchool serviceLocator, IServiceLocatorMaster masterLocator)
        {
            var context = serviceLocator.Context;
            Trace.Assert(context.SchoolYearId.HasValue);
            Trace.Assert(context.PersonId.HasValue);
            Trace.Assert(context.SchoolLocalId.HasValue);

            var person = serviceLocator.PersonService.GetPerson(context.PersonId.Value);
            var sy = serviceLocator.SchoolYearService.GetCurrentSchoolYear();
            var isStudent = context.Role == CoreRoles.STUDENT_ROLE;
            var teacherId = isStudent ? null : (int?)person.Id;
            var studentId = isStudent ? (int?) person.Id : null;
            var classes = settings.ClassId.HasValue
                ? new List<ClassDetails> { serviceLocator.ClassService.GetClassDetailsById(settings.ClassId.Value)}
                : ( isStudent 
                        ? serviceLocator.ClassService.GetStudentClasses(sy.Id, person.Id)
                        : serviceLocator.ClassService.GetTeacherClasses(sy.Id, person.Id)
                  );

            var anns = serviceLocator.AnnouncementFetchService.GetAnnouncementComplexList(settings.StartDate, settings.EndDate, true, settings.ClassId);
            

            var schedules = serviceLocator.ClassPeriodService.GetSchedule(teacherId, studentId, settings.ClassId, settings.StartDate, settings.EndDate);
            var school = serviceLocator.SchoolService.GetSchool(context.SchoolLocalId.Value);
            return ShortFeedExportModel.Create(person, school.Name, sy.Name, classes, schedules, anns);
        }

        public string GetReportDefinitionFile()
        {
            return SHORT_FEED_REPORT_FILE_NAME;
        }
        
    }
}
