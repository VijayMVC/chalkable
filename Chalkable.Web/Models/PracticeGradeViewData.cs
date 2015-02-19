﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.AnnouncementsViewData;

namespace Chalkable.Web.Models
{
    public class PracticeGradeGridViewData
    {
        public IList<StandardViewData> Standrds { get; set; }
        public IList<PracticeGradeViewData> PracticeGrades { get; set; }
 
        public static PracticeGradeGridViewData Create(IList<PracticeGradeDetailedInfo> practiceGrades, IList<Standard> standards)
        {
            var res = new PracticeGradeGridViewData
                {
                    Standrds = StandardViewData.Create(standards),
                    PracticeGrades = practiceGrades.Select(x => PracticeGradeViewData.Create(x.PracticeGrade, x.GradingStandardInfo, x.Standard)).ToList()
                };
            return res;
        }
    }

    public class PracticeGradeViewData
    {
        public StandardViewData Standard { get; set; }
        public string PracticeScore { get; set; }
        public string GradeBookScore { get; set; }
        public DateTime? GradedDate { get; set; }
        public Guid? ApplicationId { get; set; }

        public static PracticeGradeViewData Create(PracticeGrade practiceGrade, GradingStandardInfo gradingStandardInfo, StandardDetailsInfo standard)
        {
            var res = new PracticeGradeViewData {Standard = StandardViewData.Create(standard)};
            if (practiceGrade != null)
            {
                res.PracticeScore = practiceGrade.Score;
                res.GradedDate = practiceGrade.Date;
                res.ApplicationId = practiceGrade.ApplicationRef;
            }
            if (gradingStandardInfo != null)
            {
                var numericGrade = gradingStandardInfo.NumericGrade;
                res.GradeBookScore = gradingStandardInfo.AlphaGradeName ?? 
                    (numericGrade.HasValue ? numericGrade.Value.ToString() : null);
            }
            return res;
        }

    }
}