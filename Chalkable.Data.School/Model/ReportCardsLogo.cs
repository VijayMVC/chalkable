using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class ReportCardsLogo
    {
        [PrimaryKeyFieldAttr]
        [IdentityFieldAttr]
        public int Id { get; set; }
        public int? SchoolRef { get; set; }
        public string LogoAddress { get; set; }
    }
}
