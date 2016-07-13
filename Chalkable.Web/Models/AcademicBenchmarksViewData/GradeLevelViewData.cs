﻿using GradeLevel = Chalkable.BusinessLogic.Model.AcademicBenchmark.GradeLevel;

namespace Chalkable.Web.Models.AcademicBenchmarksViewData
{
    public class GradeLevelViewData
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Low { get; set; }
        public string Height { get; set; }
        public static GradeLevelViewData Create(GradeLevel grLevel)
        {
            return new GradeLevelViewData
            {
                Description = grLevel.Description,
                Code = grLevel.Code,
                Height = grLevel.Height,
                Low = grLevel.Low
            };
        }
    }
}