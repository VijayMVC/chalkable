using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Chalkable.API.ActionFilters;
using Chalkable.API.Exceptions;
using Chalkable.API.Models;

namespace Chalkable.API.Controllers
{
    public abstract class HomeController: BaseController
    {
        [AllowCorsPolicy]
        public virtual async Task<ActionResult> Index(string mode, string code, string apiRoot, int? announcementApplicationId,
            int? studentId, int? announcementId, int? announcementType, int? attributeId, int? start, int? count, string contentId)
        {

            if (string.IsNullOrWhiteSpace(apiRoot))
                return new EmptyResult();

            ChalkableAuthorization = ChalkableAuthorization ?? new ChalkableAuthorization(apiRoot);
            if (ChalkableAuthorization.ApiRoot != apiRoot)
                ChalkableAuthorization = new ChalkableAuthorization(apiRoot);

            var standards = StandardInfo.FromQuery(Request.Params, HttpContext.Server.UrlDecode).ToList();

            if (mode == Settings.CONTENT_QUERY)
            {
                try
                {
                    AuthorizeQueryRequest();
                    var res = GetApplicationContents(standards, start, count);
                    return ChlkJsonResult(res, true);
                }
                catch (Exception ex)
                {
                    return ChlkExceptionJsonResult(ex);
                }
            }

            if (string.IsNullOrWhiteSpace(code))
                return new EmptyResult();

            await ChalkableAuthorization.AuthorizeAsync(code);

            if (string.IsNullOrWhiteSpace(mode))
                mode = Settings.MY_VIEW_MODE;

            CurrentUser = await GetCurrentUser(mode);
        
            return await ResolveAction(mode, announcementApplicationId, studentId, announcementId, announcementType,
                attributeId, StandardInfo.FromQuery(Request.Params, HttpContext.Server.UrlDecode), contentId);
        }

        protected virtual async Task<SchoolPerson> GetCurrentUser(string mode)
        {
            return mode == Settings.SYSADMIN_MODE
                ? SchoolPerson.SYSADMIN
                : await new ChalkableConnector(ChalkableAuthorization).Person.GetMe();
        }

        private void AuthorizeQueryRequest()
        {
            var token = Request.Headers["Authorization"];
            if (string.IsNullOrWhiteSpace(token))
                throw new ChalkableApiException("Security error. Missing token");
            token = token.Replace("Bearer:", "").Trim();

            var identityParams = GetQueryIdentityParams();
            ChalkableAuthorization.AuthorizeQueryRequest(token, identityParams);
        }

        private IList<string> GetQueryIdentityParams()
        {
            var orderedPrKeys = Request.QueryString.AllKeys.OrderBy(x => x).ToList();
            return orderedPrKeys.Select(prKey => Request.Params[prKey])
                                .Where(prValue => !string.IsNullOrWhiteSpace(prValue))
                                .ToList();
        } 
        
        private JsonResult ChlkJsonResult(object data, bool success)
        {
            var list = data as IPaginatedList;
            var res = (list == null) ? new JsonResult
            {
                Data = new
                {
                    success,
                    data
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            } : new JsonResult
            {
                Data = new
                {
                    success,
                    data,
                    totalcount = list.TotalCount
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
            return res;
        }

        private JsonResult ChlkExceptionJsonResult(Exception ex)
        {
            return new JsonResult
            {
                Data = new
                {
                    success = false,
                    data = ExceptionViewData.Create(ex, ex.InnerException)
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

        protected virtual PaginatedList<ApplicationContent> GetApplicationContents(IList<StandardInfo> standardInfos, int? start, int? count)
        {
            throw new NotImplementedException();
        }

        protected abstract Task<ActionResult> ResolveAction(string mode, int? announcementApplicationId,
            int? studentId, int? announcementId, int? announcementType, int? attributeId,
            IEnumerable<StandardInfo> standards, string contentId);
    }
}
