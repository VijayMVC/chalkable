using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Chalkable.API.Helpers;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
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


        public static void NotifyApplications(IServiceLocatorMaster masterLocator, IList<AnnouncementApplication> annApps,
            int announcementId, int announcemnetType, NotifyAppType type)
        {
            NotifyApplications(masterLocator, annApps.Select(x => x.ApplicationRef), announcementId, announcemnetType, type);
        }

        public static void NotifyApplications(IServiceLocatorMaster masterLocator, IEnumerable<Guid> applicationIds,
            int announcementId, int announcemnetType, NotifyAppType type)
        {
            foreach (var appId in applicationIds)
                Task.Run(() => NotifyApplicationAsync(masterLocator, appId, announcementId, announcemnetType, type));
        }


        public static async Task NotifyApplicationAsync(IServiceLocatorMaster masterLocator, Guid applicationId, int announcementId,
            int announcemnetType, NotifyAppType type)
        {
            var app = masterLocator.ApplicationService.GetApplicationById(applicationId);
            var url = app.Url;
            var sessionKey = ChalkableAuthentication.GetSessionKey();
            var token = masterLocator.ApplicationService.GetAccessToken(applicationId, sessionKey);
            var mode = type == NotifyAppType.Attach 
                ? API.Settings.ANNOUNCEMENT_APPLICATION_SUBMIT 
                : API.Settings.ANNOUNCEMENT_APPLICATION_REMOVE;

            var ps = new Dictionary<string, string>
            {
                ["apiRoot"] = "https://" + Chalkable.Common.Settings.Domain,
                ["announcementId"] = announcementId.ToString(),
                ["announcementType"] = announcemnetType.ToString(),
                ["mode"] = mode,
                ["token"] = token,
                ["_"] = Guid.NewGuid().ToString()
            };

            var client = new WebClient();
            try
            {
                SignRequest(client, ps, app.SecretKey);
                AddParams(client, ps);
                var task = client.DownloadDataTaskAsync(url);
                await task;

            }
            catch (WebException ex)
            {
                //TODO handler
                return;
            }
        }

        private static void AddParams(WebClient client, IDictionary<string, string> ps)
        {
            foreach (var p in ps)
                client.QueryString.Add(p.Key, p.Value);
        }

        private static void SignRequest(WebClient client, IDictionary<string, string> p, string secretKey)
        {
            var encodedKey = HashHelper.HexOfCumputedHash(secretKey);
            var signature = GenerateSignature(p, encodedKey);
            client.Headers.Add(API.ChalkableAuthorization.AuthenticationHeaderName, $"{API.ChalkableAuthorization.AuthenticationSignature}:{signature}");
        }


        private static string GenerateSignature(IDictionary<string, string> parameters, string encodedKey)
        {
            var values = parameters.OrderBy(x => x.Key).Select(x => x.Value).ToList();
            var msg = values.JoinString("|") + "|" + encodedKey;
            return HashHelper.HexOfCumputedHash(msg);
        }
    }
    public enum NotifyAppType
    {
        None = 0,
        Attach = 1,
        Remove = 2,
    }
}