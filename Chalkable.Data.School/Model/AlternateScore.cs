using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class AlternateScore
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IncludeInAverage { get; set; }
        public decimal? PercentOfMaximumScore { get; set; }
    }
}
