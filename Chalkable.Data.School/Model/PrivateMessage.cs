using System;
using System.Collections.Generic;
using Chalkable.Data.Common;

namespace Chalkable.Data.School.Model
{
    public class PrivateMessage
    {
        public const string ID_FIELD = "Id";
        public const string FROM_PERSON_REF_FIELD = "FromPersonRef";
        public const string DELETED_BY_SENDER_FIELD = "DeletedBySender";
        public const string SUBJECT_FIELD = "Subject";
        public const string BODY_FIELD = "Body";
        public const string SENT_FIELD = "Sent";

        [IdentityFieldAttr]
        [PrimaryKeyFieldAttr]
        public int Id { get; set; }
        public int FromPersonRef { get; set; }
        public DateTime? Sent { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool DeletedBySender { get; set; }
        [NotDbFieldAttr]
        public Person Sender { get; set; }
    }


    public class PrivateMessageRecipient
    {
        public const string PRIVATE_MESSAGE_REF_FIELD = "PrivateMessageRef";
        public const string REPICENT_REF_FIELD = "RecipientRef";
        public const string RECIPIENT_CLASS_REF_FIELD = "RecipientClassRef";
        public const string DELETED_BY_RECIPIENT_FIELD = "DeletedByRecipient";
        public const string READ_FIELD = "Read";

        [PrimaryKeyFieldAttr]
        public int PrivateMessageRef { get; set; }
        [PrimaryKeyFieldAttr]
        public int RecipientRef { get; set; }
        public bool Read { get; set; }
        public bool DeletedByRecipient { get; set; }
        public int? RecipientClassRef { get; set; }
    }

    public class IncomePrivateMessage : PrivateMessage
    {   public bool Read { get; set; }
        public bool DeletedByRecipient { get; set; }
    }

    public class SentPrivateMessage : PrivateMessage
    {
        public IList<Person> RecipientPersons { get; set; }
        public Class RecipientClass { get; set; } 
    }

    public class PossibleMessageRecipients
    {
        public IList<Person> Persons { get; set; }
        public IList<Class> Classes { get; set; }  
    }

}
