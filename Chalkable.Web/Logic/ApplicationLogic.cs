using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Logic
{
    public class ApplicationLogic
    {

        public static IList<AnnouncementApplicationViewData> PrepareAnnouncementApplicationInfo(IServiceLocatorSchool schoolLocator, IServiceLocatorMaster masterLocator, int announcementId)
        {
            var annApps = schoolLocator.ApplicationSchoolService.GetAnnouncementApplicationsByAnnId(announcementId, true);
            var apps = masterLocator.ApplicationService.GetApplicationsByIds(annApps.Select(x=>x.ApplicationRef).ToList());
            var announcementType = schoolLocator.AnnouncementFetchService.GetAnnouncementType(announcementId);

            return AnnouncementApplicationViewData.Create(annApps, apps, schoolLocator.Context.PersonId, announcementType);
        } 
        
        public static IList<BaseApplicationViewData> GetSuggestedAppsForAttach(IServiceLocatorMaster masterLocator, IServiceLocatorSchool schooLocator
            , int personId, int classId, IList<Guid> abIds, int markingPeriodId, int? start = null, int? count = null)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            
            var applications = masterLocator.ApplicationService.GetSuggestedApplications(abIds, start.Value, count.Value);           
            applications = applications.Where(a => a.CanAttach).ToList();

            var res = applications.Select(BaseApplicationViewData.Create);
            
            // get without content apps only 
            return res.Where(x => !x.ApplicationAccess.ProvidesRecommendedContent).ToList();
        }

    }
}