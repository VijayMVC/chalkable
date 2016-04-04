using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess.AnnouncementsDataAccess;
using Chalkable.Data.School.Model;
using Chalkable.Data.School.Model.Announcements;

namespace Chalkable.BusinessLogic.Model
{
    public class FeedSettingsInfo
    {
        public int? AnnouncementType { get; set; }
        public int? SortType { get; set; }

        public AnnouncementSortOption? SortTypeEnum
        {
            get { return (AnnouncementSortOption?) SortType; }
            set { SortType = (int?) value; }
        }

        public AnnouncementTypeEnum? AnnouncementTypeEnum
        {
            get { return (AnnouncementTypeEnum?) AnnouncementType; }
            set { AnnouncementType = (int?) value; }
        }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? GradingPeriodId { get; set; }
        public bool AnyDate { get; set; }

        public FeedSettingsInfo() { }

        public FeedSettingsInfo(IDictionary<string, string> kv)
        {
            if (kv.ContainsKey(PersonSetting.FEED_START_DATE))
                FromDate = DateTimeTools.ParseExactNullable(kv[PersonSetting.FEED_START_DATE]);
            if (kv.ContainsKey(PersonSetting.FEED_END_DATE))
                ToDate = DateTimeTools.ParseExactNullable(kv[PersonSetting.FEED_END_DATE]);

            if (kv.ContainsKey(PersonSetting.FEED_GRADING_PERIOD_ID) && !string.IsNullOrWhiteSpace(kv[PersonSetting.FEED_GRADING_PERIOD_ID]))
                GradingPeriodId = int.Parse(kv[PersonSetting.FEED_GRADING_PERIOD_ID]);

            if (kv.ContainsKey(PersonSetting.FEED_ANNOUNCEMENT_TYPE) && !string.IsNullOrWhiteSpace(kv[PersonSetting.FEED_ANNOUNCEMENT_TYPE]))
                AnnouncementType = int.Parse(kv[PersonSetting.FEED_ANNOUNCEMENT_TYPE]);

            if (kv.ContainsKey(PersonSetting.FEED_SORTING) && !string.IsNullOrWhiteSpace(kv[PersonSetting.FEED_SORTING]))
            SortType = int.Parse(kv[PersonSetting.FEED_SORTING]);

            AnyDate = !GradingPeriodId.HasValue && !FromDate.HasValue && !ToDate.HasValue;
        }

        private static bool GetBoolFromDictionary(string key, IDictionary<string, string> keyValue)
        {
            return keyValue.ContainsKey(key) && !string.IsNullOrWhiteSpace(keyValue[key]) && bool.Parse(keyValue[key]);
        }


        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                [PersonSetting.FEED_START_DATE] = FromDate,
                [PersonSetting.FEED_END_DATE] = ToDate,
                [PersonSetting.FEED_ANNOUNCEMENT_TYPE] = AnnouncementType,
                [PersonSetting.FEED_GRADING_PERIOD_ID] = GradingPeriodId,
                [PersonSetting.FEED_SORTING] = SortType
            };
        }
    }
}
