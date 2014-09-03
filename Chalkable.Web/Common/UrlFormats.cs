using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Common
{
    public static class UrlsConstants
    {
        public const string SETUP_URL_FORMAT = "setup/hello/{0}";
        public const string INVITE_URL_FORMAT = "setup/invite";
        public const string SCHEDULE_URL_FORMAT = "schools/schedule/{0}";
        public const string ATTENDANCE_CLASS_LIST_URL_FORMAT = "attendances/class-list/{0}";
        public const string DEV_RESET_PASSWORD_URL = "developer/viewChangePassword";
        public const string FUNDS_URL = "funds/schoolPersonFunds";
    }
}