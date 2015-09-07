using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model.Sis
{
    public class AlternateScore
    {
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IncludeInAverage { get; set; }
        public decimal? PercentOfMaximumScore { get; set; }
    }
}
