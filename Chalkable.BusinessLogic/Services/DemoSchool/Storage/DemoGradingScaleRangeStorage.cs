using System;
using System.Collections;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoGradingScaleRangeStorage: BaseDemoIntStorage<GradingScaleRange>
    {
        public DemoGradingScaleRangeStorage(DemoStorage storage)
            : base(storage, null, true)
        {
        }

        public override void Setup()
        {
            var gdScaleRanges = new List<GradingScaleRange>()
            {
                new GradingScaleRange()
                {
                    GradingScaleRef = 1,
                    AlphaGradeRef = 6,
                    LowValue = 84.50m,
                    HighValue = 100.0m,
                    AveragingEquivalent = 100,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 1,
                    AlphaGradeRef = 7,
                    LowValue = 69.50m,
                    HighValue = 84.49999m,
                    AveragingEquivalent = 84,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 1,
                    AlphaGradeRef = 8,
                    LowValue = 0.0m,
                    HighValue = 69.499999m,
                    AveragingEquivalent = 69,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 1,
                    LowValue = 93.50m,
                    HighValue = 96.49999m,
                    AveragingEquivalent = 96,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 2,
                    LowValue = 83.50m,
                    HighValue = 86.49999m,
                    AveragingEquivalent = 86,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 3,
                    LowValue = 73.50m,
                    HighValue = 76.49999m,
                    AveragingEquivalent = 76,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 4,
                    LowValue = 63.50m,
                    HighValue = 66.49999m,
                    AveragingEquivalent = 66,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 4,
                    LowValue = 63.50m,
                    HighValue = 66.49999m,
                    AveragingEquivalent = 66,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 5,
                    LowValue = 0.0m,
                    HighValue = 60.49999m,
                    AveragingEquivalent = 60,
                },
                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 9,
                    LowValue = 96.5m,
                    HighValue = 100.00m,
                    AveragingEquivalent = 100,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 10,
                    LowValue = 90.5m,
                    HighValue = 93.49999m,
                    AveragingEquivalent = 93,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 11,
                    LowValue = 86.5m,
                    HighValue = 90.49999m,
                    AveragingEquivalent = 90,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 12,
                    LowValue = 80.5m,
                    HighValue = 83.49999m,
                    AveragingEquivalent = 83,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 13,
                    LowValue = 76.5m,
                    HighValue = 80.49999m,
                    AveragingEquivalent = 80,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 14,
                    LowValue = 70.5m,
                    HighValue = 73.49999m,
                    AveragingEquivalent = 73,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 15,
                    LowValue = 66.5m,
                    HighValue = 70.49999m,
                    AveragingEquivalent = 70,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 2,
                    AlphaGradeRef = 16,
                    LowValue = 60.5m,
                    HighValue = 63.49999m,
                    AveragingEquivalent = 63,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 1,
                    LowValue = 93.0m,
                    HighValue = 97.49999m,
                    AveragingEquivalent = 95,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 2,
                    LowValue = 83.0m,
                    HighValue = 86.49999m,
                    AveragingEquivalent = 85,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 3,
                    LowValue = 73.0m,
                    HighValue = 76.49999m,
                    AveragingEquivalent = 75,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 4,
                    LowValue = 63.0m,
                    HighValue = 66.49999m,
                    AveragingEquivalent = 65,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 5,
                    LowValue = 1.0m,
                    HighValue = 59.49999m,
                    AveragingEquivalent = 50,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 6,
                    LowValue = 0.05m,
                    HighValue = 0.06m,
                    AveragingEquivalent = 0,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 7,
                    LowValue = 0.03m,
                    HighValue = 0.04m,
                    AveragingEquivalent = 0,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 8,
                    LowValue = 0.00m,
                    HighValue = 0.02m,
                    AveragingEquivalent = 0,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 9,
                    LowValue = 98.00m,
                    HighValue = 110.00m,
                    AveragingEquivalent = 100,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 10,
                    LowValue = 90.00m,
                    HighValue = 92.9999m,
                    AveragingEquivalent = 91,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 11,
                    LowValue = 87.00m,
                    HighValue = 89.9999m,
                    AveragingEquivalent = 88,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 12,
                    LowValue = 80.00m,
                    HighValue = 82.9999m,
                    AveragingEquivalent = 81,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 13,
                    LowValue = 77.00m,
                    HighValue = 79.9999m,
                    AveragingEquivalent = 78,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 14,
                    LowValue = 70.00m,
                    HighValue = 72.9999m,
                    AveragingEquivalent = 71,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 15,
                    LowValue = 67.00m,
                    HighValue = 69.9999m,
                    AveragingEquivalent = 68,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 16,
                    LowValue = 60.00m,
                    HighValue = 62.9999m,
                    AveragingEquivalent = 61,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 17,
                    LowValue = 00.70m,
                    HighValue = 00.8m,
                    AveragingEquivalent = 61,
                },

                new GradingScaleRange()
                {
                    GradingScaleRef = 3,
                    AlphaGradeRef = 19,
                    LowValue = 00.90m,
                    HighValue = 0.99999m,
                    AveragingEquivalent = 50,
                }
            };
            Add(gdScaleRanges);
        }
    }
}
