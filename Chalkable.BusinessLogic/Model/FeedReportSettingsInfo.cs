using System;
using System.Collections.Generic;
using Chalkable.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Model
{
    public class FeedReportSettingsInfo
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IncludeDetails { get; set; }
        public bool IncludeHiddenActivities { get; set; }
        public bool IncludeHiddenAttributes { get; set; }
        public bool IncludeAttachments { get; set; }
        public bool LessonPlanOnly { get; set; }


        public FeedReportSettingsInfo() { }

        public FeedReportSettingsInfo(IDictionary<string, string> keyValue)
        {
            if (keyValue.ContainsKey(PersonSetting.FEED_REPORT_START_DATE))
                StartDate = DateTimeTools.ParseExactNullable(keyValue[PersonSetting.FEED_REPORT_START_DATE]);
            if (keyValue.ContainsKey(PersonSetting.FEED_REPORT_END_DATE))
                EndDate = DateTimeTools.ParseExactNullable(keyValue[PersonSetting.FEED_REPORT_END_DATE]);
            
            IncludeDetails = GetBoolFromDictionary(PersonSetting.FEED_REPORT_INCLUDE_DETAILS, keyValue);
            IncludeAttachments = GetBoolFromDictionary(PersonSetting.FEED_REPORT_INCLUDE_ATTACHMENTS, keyValue);
            IncludeHiddenAttributes = GetBoolFromDictionary(PersonSetting.FEED_REPORT_INCLUDE_HIDDEN_ATTRIBUTES, keyValue);
            IncludeHiddenActivities = GetBoolFromDictionary(PersonSetting.FEED_REPORT_INCLUDE_HIDDEN_ACTIVITIES, keyValue);
            LessonPlanOnly = GetBoolFromDictionary(PersonSetting.FEED_REPORT_LP_ONLY, keyValue);
        }

        private static bool GetBoolFromDictionary(string key, IDictionary<string, string> keyValue)
        {
            return keyValue.ContainsKey(key) && !string.IsNullOrWhiteSpace(keyValue[key]) && bool.Parse(keyValue[key]);
        }

        public IDictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                [PersonSetting.FEED_REPORT_START_DATE] = StartDate,
                [PersonSetting.FEED_REPORT_END_DATE] = EndDate,
                [PersonSetting.FEED_REPORT_INCLUDE_DETAILS] = IncludeDetails,
                [PersonSetting.FEED_REPORT_INCLUDE_HIDDEN_ACTIVITIES] = IncludeHiddenActivities,
                [PersonSetting.FEED_REPORT_INCLUDE_HIDDEN_ATTRIBUTES] = IncludeHiddenAttributes,
                [PersonSetting.FEED_REPORT_LP_ONLY] = LessonPlanOnly,
                [PersonSetting.FEED_REPORT_INCLUDE_ATTACHMENTS] = IncludeAttachments
            };
        }    
    }
}