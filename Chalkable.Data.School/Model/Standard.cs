﻿using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class Standard
    {
        public const string ID_FIELD = "Id";
        public const string STANDARD_SUBJECT_ID_FIELD = "StandardSubjectRef";
        public const string LOWER_GRADE_LEVEL_REF_FIELD = "LowerGradeLevelRef";
        public const string UPPER_GRADE_LEVEL_REF_FIELD = "UpperGradeLevelRef";
        public const string PARENT_STANDARD_REF_FIELD = "ParentStandardRef";
        public const string ACADEMIC_BENCHMARK_ID_FIELD = "AcademicBenchmarkId";

        public const string NAME_FIELD = "Name";
        public const string DESCRIPTION_FIELD = "Description";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }

        public int? ParentStandardRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StandardSubjectRef { get; set; }
        public int? LowerGradeLevelRef { get; set; }
        public int? UpperGradeLevelRef { get; set; }
        public bool IsActive { get; set; }
        public Guid? AcademicBenchmarkId { get; set; }

        [NotDbFieldAttr]
        public string CCStandardCode { get; set; }
    }


    public class StandardTreeItem : Standard
    {
        public IList<StandardTreeItem> StandardChildren { get; set; }
    }
}
