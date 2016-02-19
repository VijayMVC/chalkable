REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.MessageId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.classes.Class');
REQUIRE('ria.serialize.SJX');

NAMESPACE('chlk.models.messages', function () {

    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.messages.MessageTypeEnum*/
    ENUM('MessageTypeEnum', {
        INCOME: 1,
        SENT_FOR_PERSON: 2,
        SENT_FOR_CLASS: 3
    });

    /** @class chlk.models.messages.Message*/
    CLASS(
        'Message', IMPLEMENTS(ria.serialize.IDeserializable), [
            chlk.models.id.MessageId, 'id',

            chlk.models.common.ChlkDate, 'sent',

            Boolean, 'replay',

            String, 'subject',

            String, 'body',

            Boolean, 'read',

            Boolean, 'deleteBySender',

            Boolean, 'deleteByRecipient',

            chlk.models.people.User, 'sender',

            chlk.models.people.User, 'recipientPerson',

            chlk.models.classes.Class, 'recipientClass',

            Boolean, 'inbox',

            chlk.models.messages.MessageTypeEnum, 'type',

            Boolean, 'disabledMessaging',

            VOID, function deserialize(raw) {
                if(raw.incomemessagedata){
                    this.id = SJX.fromValue(raw.incomemessagedata.id, chlk.models.id.MessageId);
                    this.body = SJX.fromValue(raw.incomemessagedata.body, String);
                    this.subject = SJX.fromValue(raw.incomemessagedata.subject, String);
                    this.deleteByRecipient = SJX.fromValue(raw.incomemessagedata.deletebyrecipient, Boolean);
                    this.read = SJX.fromValue(raw.incomemessagedata.read, Boolean);
                    this.sender = SJX.fromDeserializable(raw.incomemessagedata.sender, chlk.models.people.User);
                    this.sent = SJX.fromDeserializable(raw.incomemessagedata.sent, chlk.models.common.ChlkDate);
                    this.type = chlk.models.messages.MessageTypeEnum.INCOME;
                }else{
                    if(raw.sentmessagedata){
                        this.id = SJX.fromValue(raw.sentmessagedata.id, chlk.models.id.MessageId);
                        this.body = SJX.fromValue(raw.sentmessagedata.body, String);
                        this.subject = SJX.fromValue(raw.sentmessagedata.subject, String);
                        this.deleteBySender = SJX.fromValue(raw.sentmessagedata.deletedbysender, Boolean);
                        this.recipientPerson = SJX.fromDeserializable(raw.sentmessagedata.recipientperson, chlk.models.people.User);
                        this.recipientClass = SJX.fromDeserializable(raw.sentmessagedata.recipientclass, chlk.models.classes.Class);
                        this.sent = SJX.fromDeserializable(raw.sentmessagedata.sent, chlk.models.common.ChlkDate);
                        this.type = raw.sentmessagedata.recipientperson ? chlk.models.messages.MessageTypeEnum.SENT_FOR_PERSON : chlk.models.messages.MessageTypeEnum.SENT_FOR_CLASS;
                    }
                }
            },


            String, function getShortSubject(){
                return buildShortText(this.getSubject(), 50);
            },
            String, function getShortBody(){
                return buildShortText(this.getBody(), 70);
            },

            [[Boolean, String, String, chlk.models.people.User, chlk.models.common.ChlkDate]],
            function $(inbox_, body_, subject_, recipient_, sent_, disabled_){
                BASE();
                if(inbox_)
                    this.setInbox(inbox_);
                if(body_)
                    this.setBody(body_);
                if(subject_)
                    this.setSubject(subject_);
                if(recipient_)
                    this.setRecipientPerson(recipient_);
                if(sent_)
                    this.setSent(sent_);
                this.setDisabledMessaging(disabled_ || false);
            }
        ]);
});
