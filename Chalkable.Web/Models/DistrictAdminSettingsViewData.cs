using System.Collections.Generic;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Models
{
    public class DistrictAdminSettingsViewData
    {
        public MessagingSettingsViewData MessagingSettings { get; set; }
        public IList<BaseApplicationViewData> InstalledApps { get; set; }

        public static DistrictAdminSettingsViewData Create(MessagingSettingsViewData messagingSettings,
            IList<BaseApplicationViewData> apps)
        {
            return new DistrictAdminSettingsViewData
            {
                MessagingSettings = messagingSettings,
                InstalledApps = apps
            };
        }
    }
}