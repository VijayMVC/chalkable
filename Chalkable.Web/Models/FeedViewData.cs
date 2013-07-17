using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models
{
    public class FeedViewData
    {
        public string FType { get; set; }
        public object Value { get; set; }

        private const string ANNOUNCEMENT = "Announcement";
        
        public static FeedViewData Create(AnnouncementComplex announcement)
        {
            var res = new FeedViewData
            {
                FType = ANNOUNCEMENT,
                Value = AnnouncementViewData.Create(announcement),
            };
            return res;
        }
        public static IList<FeedViewData> Create(IList<AnnouncementComplex> announcements)
        {
            return announcements.Select(Create).ToList();
        }
    }
}