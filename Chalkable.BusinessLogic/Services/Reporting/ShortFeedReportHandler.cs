using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Security;
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

            //bool isForAdminPortal = BaseSecurity.IsDistrictAdmin(serviceLocator.Context) && !inputModel.ClassId.HasValue;

            //var anns = isForAdminPortal
            //    ? serviceLocator.AnnouncementFetchService.GetAnnouncementsForAdminFeed(inputModel.Complete, null, feedSettings)
            //    : serviceLocator.AnnouncementFetchService.GetAnnouncementsForFeed(inputModel.Complete, inputModel.ClassId, feedSettings);

            var anns = serviceLocator.AnnouncementFetchService.GetAnnouncementDetailses(feedSettings.FromDate, feedSettings.ToDate, inputModel.ClassId, inputModel.Complete, feedSettings.AnnouncementTypeEnum);
            
            //hide hidden activities
            if (!inputModel.Settings.IncludeHiddenActivities)
                anns = anns.Where(x => x.ClassAnnouncementData == null || x.ClassAnnouncementData.VisibleForStudent).ToList();

            var fromDate = inputModel.Settings.StartDate ?? serviceLocator.Context.SchoolYearStartDate;
            var toDate = inputModel.Settings.EndDate ?? serviceLocator.Context.SchoolYearEndDate;
            return ShortFeedExportModel.Create(baseData.Person, baseData.SchoolName, baseData.SchoolYearName, serviceLocator.Context.NowSchoolTime,
                fromDate, toDate, baseData.Classes, baseData.Staffs, baseData.DayTypes, anns);
        }
        
    }
}
