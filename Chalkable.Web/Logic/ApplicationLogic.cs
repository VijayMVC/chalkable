using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Chalkable.API.Helpers;
using System.Threading.Tasks;
using System.Web;
using Chalkable.API.Exceptions;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Authentication;
using Chalkable.Web.Models.ApplicationsViewData;
using Mindscape.Raygun4Net;

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

        //TODO: think to move notify Applications logic
        public static void NotifyApplications(IServiceLocatorMaster masterLocator, IList<AnnouncementApplication> annApps
            , int announcemnetType, string sessionKey, NotifyAppType type)
        {
            foreach (var annApp in annApps)
                NotifyApplication(masterLocator, annApp.ApplicationRef, annApp.Id,  announcemnetType, sessionKey, type);
        }

        public static void NotifyApplication(IServiceLocatorMaster masterLocator, Guid applicationId,
            int announcementApplicationId, int announcemnetType, string sessionKey, NotifyAppType type)
        {
            Task.Run(()=>NotifyApplicationAsync(masterLocator, applicationId, announcementApplicationId, announcemnetType, sessionKey, type));
        }

        private static readonly RaygunClient RaygunClient = new RaygunClient();
        public static async Task NotifyApplicationAsync(IServiceLocatorMaster masterLocator, Guid applicationId, int announcementApplicationId,
            int announcemnetType, string sessionKey, NotifyAppType type)
        {
            try
            {
                var app = masterLocator.ApplicationService.GetApplicationById(applicationId);
                var url = app.Url;
                var token = masterLocator.ApplicationService.GetAccessToken(applicationId, sessionKey);
                var mode = type == NotifyAppType.Attach
                    ? API.Settings.ANNOUNCEMENT_APPLICATION_SUBMIT
                    : API.Settings.ANNOUNCEMENT_APPLICATION_REMOVE;

                var ps = new Dictionary<string, string>
                {
                    ["apiRoot"] = "https://" + Chalkable.Common.Settings.Domain,
                    ["announcementApplicationId"] = announcementApplicationId.ToString(),
                    ["announcementType"] = announcemnetType.ToString(),
                    ["mode"] = mode,
                    ["token"] = token,
                    ["_"] = Guid.NewGuid().ToString()
                };

                var webRequest = (HttpWebRequest)WebRequest.Create(url);

                webRequest.KeepAlive = true;
                webRequest.Credentials = CredentialCache.DefaultCredentials;
                webRequest.Method = WebRequestMethods.Http.Post;
                webRequest.Accept = "application/json";
                webRequest.AllowAutoRedirect = false;
                SignRequest(webRequest, ps, app.SecretKey);
                PrepareParams(webRequest, ps);

                var response = await webRequest.GetResponseAsync();
                using (var stream = response.GetResponseStream())
                {
                    var statusCode = (response as HttpWebResponse)?.StatusCode;
                    if (stream == null || (statusCode.HasValue && statusCode.Value != HttpStatusCode.OK))
                        HandleException(new ChalkableException($"Server {url} faild to responce." +
                                                               $"Request Parameters: {ps.Select(x => $"{x.Key}={x.Value}").JoinString("&")}"));
                }
            }
            catch (WebException ex)
            {
                HandleException(ex);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        private static void HandleException(Exception e)
        {
#if !DEBUG
                RaygunClient.SendInBackground(e);
#endif
#if DEBUG
            throw e;
#endif
        }

        private static void PrepareParams(HttpWebRequest request, IDictionary<string, string> ps)
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);
            foreach (var p in ps)
                nvc.Add(p.Key, p.Value);
            var stream = new MemoryStream();
            var data = Encoding.ASCII.GetBytes(nvc.ToString());
            stream.Write(data, 0, data.Length);
            stream.Seek(0, SeekOrigin.Begin);
            request.ContentLength = stream.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            stream.CopyTo(request.GetRequestStream());
            stream.Dispose();

        }

        private static void SignRequest(HttpWebRequest request, IDictionary<string, string> p, string secretKey)
        {
            var encodedKey = HashHelper.HexOfCumputedHash(secretKey);
            var signature = GenerateSignature(p, encodedKey);
            request.Headers.Add(API.ChalkableAuthorization.AuthenticationHeaderName, $"{API.ChalkableAuthorization.AuthenticationSignature}:{signature}");
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