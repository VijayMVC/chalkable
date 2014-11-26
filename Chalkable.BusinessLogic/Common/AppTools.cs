using System.Text;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Master.Model;

namespace Chalkable.BusinessLogic.Common
{
    public static class AppTools
    {
        public const string APPS_VIEW_PAGE = "view";
        public const string APPS_MY_VIEW_PAGE = "myview";
        public const string APPS_EDIT_PAGE = "edit";
        public const string APPS_GRADING_VIEW_PAGE = "gradingview";
        public const string APPS_MANIFEST_PAGE = "manifest.xml";
        public const string PERMISSION_TAG = "permission";
        public const string NAMESPACE_TAG = "namespace";
        public const string AUTH_CODE_PARAM = "code";
        public const string ANNOUNCEMENT_APPLICATION_ID_PARAM = "announcementapplicationid";
        public const string APPLICATION_INSTALL_ID_PARAM = "applicationinstallid";
        public const string MODE_PARAM = "mode";
        public const string ICON_17_FILE = "icon17.png";
        public const string ICON_47_FILE = "icon47.png";

        public static string BuildAppUrl(Application application, int? announcementAppId, int? appInstallId, AppMode mode)
        {
            string page;
            switch (mode)
            {
                case AppMode.View:
                    page = APPS_VIEW_PAGE;
                    break;
                case AppMode.MyView:
                    page = APPS_MY_VIEW_PAGE;
                    break;
                case AppMode.Edit:
                    page = APPS_EDIT_PAGE;
                    break;
                case AppMode.GradingView:
                    page = APPS_GRADING_VIEW_PAGE;
                    break;
                default:
                    throw new ChalkableException(ChlkResources.ERR_INVALID_APPLICATION_MODE);
            }
            var url = application.Url;
            var paramsBuilder = new StringBuilder();
            paramsBuilder.AppendFormat("{0}={1}&", MODE_PARAM, page);
            if (announcementAppId.HasValue)
                paramsBuilder.AppendFormat("{0}={1}&", ANNOUNCEMENT_APPLICATION_ID_PARAM, announcementAppId.Value);
            if (appInstallId.HasValue)
                paramsBuilder.AppendFormat("{0}={1}", APPLICATION_INSTALL_ID_PARAM, appInstallId.Value);

            var fmt = url.Contains("?") ? "{0}&{1}" : "{0}?{1}";
            return paramsBuilder.Length == 0 ? url : string.Format(fmt, url, paramsBuilder);
        }

        public static string BuildIconUrl(Application application, bool large)
        {
            var url = UrlTools.UrlCombine(application.Url, large ? ICON_47_FILE : ICON_17_FILE);
            return url;
        }
    }
}
