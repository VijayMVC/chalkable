﻿using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Services.Reporting
{

    public class ShortFeedReportHandler : BaseFeedReportHandler
    {
        public override string ReportDefinitionFile => "Reports\\FeedReport\\FeedShortReport.rdlc";
        
        public override object PrepareDataSource(FeedReportInputModel inputModel, ReportingFormat format, IServiceLocatorSchool serviceLocator, IServiceLocatorMaster masterLocator)
        {
            var baseData = PrepareBaseReportData(inputModel, serviceLocator);
            
            var feedSettings = new FeedSettingsInfo
            {
                AnnouncementType = inputModel.Settings.LessonPlanOnly ? (int)AnnouncementTypeEnum.LessonPlan : inputModel.AnnouncementType,
                FromDate = inputModel.Settings.StartDate,
                ToDate = inputModel.Settings.EndDate
            };

            var anns = serviceLocator.AnnouncementFetchService.GetAnnouncementsForFeed(inputModel.Complete, null, inputModel.ClassId, feedSettings);

            //hide hidden activities
            if (!inputModel.Settings.IncludeHiddenActivities)
                anns = anns.Where(x => x.ClassAnnouncementData == null || x.ClassAnnouncementData.VisibleForStudent).ToList();
            
            return ShortFeedExportModel.Create(baseData.Person, baseData.SchoolName, baseData.SchoolYearName, serviceLocator.Context.NowSchoolTime
                , baseData.Classes, baseData.Staffs, baseData.DayTypes, anns);
        }
        
    }
}
