using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chalkable.API.Helpers;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.Master.Model;
using Chalkable.Web.Authentication;
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
            , IList<Guid> abIds, int? start = null, int? count = null)
        {
            start = start ?? 0;
            count = count ?? int.MaxValue;
            
            var applications = masterLocator.ApplicationService.GetSuggestedApplications(abIds, start.Value, count.Value);           
            applications = applications.Where(a => a.CanAttach).ToList();

            var res = applications.Select(BaseApplicationViewData.Create);
            
            // get without content apps only 
            return res.Where(x => !x.ApplicationAccess.ProvidesRecommendedContent).ToList();
        }
        
        public static IList<BaseApplicationViewData> GetApplicationsWithContent(IServiceLocatorSchool schoolLocator, IServiceLocatorMaster masterLocator)
        {
            IList<Application> applications = masterLocator.ApplicationService.GetApplications(live: true);
            applications = applications.Where(x => x.ProvidesRecommendedContent).ToList();

            var res = BaseApplicationViewData.Create(applications);
            foreach (var app in res)
            {
                app.EncodedSecretKey = HashHelper.HexOfCumputedHash(applications.First(x => x.Id == app.Id).SecretKey);
                app.AccessToken = masterLocator.ApplicationService.GetAccessToken(app.Id, ChalkableAuthentication.GetSessionKey());
            }

            return res;
        }
    }
}