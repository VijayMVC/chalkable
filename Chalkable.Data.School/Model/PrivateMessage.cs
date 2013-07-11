using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.School.Model
{
    public class PrivateMessage
    {
        public Guid Id { get; set; }
        public Guid FromPersonRef { get; set; }
        public Guid ToPersonRef { get; set; }
        public DateTime? Sent { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Read { get; set; }
        public bool DeletedBySender { get; set; }
        public bool DeletedByRecipient { get; set; }
    }
}
