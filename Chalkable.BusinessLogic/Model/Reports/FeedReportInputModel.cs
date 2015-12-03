﻿using System;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class FeedReportInputModel
    {
        public int? ClassId { get; set; }
        public FeedReportSettingsInfo Settings { get; set; }
        
        public ReportingFormat? FormatTyped
        {
            get { return (ReportingFormat?)Format; }
            set { Format = (int?)value; }
        }
        public int? Format { get; set; }
    }
}