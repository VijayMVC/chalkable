using System.Collections.Generic;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{
    public class FeedComplexViewData
    {
        public IList<AnnouncementViewData> AnnoucementViewDatas { get; set; }
        public IList<AnnouncementViewData> CreatedAnnouncements { get; set; } 
        public FeedSettingsViewData SettingsForFeed { get; set; }
    }
}