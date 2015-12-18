﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Chalkable.Web.Logic;

namespace Chalkable.Web.Models.DisciplinesViewData
{
    public class ClassDisciplineSummaryViewData
    {
        public int ClassId { get; set; }
        public int DateType { get; set; }
        public IList<DailyStatsViewData> DailySummaries { get; set; }

        public static ClassDisciplineSummaryViewData Create(int classId, ClassLogic.DatePeriodTypeEnum datePeriodTpe, IList<DailyStatsViewData> dailySummaries)
        {
            return new ClassDisciplineSummaryViewData
            {
                ClassId = classId,
                DateType = (int) datePeriodTpe,
                DailySummaries = dailySummaries
            };
        }
    }

    public class DailyStatsViewData
    {
        public DateTime DateTime { get; set; }
        public string Summary { get; set; }
        public int Number { get; set; }

        
        public static DailyStatsViewData Create(DateTime date, int number, string format)
        {
            return new DailyStatsViewData {DateTime = date, Number = number, Summary = date.ToString(format, CultureInfo.InvariantCulture)};
        }
    }
}