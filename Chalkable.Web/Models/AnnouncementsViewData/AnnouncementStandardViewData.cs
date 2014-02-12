using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;

namespace Chalkable.Web.Models.AnnouncementsViewData
{
    public class AnnouncementStandardViewData
    {
        public int StandardId { get; set; }
        public int? ParentStandardRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StandardSubjectRef { get; set; }
        public int? LowerGradeLevelRef { get; set; }
        public int? UpperGradeLevelRef { get; set; }
        public bool IsActive { get; set; } 

        public static AnnouncementStandardViewData Create(Standard standard)
        {
            return new AnnouncementStandardViewData
                {
                    StandardId = standard.Id,
                    Description = standard.Description,
                    Name = standard.Name,
                    ParentStandardRef = standard.ParentStandardRef
                };
        }

        public static IList<AnnouncementStandardViewData> Create(IList<Standard> standards)
        {
            return standards.Select(Create).ToList();
        }
        public static IList<AnnouncementStandardViewData> Create(IList<AnnouncementStandardDetails> standardDetailses)
        {
            return standardDetailses.Select(x => Create(x.Standard)).ToList();
        }
    }
}