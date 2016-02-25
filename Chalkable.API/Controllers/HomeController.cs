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
            if (mode == Settings.CONTENT_QUERY)
            {
                if (!Request.Headers.AllKeys.Contains("Authorization"))
                    return ChlkJsonResult(new ChalkableApiException("Security error. Missing token"), false);

                var token = Request.Headers["Authorization"];
                token = token.Replace("Bearer:", "").Trim();

                if (string.IsNullOrWhiteSpace(apiRoot) && Request.UrlReferrer != null)
                {
                    apiRoot = $"{Request.UrlReferrer.Scheme}://{Request.UrlReferrer.Host}";
                }
                return ProcessApplicationContent(token, apiRoot, announcementId, announcementType, start, count);
            }
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(apiRoot))
                return new EmptyResult();

            ChalkableAuthorization = ChalkableAuthorization ?? new ChalkableAuthorization(apiRoot);
            if (ChalkableAuthorization.ApiRoot != apiRoot)
                ChalkableAuthorization = new ChalkableAuthorization(apiRoot);

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


        private ActionResult ProcessApplicationContent(string token, string apiRoot, int? announcementId, int? announcementType, int? start, int? count)
        {
            var standards = StandardInfo.FromQuery(Request.Params, HttpContext.Server.UrlDecode).ToList();

            var builder = new StringBuilder();
            builder.Append($"{announcementId}|{announcementType}|");
            if (standards.Count > 0)
                builder.Append(standards.Select(x => 
                {
                    var res = "";
                    if (!string.IsNullOrWhiteSpace(x.CommonCoreStandard))
                        res += x.CommonCoreStandard + "|";
                    if (!string.IsNullOrWhiteSpace(x.StandardId))
                        res += x.StandardId + "|";
                    if (!string.IsNullOrWhiteSpace(x.StandardName))
                        res += x.StandardName + "|";
                    return res;
                })
                .JoinString(""));

            var appKey = Settings.GetConfiguration(apiRoot).AppSecret;
            var encodedKey = HashHelper.HexOfCumputedHash(appKey);
            builder.Append(encodedKey);
            var hash = HashHelper.HexOfCumputedHash(builder.ToString());
            if (token != hash)
                return ChlkJsonResult(new ChalkableApiException("Security error. Invalid token"), false);
            try
            {
                var res = GetApplicationContents(standards, start, count);
                return res == null 
                    ? ChlkJsonResult(new ChalkableApiException("Empty Application Content Result"), false)
                    : PaginatedListJsonResult(res.ApplicationContents, res.TotalCount);
            }
            catch (Exception ex)
            {
                return ChlkJsonResult(ex, false);
            }
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
