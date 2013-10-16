using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.SisConnector.PublicModel
{
    public class SchoolYearInfo
    {
        public int CalendarId { get; set; }
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
