using System.Collections.Generic;
using Chalkable.BusinessLogic.Services.School.Announcements;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{
    public class FeedComplexViewData
    {
        public IList<AnnouncementViewData> AnnoucementViewDatas { get; set; }
        public FeedSettings SettingsForFeed { get; set; }
    }
}