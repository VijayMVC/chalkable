using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.API.Models.AcademicBenchmark
{
    public class Topic
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public string Deepest { get; set; }
        public short Level { get; set; }
        public string Status { get; set; }
        public Guid ParentId { get; set; }
        public bool IsDeepest => Deepest == "Y";
        public bool IsActive => Status == "Active";
        public Authority Authority { get; set; }
        public Document Document { get; set; }
    }
}
