using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ClassAnnouncementType
    {
        public const string CLASS_REF_FIELD = "ClassRef";
        public const string PERCENTAGE_FIELD = "Percentage";
        
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Gradable { get; set; }
        public decimal Percentage { get; set; }
        public int ClassRef { get; set; }
        public int? ChalkableAnnouncementTypeRef { get; set; }
    }

    public class GradedClassAnnouncementType : ClassAnnouncementType
    {
        public double? Avg { get; set; }      
    }
}
