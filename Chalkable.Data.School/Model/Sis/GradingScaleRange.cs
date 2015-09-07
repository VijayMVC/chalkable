﻿using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class GradingScaleRange
    {
        public const string GRADING_SCALE_REF_FIELD = "GradingScaleRef";
        public const string ALPHA_GRADE_REF_FIELD = "AlphaGradeRef";
        public const string HIGH_VALUE_FIELD = "HighValue";

        [PrimaryKeyFieldAttr]
        public int GradingScaleRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int AlphaGradeRef { get; set; }
        public decimal LowValue { get; set; }
        public decimal HighValue { get; set; }
        public int AveragingEquivalent { get; set; }
        public bool AwardGradCredit { get; set; }
        public bool IsPassing { get; set; }
    }
}
