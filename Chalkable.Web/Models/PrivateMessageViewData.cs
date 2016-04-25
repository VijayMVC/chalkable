using System;
using System.Collections.Generic;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;
using Chalkable.Web.Models.ClassesViewData;
using Chalkable.Web.Models.PersonViewDatas;

namespace Chalkable.Web.Models
{

    public class PrivateMessageShortViewData
    {
        public int Id { get; set; }
        public DateTime? Sent { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public ShortPersonViewData Sender { get; set; }

        protected PrivateMessageShortViewData(PrivateMessage message)
        {
            Id = message.Id;
            Sent = message.Sent;
            Subject = message.Subject;
            Body = message.Body;
        }

    }
    

    
    public class IncomePrivateMessageViewData : PrivateMessageShortViewData
    {

        public bool Read { get; set; }
        public bool DeletedByRecipient { get; set; }

        protected IncomePrivateMessageViewData(PrivateMessage message) : base(message)
        {
        }

        public static IncomePrivateMessageViewData Create(IncomePrivateMessage message)
        {
            return new IncomePrivateMessageViewData(message)
            {
                Read = message.Read,
                DeletedByRecipient = message.DeletedByRecipient,
                Sender = ShortPersonViewData.Create(message.Sender)
            };
        }
    }

    public class SentPrivateMessageViewData : PrivateMessageShortViewData
    {
        public bool DeletedBySender { get; set; }
        public ShortClassViewData RecipientClass { get; set; }
        public ShortPersonViewData RecipientPerson { get; set; }
        public IList<ShortPersonViewData> AllRecipients { get; set; }

        protected SentPrivateMessageViewData(PrivateMessage message) : base(message)
        {
        }

        public static SentPrivateMessageViewData Create(SentPrivateMessage message)
        {
            var res = new SentPrivateMessageViewData(message)
            {
                DeletedBySender = message.DeletedBySender,
                AllRecipients = ShortPersonViewData.Create(message.RecipientPersons)
            };
            if (message.RecipientClass != null)
                res.RecipientClass = ShortClassViewData.Create(message.RecipientClass);
            if (message.RecipientPersons.Count == 1)
                res.RecipientPerson = ShortPersonViewData.Create(message.RecipientPersons[0]);
            return res;
        }
    }

    public class PrivateMessageComplexViewData
    {
        public IncomePrivateMessageViewData IncomeMessageData { get; set; }
        public SentPrivateMessageViewData SentMessageData { get; set; }

        public static PrivateMessageComplexViewData Create(PrivateMessage privateMessage)
        {
            var incomeMessage = privateMessage as IncomePrivateMessage;
            if (incomeMessage != null)
                return new PrivateMessageComplexViewData
                {
                    IncomeMessageData = IncomePrivateMessageViewData.Create(incomeMessage)
                };
            var sentMessage = privateMessage as SentPrivateMessage;
            if (sentMessage != null)
                return new PrivateMessageComplexViewData
                {
                    SentMessageData = SentPrivateMessageViewData.Create(sentMessage)
                };
            throw new ChalkableException("Not supported private message type.");
        }
    }
}