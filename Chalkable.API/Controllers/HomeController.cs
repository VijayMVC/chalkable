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
        public virtual async Task<ActionResult> Index(string mode, string token, string apiRoot, int? announcementApplicationId,
            int? studentId, int? announcementId, int? announcementType, int? attributeId, int? start, int? count, string contentId
            ,int? classId, int? schoolId)
        {

            if (string.IsNullOrWhiteSpace(apiRoot))
                return new EmptyResult();

            ChalkableAuthorization = ChalkableAuthorization ?? new ChalkableAuthorization(apiRoot);
            if (ChalkableAuthorization.ApiRoot != apiRoot)
                ChalkableAuthorization = new ChalkableAuthorization(apiRoot);

            var standards = StandardInfo.FromQuery(Request.Params, HttpContext.Server.UrlDecode).ToList();

            if (string.IsNullOrWhiteSpace(token))
                return new EmptyResult();

            if (IsChalkableCallBack(mode))
            {
                try
                {
                    await AuthorizeQueryRequest(token);
                    switch (mode)
                    {
                        case Settings.CONTENT_QUERY:
                            return ChlkJsonResult(GetApplicationContents(standards, start, count), true);

                        case Settings.ANNOUNCEMENT_APPLICATION_SUBMIT:
                        case Settings.ANNOUNCEMENT_APPLICATION_REMOVE:
                            HandleChalkableNotification(mode, announcementApplicationId, announcementType);
                            return ChlkJsonResult(true, true);

                        default:
                            throw new Exception($"Unknown mode \"{mode}\"");
                    }
                }
                catch (Exception ex)
                {
                    return ChlkExceptionJsonResult(ex);
                }
            }

            await ChalkableAuthorization.AuthorizeAsync(token);

            if (string.IsNullOrWhiteSpace(mode))
                mode = Settings.MY_VIEW_MODE;

            CurrentUser = await GetCurrentUser(mode);
        
            return await ResolveAction(mode, announcementApplicationId, studentId, announcementId, announcementType,
                attributeId, StandardInfo.FromQuery(Request.Params, HttpContext.Server.UrlDecode), contentId, classId, schoolId);
        }



        private bool IsChalkableCallBack(string mode)
        {
            return mode == Settings.CONTENT_QUERY
                   || mode == Settings.ANNOUNCEMENT_APPLICATION_SUBMIT
                   || mode == Settings.ANNOUNCEMENT_APPLICATION_REMOVE;
        }

        protected virtual async Task<SchoolPerson> GetCurrentUser(string mode)
        {
            return mode == Settings.SYSADMIN_MODE
                ? SchoolPerson.SYSADMIN
                : await new ChalkableConnector(ChalkableAuthorization).Person.GetMe();
        }

        private async Task AuthorizeQueryRequest(string accessToken)
        {
            var authenticationSignature = Request.Headers[ChalkableAuthorization.AuthenticationHeaderName];
            if (string.IsNullOrWhiteSpace(authenticationSignature))
                throw new ChalkableApiException("Security error. Missing token");
            authenticationSignature = authenticationSignature.Replace($"{ChalkableAuthorization.AuthenticationSignature}:", "").Trim();

            var identityParams = GetQueryIdentityParams();
            await ChalkableAuthorization.AuthorizeQueryRequest(authenticationSignature, identityParams, accessToken);
        }

        private IList<string> GetQueryIdentityParams()
        {
            var paramNames = Request.QueryString.AllKeys.Length > 0
                ? Request.QueryString.AllKeys
                : Request.Form.AllKeys;
            var orderedPrKeys = paramNames.OrderBy(x => x).ToList();
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

        protected virtual void HandleChalkableNotification(string mode, int? announcementApplicationId, int? announcementType)
        {
            // do nothing, silently ignore
        }


        protected abstract Task<ActionResult> ResolveAction(string mode, int? announcementApplicationId,
            int? studentId, int? announcementId, int? announcementType, int? attributeId,
            IEnumerable<StandardInfo> standards, string contentId, int? classId, int? schoolId);
    }
}
