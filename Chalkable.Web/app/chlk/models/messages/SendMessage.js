REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.people.User');


NAMESPACE('chlk.models.messages', function () {

    "use strict";
    /** @class chlk.models.messages.SendMessage*/
    CLASS(
        'SendMessage', [
            chlk.models.id.SchoolPersonId, 'recipientId',
            String, 'subject',
            String, 'body',
            Boolean, 'inbox'
        ]);
});

