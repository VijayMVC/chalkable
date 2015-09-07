using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class StandardSubject
    {
        public const string ID_FIELD = "Id";

        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? AdoptionYear { get; set; }
        public bool IsActive { get; set; }
    }
}
