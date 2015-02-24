using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Model;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{

    public class StandardViewData
    {
        public int StandardId { get; set; }
        public int? ParentStandardId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StandardSubjectId { get; set; }
        public string CCStandardCode { get; set; }
        public Guid? AcademicBenchmarkId { get; set; }
        
        protected StandardViewData(){}

        protected StandardViewData(Standard standard)
        {
            StandardId = standard.Id;
            Description = standard.Description;
            Name = standard.Name;
            ParentStandardId = standard.ParentStandardRef;
            StandardSubjectId = standard.StandardSubjectRef;
            AcademicBenchmarkId = standard.AcademicBenchmarkId;
            CCStandardCode = standard.CCStandardCode;
        }
        
        public static StandardViewData Create(Standard standard)
        {
            return new StandardViewData(standard);
        }

        public static StandardViewData Create(StandardDetailsInfo standardDetailsInfo)
        {
            return Create(standardDetailsInfo.Standard);
        }

        public static IList<StandardViewData> Create(IList<Standard> standards)
        {
            return standards.Select(Create).ToList();
        }
        public static IList<StandardViewData> Create(IList<AnnouncementStandardDetails> standardDetailses)
        {
            return standardDetailses.Select(x => Create(x.Standard)).ToList();
        }
    }
}