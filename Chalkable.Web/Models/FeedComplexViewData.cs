using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{
    public class FeedComplexViewData
    {
        public IList<AnnouncementViewData> AnnoucementViewDatas { get; set; }
        public bool? LessonPlansOnly { get; set; }
        public bool? SortType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}