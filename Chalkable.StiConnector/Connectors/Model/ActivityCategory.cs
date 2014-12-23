using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class ActivityCategory
    {
        public string Description { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int SectionId { get; set; }
        public decimal? Percentage { get; set; }
    }
}
