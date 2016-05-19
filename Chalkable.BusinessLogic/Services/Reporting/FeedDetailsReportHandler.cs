using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    public class FeedDetailsReportHandler : BaseFeedReportHandler
    {
        public override string ReportDefinitionFile => "Reports\\FeedReport\\FeedDetailsReport.rdlc";
        
        public override object PrepareDataSource(FeedReportInputModel inputModel, ReportingFormat format, IServiceLocatorSchool serviceLocator,
            IServiceLocatorMaster masterLocator)
        {
            var baseData = PrepareBaseReportData(inputModel, serviceLocator);
            var settings = inputModel.Settings;
            
            
            var annType = inputModel.Settings.LessonPlanOnly ? AnnouncementTypeEnum.LessonPlan : (AnnouncementTypeEnum?) inputModel.AnnouncementType;
            var anns = serviceLocator.AnnouncementFetchService.GetAnnouncementDetailses(settings.StartDate, settings.EndDate, inputModel.ClassId, inputModel.Complete, annType);

            //hide hidden activities if not included
            if (!settings.IncludeHiddenActivities)
                anns = anns.Where(x => x.ClassAnnouncementData == null || x.ClassAnnouncementData.VisibleForStudent).ToList();

            // hide attachments and applications if not included
            if (!settings.IncludeAttachments)
                anns = anns.Select(x =>
                {
                    x.AnnouncementAttachments = new List<AnnouncementAttachment>();
                    x.AnnouncementApplications = new List<AnnouncementApplication>();
                    return x;
                }).ToList();

            // hide hidden attributes if not included 
            if (!settings.IncludeHiddenAttributes)
                anns = anns.Select(x =>
                {
                    x.AnnouncementAttributes = x.AnnouncementAttributes.Where(a => a.VisibleForStudents).ToList();
                    return x;
                }).ToList();

            //Get apps with images
            var appIds = anns.SelectMany(x => x.AnnouncementApplications.Select(y => y.ApplicationRef)).Distinct().ToList();
            var apps = masterLocator.ApplicationService.GetApplicationsByIds(appIds);
            IDictionary<Guid, byte[]> appsImages = new Dictionary<Guid, byte[]>();
            foreach (var app in apps)
            {
                if (appsImages.ContainsKey(app.Id)) continue;
                var image = masterLocator.ApplicationPictureService.GetPicture(app.BigPictureRef.Value, 170, 110);
                appsImages.Add(app.Id, image);
            }

            return FeedDetailsExportModel.Create(baseData.Person, baseData.SchoolName, baseData.SchoolYearName, serviceLocator.Context.NowSchoolTime, 
                anns, baseData.Classes, baseData.DayTypes, baseData.Staffs, apps, appsImages);
        }

    }
}
