using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Chalkable.API.ActionFilters;
using Chalkable.API.Exceptions;
using Chalkable.API.Helpers;
using Chalkable.API.Models;

namespace Chalkable.API.Controllers
{
    public abstract class HomeController: BaseController
    {
        [AllowCorsPolicy]
        public virtual async Task<ActionResult> Index(string mode, string code, string apiRoot, int? announcementApplicationId,
            int? studentId, int? announcementId, int? announcementType, int? attributeId, int? applicationInstallId, int? start, int? count, string contentId)
        {

            if (string.IsNullOrWhiteSpace(apiRoot))
                return new EmptyResult();

            ChalkableAuthorization = ChalkableAuthorization ?? new ChalkableAuthorization(apiRoot);
            if (ChalkableAuthorization.ApiRoot != apiRoot)
                ChalkableAuthorization = new ChalkableAuthorization(apiRoot);

            if (mode == Settings.CONTENT_QUERY)
            {
                try
                {
                    var token = Request.Headers["Authorization"];
                    if (string.IsNullOrWhiteSpace(token))
                        return new EmptyResult();
                    token = token.Replace("Bearer:", "").Trim();
                    return ProcessApplicationContent(token, mode, announcementId, announcementType, start, count);
                }
                catch (Exception ex)
                {
                    return ChlkJsonResult(ex, false);
                }
            }

            if (string.IsNullOrWhiteSpace(code))
                return new EmptyResult();

            await ChalkableAuthorization.AuthorizeAsync(code);

            if (string.IsNullOrWhiteSpace(mode))
                mode = Settings.MY_VIEW_MODE;

            CurrentUser = await GetCurrentUser(mode);
        
            return await ResolveAction(mode, announcementApplicationId, studentId, announcementId, announcementType,
                attributeId, applicationInstallId, StandardInfo.FromQuery(Request.Params, HttpContext.Server.UrlDecode), contentId);
        }

        protected virtual async Task<SchoolPerson> GetCurrentUser(string mode)
        {
            return mode == Settings.SYSADMIN_MODE
                ? SchoolPerson.SYSADMIN
                : await new ChalkableConnector(ChalkableAuthorization).Person.GetMe();
        }

        private ActionResult ProcessApplicationContent(string token, string mode, int? announcementId, int? announcementType, int? start, int? count)
        {
            var standards = StandardInfo.FromQuery(Request.Params, HttpContext.Server.UrlDecode).ToList();
            var identityParams = PrepareContentQueryIdentityParams(announcementId, announcementType, standards);

            ChalkableAuthorization.AuthorizeQueryRequest(token, identityParams);

            var res = GetApplicationContents(standards, start, count);
            if (res == null)
                throw new ChalkableApiException($"Empty Application Content Result in {mode} request to {ChalkableAuthorization.Configuration.ApplicationRoot}");

            return PaginatedListJsonResult(res.ApplicationContents, res.TotalCount);  
        }

        private IList<string> PrepareContentQueryIdentityParams(int? announcementId, int? announcementType, IList<StandardInfo> standards)
        {
            var identityParams = new List<string>();
            if (announcementId.HasValue)
                identityParams.Add(announcementId.Value.ToString());
            if (announcementType.HasValue)
                identityParams.Add(announcementType.Value.ToString());
            foreach (var standard in standards)
            {
                if (!string.IsNullOrWhiteSpace(standard.CommonCoreStandard))
                    identityParams.Add(standard.CommonCoreStandard);
                if (!string.IsNullOrWhiteSpace(standard.StandardId))
                    identityParams.Add(standard.StandardId);
                if (!string.IsNullOrWhiteSpace(standard.StandardName))
                    identityParams.Add(standard.StandardName);
            }
            return identityParams;
        } 
        

        private JsonResult ChlkJsonResult(object data, bool success)
        {
            return new JsonResult
            {
                Data = new
                {
                    success,
                    data
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private JsonResult PaginatedListJsonResult(object data, int totalCount)
        {
            return new JsonResult
            {
                Data = new
                {
                    data,
                    totalcount = totalCount,
                    success = true
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public class StandardInfo
        {
            public string StandardId { get; set; }
            public string CommonCoreStandard { get; set; }
            public string StandardName { get; set; }

            internal static IEnumerable<StandardInfo> FromQuery(NameValueCollection urlParams, Func<string, string> urlDecodeMethod = null)
            {
                var p = new List<StandardInfo>();

                const int maxStandards = 1000;
                for (var index = 0; index < maxStandards; ++index)
                {
                    var ccCode = urlParams[Settings.CC_STANDARD_CODE_PARAM + $"[{index}]"];
                    var name = urlParams[Settings.STANDARD_NAME_PARAM + $"[{index}]"];
                    var id = urlParams[Settings.STANDARD_ID_PARAM + $"[{index}]"];

                    if (string.IsNullOrWhiteSpace(ccCode) 
                        && string.IsNullOrWhiteSpace(name) 
                        && string.IsNullOrWhiteSpace(id))
                        break; 

                    p.Add(new StandardInfo
                    {
                        CommonCoreStandard = string.IsNullOrWhiteSpace(ccCode) || urlDecodeMethod == null ? ccCode : urlDecodeMethod(ccCode),
                        StandardName = string.IsNullOrWhiteSpace(name) || urlDecodeMethod == null ? name : urlDecodeMethod(name),
                        StandardId = id
                    });
                }

                return p;
            }

        }

        protected abstract PaginatedListOfApplicationContent GetApplicationContents(IList<StandardInfo> standardInfos, int? start, int? count);

        protected abstract Task<ActionResult> ResolveAction(string mode, int? announcementApplicationId,
            int? studentId, int? announcementId, int? announcementType, int? attributeId, int? applicationInstallId,
            IEnumerable<StandardInfo> standards, string contentId);
    }
}
