﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public class ShortFeedReportHandler : IReportHandler<FeedReportInputModel>
    {
        private const string SHORT_FEED_REPORT_FILE_NAME = "Reports\\FeedReport\\FeedShortReport.rdlc";

        public object PrepareDataSource(FeedReportInputModel settings, ReportingFormat format, IServiceLocatorSchool serviceLocator, IServiceLocatorMaster masterLocator)
        {
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
            var classes = settings.ClassId.HasValue
                ? new List<ClassDetails> { serviceLocator.ClassService.GetClassDetailsById(settings.ClassId.Value)}
                : ( isStudent 
                        ? serviceLocator.ClassService.GetStudentClasses(sy.Id, person.Id)
                        : serviceLocator.ClassService.GetTeacherClasses(sy.Id, person.Id)
                  );

            var anns = serviceLocator.AnnouncementFetchService.GetAnnouncementComplexList(settings.StartDate, settings.EndDate, true, settings.ClassId);
            

            var schedules = new List<ScheduleItem>();//serviceLocator.ClassPeriodService.GetSchedule(teacherId, studentId, settings.ClassId, settings.StartDate, settings.EndDate);
            return ShortFeedExportModel.Create(person, school.Name, sy.Name, classes, schedules, anns);
        }

        public string GetReportDefinitionFile()
        {
            return SHORT_FEED_REPORT_FILE_NAME;
        }
        
    }

    public class FeedDetailsReportHandler : IReportHandler<FeedReportInputModel>
    {
        private const string FEED_DETAILS_REPORT_FILE_NAME = "Reports\\FeedReport\\FeedDetailsReport.rdlc";
        
        public object PrepareDataSource(FeedReportInputModel settings, ReportingFormat format, IServiceLocatorSchool serviceLocator,
            IServiceLocatorMaster masterLocator)
        {
            var context = serviceLocator.Context;
            Trace.Assert(context.SchoolYearId.HasValue);
            Trace.Assert(context.PersonId.HasValue);
            Trace.Assert(context.SchoolLocalId.HasValue);

            var person = serviceLocator.PersonService.GetPerson(context.PersonId.Value);
            var sy = serviceLocator.SchoolYearService.GetCurrentSchoolYear();
            var school = serviceLocator.SchoolService.GetSchool(context.SchoolLocalId.Value);

            var isStudent = context.Role == CoreRoles.STUDENT_ROLE;
            
            var classes = settings.ClassId.HasValue
                ? new List<ClassDetails> { serviceLocator.ClassService.GetClassDetailsById(settings.ClassId.Value) }
                : (isStudent
                        ? serviceLocator.ClassService.GetStudentClasses(sy.Id, person.Id)
                        : serviceLocator.ClassService.GetTeacherClasses(sy.Id, person.Id)
                  );
            var classTeachers = classes.SelectMany(x => x.ClassTeachers.Select(y => y)).ToList();

            var anns = serviceLocator.AnnouncementFetchService.GetAnnouncementDetailses(settings.StartDate, settings.EndDate, true, settings.ClassId);
            var appIds = anns.SelectMany(x => x.AnnouncementApplications.Select(y => y.ApplicationRef)).Distinct().ToList();
            var apps = masterLocator.ApplicationService.GetApplicationsByIds(appIds);
            IDictionary<Guid, byte[]> appsImages = new Dictionary<Guid, byte[]>();
            foreach (var app in apps)
            {
                if(appsImages.ContainsKey(app.Id)) continue;
                var image = masterLocator.ApplicationPictureService.GetPicture(app.Id, 170, 110);
                appsImages.Add(app.Id, image);
            }

            IDictionary<int, byte[]> attsImages = new Dictionary<int, byte[]>();
            var attachments = anns.SelectMany(x => x.AnnouncementAttributes.Where(y => y.AttachmentRef.HasValue).Select(y => y.Attachment)).ToList();
            attachments.AddRange(anns.SelectMany(x => x.AnnouncementAttachments.Select(y => y.Attachment)).ToList());
            foreach (var att in attachments)
            {
                if(att.IsDocument || attsImages.ContainsKey(att.Id)) continue;
                attsImages.Add(att.Id, serviceLocator.AttachementService.GetAttachmentContent(att).Content);
            }
            
            var staffIds = classTeachers.Select(x => x.PersonRef).Distinct().ToList();
            var staffs = staffIds.Select(y => serviceLocator.StaffService.GetStaff(y)).ToList();

            //todo get schedules and fix performence issue in getting schedules 
            var teacherId = isStudent ? null : (int?)person.Id;
            var studentId = isStudent ? (int?)person.Id : null;
            var schedules = new List<ScheduleItem>();

            return FeedDetailsExportModel.Create(person, school.Name, sy.Name, anns, classTeachers, staffs, schedules, apps, appsImages, attsImages);
        }

        public string GetReportDefinitionFile()
        {
            return FEED_DETAILS_REPORT_FILE_NAME;
        }
    }
}
