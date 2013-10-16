using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{
    public class PrivateMessageViewData
    {
        public Guid Id { get; set; }
        public DateTime? Sent { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Read { get; set; }
        public bool DeletedBySender { get; set; }
        public bool DeletedByRecipient { get; set; }
        public ShortPersonViewData Sender { get; set; }
        public ShortPersonViewData Recipient { get; set; }

        public static PrivateMessageViewData Create(PrivateMessageDetails privateMessage)
        {
           
            var res = new PrivateMessageViewData
            {
                Id = privateMessage.Id,
                Sent = privateMessage.Sent,
                Subject = privateMessage.Subject,
                Body = privateMessage.Body,
                Read = privateMessage.Read,
                DeletedBySender = privateMessage.DeletedBySender,
                DeletedByRecipient = privateMessage.DeletedByRecipient,
                Sender = ShortPersonViewData.Create(privateMessage.Sender),
                Recipient = ShortPersonViewData.Create(privateMessage.Recipient)
            };
            res.Read = privateMessage.Read;
            return res;
        }

        public static IList<PrivateMessageViewData> Create(IList<PrivateMessageDetails> privateMessages)
        {
            return privateMessages.Select(Create).ToList();
        } 

    }
}