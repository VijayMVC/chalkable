REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.MessageId');
REQUIRE('chlk.models.people.User');


NAMESPACE('chlk.models.messages', function () {

    "use strict";
    /** @class chlk.models.messages.Message*/
    CLASS(
        'Message', [
            chlk.models.id.MessageId, 'id',
            chlk.models.common.ChlkDate, 'sent',
            Boolean, 'replay',
            String, 'subject',
            String, 'body',
            Boolean, 'read',
            [ria.serialize.SerializeProperty('deletebysender')],
            Boolean, 'deleteBySender',
            [ria.serialize.SerializeProperty('deletebyrecipient')],
            Boolean, 'deleteByRecipient',
            chlk.models.people.User, 'sender',
            chlk.models.people.User, 'recipient',

            [[String, String, chlk.models.people.User, chlk.models.common.ChlkDate]],
            function $(body_, subject_, recipient_, sent_){
                BASE();
                if(body_)
                    this.setBody(body_);
                if(subject_)
                    this.setSubject(subject_);
                if(recipient_)
                    this.setRecipient(recipient_);
                if(sent_)
                    this.setSent(sent_);
            }
        ]);
});
