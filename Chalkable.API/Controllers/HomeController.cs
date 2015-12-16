using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Chalkable.API.Controllers
{
    public abstract class HomeController: BaseController
    {
        public virtual async Task<ActionResult> Index(string mode, string code, string apiRoot, int? announcementApplicationId,
            int? studentId, int? announcementId, int? announcementType, int? attributeId, int? applicationInstallId)
        {
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
                attributeId, applicationInstallId, StandardInfo.FromQuery(Request.Params));
        }

        protected virtual async Task<Models.SchoolPerson> GetCurrentUser(string mode)
        {
            return await new ChalkableConnector(ChalkableAuthorization).Person.GetMe();
        }

        public class StandardInfo
        {
            public string StandardId { get; set; }
            public string CommonCoreStandard { get; set; }
            public string StandardName { get; set; }

            internal static IEnumerable<StandardInfo> FromQuery(NameValueCollection urlParams)
            {
                var p = new List<StandardInfo>();

                const int maxStandards = 1000;
                for (var index = 0; index < maxStandards; ++index)
                {
                    var ccCode = urlParams[Settings.CC_STANDARD_CODE_PARAM + $"[{index}]"];
                    var name = urlParams[Settings.STANDARD_NAME_PARAM + $"[{index}]"];
                    var id = urlParams[Settings.STANDARD_ID_PARAM + $"[{index}]"];

                    if (string.IsNullOrWhiteSpace(ccCode) || string.IsNullOrWhiteSpace(name) ||
                        string.IsNullOrWhiteSpace(id))
                        break;

                    p.Add(new StandardInfo
                    {
                        CommonCoreStandard = ccCode,
                        StandardName = name,
                        StandardId = id
                    });
                }

                return p;
            }

        }

        protected abstract Task<ActionResult> ResolveAction(string mode, int? announcementApplicationId,
            int? studentId, int? announcementId, int? announcementType, int? attributeId, int? applicationInstallId,
            IEnumerable<StandardInfo> standards);
    }
}
