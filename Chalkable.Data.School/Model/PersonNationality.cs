using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class PersonNationality
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int PersonRef { get; set; }
        public string Nationality { get; set; }
        public bool IsPrimary { get; set; }
        public int CountryRef { get; set; }

    }
}
