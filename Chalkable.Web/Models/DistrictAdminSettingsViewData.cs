using System.Collections.Generic;
using Chalkable.Web.Models.ApplicationsViewData;

namespace Chalkable.Web.Models
{
    public class DistrictAdminSettingsViewData
    {
        public MessagingSettingsViewData MessagingSettings { get; set; }
        public IList<InstalledApplicationViewData> InstalledApps { get; set; }

        public static DistrictAdminSettingsViewData Create(MessagingSettingsViewData messagingSettings,
            IList<InstalledApplicationViewData> installedApps)
        {
            return new DistrictAdminSettingsViewData
            {
                MessagingSettings = messagingSettings,
                InstalledApps = installedApps
            };
        }
    }
}