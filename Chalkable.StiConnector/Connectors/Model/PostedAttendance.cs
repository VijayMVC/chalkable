using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class PostedAttendance
    {
        public DateTime Date { get; set; }

        public int SectionId { get; set; }

        public TimeSpan Time { get; set; }

    }
}
