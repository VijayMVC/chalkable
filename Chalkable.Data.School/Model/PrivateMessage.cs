using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class PrivateMessage
    {
        [IdentityFieldAttr]
        public int Id { get; set; }
        public int FromPersonRef { get; set; }
        public int ToPersonRef { get; set; }
        public DateTime? Sent { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Read { get; set; }
        public bool DeletedBySender { get; set; }
        public bool DeletedByRecipient { get; set; }
    }

    public class PrivateMessageDetails : PrivateMessage
    {
        private Person sender;
        private Person recipient;

        public Person Sender
        {
            get { return sender; }
            set
            {
                sender = value;
                if (value.Id == 0 && FromPersonRef != 0)
                    value.Id = FromPersonRef;
            }
        }
        public Person Recipient
        {
            get { return recipient; }
            set
            {
                recipient = value;
                if (value.Id == 0 && ToPersonRef != 0)
                    value.Id = ToPersonRef;
            }
        }
    }
}
