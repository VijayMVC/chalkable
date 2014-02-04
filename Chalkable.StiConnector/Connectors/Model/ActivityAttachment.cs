using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.StiConnector.Connectors.Model
{
    public class ActivityAttachment
    {
        public int ActivityId { get; set; }
        public int Id { get; set; }
        public string MimeType { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Uuid
        {
            get { return Text; }
        }
    }
}
