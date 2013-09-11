REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.MessageId');
REQUIRE('chlk.models.people.Person');


NAMESPACE('chlk.models.messages', function () {

    "use strict";
    /** @class chlk.models.messages.Message*/
    CLASS(
        'Message', [
            chlk.models.id.MessageId, 'id',
            chlk.models.common.ChlkDate, 'sent',
            String, 'subject',
            String, 'body',
            Boolean, 'read',
            [ria.serialize.SerializeProperty('deletebysender')],
            Boolean, 'deleteBySender',
            [ria.serialize.SerializeProperty('deletebyrecipient')],
            Boolean, 'deleteByRecipient',
            chlk.models.people.Person, 'sender',
            chlk.models.people.Person, 'recipient'
        ]);
});
